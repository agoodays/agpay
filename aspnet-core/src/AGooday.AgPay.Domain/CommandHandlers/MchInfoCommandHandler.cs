﻿using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Core.Notifications;
using AGooday.AgPay.Domain.Events.MchInfos;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace AGooday.AgPay.Domain.CommandHandlers
{
    public class MchInfoCommandHandler : CommandHandler,
        IRequestHandler<CreateMchInfoCommand, Unit>,
        IRequestHandler<ModifyMchInfoCommand, Unit>,
        IRequestHandler<RemoveMchInfoCommand, Unit>
    {
        // 注入仓储接口
        private readonly IMchInfoRepository _mchInfoRepository;
        private readonly IIsvInfoRepository _isvInfoRepository;
        private readonly ISysUserRepository _sysUserRepository;
        private readonly ISysUserAuthRepository _sysUserAuthRepository;
        private readonly IMchAppRepository _mchAppRepository;
        private readonly IPayOrderRepository _payOrderRepository;
        private readonly IMchPayPassageRepository _mchPayPassageRepository;
        private readonly IPayInterfaceConfigRepository _payInterfaceConfigRepository;
        private readonly IPayInterfaceDefineRepository _payInterfaceDefineRepository;

        // 用来进行DTO
        private readonly IMapper _mapper;

        // 注入总线
        private readonly IMediatorHandler Bus;
        private readonly IMQSender mqSender;
        private IMemoryCache Cache;

        public MchInfoCommandHandler(IUnitOfWork uow, IMediatorHandler bus, IMapper mapper, IMQSender mqSender, IMemoryCache cache,
            IMchInfoRepository mchInfoRepository,
            IIsvInfoRepository isvInfoRepository,
            ISysUserRepository sysUserRepository,
            ISysUserAuthRepository sysUserAuthRepository,
            IMchAppRepository mchAppRepository,
            IPayOrderRepository payOrderRepository,
            IMchPayPassageRepository mchPayPassageRepository,
            IPayInterfaceConfigRepository payInterfaceConfigRepository)
            : base(uow, bus, cache)
        {
            _mapper = mapper;
            Cache = cache;
            this.mqSender = mqSender;
            _sysUserRepository = sysUserRepository;
            _mchInfoRepository = mchInfoRepository;
            _isvInfoRepository = isvInfoRepository;
            _mchAppRepository = mchAppRepository;
            _sysUserAuthRepository = sysUserAuthRepository;
            _payOrderRepository = payOrderRepository;
            _mchPayPassageRepository = mchPayPassageRepository;
            _payInterfaceConfigRepository = payInterfaceConfigRepository;
        }

        public Task<Unit> Handle(CreateMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            var mchInfo = _mapper.Map<MchInfo>(request);
            do
            {
                mchInfo.MchNo = $"M{DateTimeOffset.Now.ToUnixTimeSeconds()}";
            } while (_mchInfoRepository.IsExistMchNo(mchInfo.MchNo));

            #region 检查
            // 校验特邀商户信息
            if (mchInfo.Type.Equals(CS.MCH_TYPE_ISVSUB) && !string.IsNullOrWhiteSpace(mchInfo.IsvNo))
            {
                // 当前服务商状态是否正确
                var isvInfo = _isvInfoRepository.GetById(mchInfo.IsvNo);
                if (isvInfo == null || isvInfo.State == CS.NO)
                {
                    Bus.RaiseEvent(new DomainNotification("", "当前服务商不可用！"));
                    return Task.FromResult(new Unit());
                }
            }
            #endregion

            #region 插入用户信息
            // 插入用户信息
            SysUser sysUser = new SysUser();
            sysUser.LoginUsername = request.LoginUsername;
            sysUser.Realname = mchInfo.ContactName;
            sysUser.Telphone = mchInfo.ContactTel;
            sysUser.UserNo = mchInfo.MchNo;
            sysUser.BelongInfoId = mchInfo.MchNo;
            sysUser.SysType = CS.SYS_TYPE.MCH;
            sysUser.Sex = CS.SEX_MALE;
            sysUser.AvatarUrl = "https://jeequan.oss-cn-beijing.aliyuncs.com/jeepay/img/defava_m.png";//默认头像
            sysUser.IsAdmin = CS.YES;
            sysUser.State = mchInfo.State;

            #region 检查
            // 登录用户名不可重复
            // 这些业务逻辑，当然要在领域层中（领域命令处理程序中）进行处理
            if (_sysUserRepository.IsExistLoginUsername(sysUser.LoginUsername, sysUser.SysType))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "登录名已经被使用！"));
                return Task.FromResult(new Unit());
            }
            // 手机号不可重复
            if (_sysUserRepository.IsExistTelphone(sysUser.Telphone, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "联系人手机号已存在！"));
                return Task.FromResult(new Unit());
            }
            // 员工号不可重复
            if (_sysUserRepository.IsExistUserNo(sysUser.UserNo, sysUser.SysType))
            {
                Bus.RaiseEvent(new DomainNotification("", "员工号已存在！"));
                return Task.FromResult(new Unit());
            }
            #endregion

            _sysUserRepository.Add(sysUser);

            #region 添加默认用户认证表
            string salt = StringUtil.GetUUID(6); //6位随机数
            string authPwd = CS.DEFAULT_PWD;
            string userPwd = BCrypt.Net.BCrypt.HashPassword(authPwd);
            //用户名登录方式
            var sysUserAuthByLoginUsername = new SysUserAuth()
            {
                UserId = sysUser.SysUserId,
                IdentityType = CS.AUTH_TYPE.LOGIN_USER_NAME,
                Identifier = sysUser.LoginUsername,
                Credential = userPwd,
                Salt = salt,
                SysType = sysUser.SysType
            };
            _sysUserAuthRepository.Add(sysUserAuthByLoginUsername);

            //手机号登录方式
            var sysUserAuthByTelphone = new SysUserAuth()
            {
                UserId = sysUser.SysUserId,
                IdentityType = CS.AUTH_TYPE.TELPHONE,
                Identifier = sysUser.Telphone,
                Credential = userPwd,
                Salt = salt,
                SysType = sysUser.SysType
            };
            _sysUserAuthRepository.Add(sysUserAuthByTelphone);
            #endregion
            #endregion

            #region 插入商户默认应用
            // 插入商户默认应用
            MchApp mchApp = new MchApp();
            mchApp.AppId = Guid.NewGuid().ToString("N").Substring(0, 24);
            mchApp.MchNo = mchInfo.MchNo;
            mchApp.AppName = "默认应用";
            mchApp.AppSecret = RandomUtil.RandomString(128);
            mchApp.State = CS.YES;
            mchApp.CreatedBy = sysUser.Realname;
            mchApp.CreatedUid = sysUser.SysUserId;

            _mchAppRepository.Add(mchApp);
            #endregion

            // 插入商户基本信息
            // 存入商户默认用户ID
            mchInfo.InitUserId = sysUser.SysUserId;
            _mchInfoRepository.Add(mchInfo);

            if (Commit())
            {
                // 提交成功后，这里需要发布领域事件
                // 比如欢迎用户注册邮件呀，短信呀等
                var createdevent = _mapper.Map<MchInfoCreatedEvent>(mchInfo);
                Bus.RaiseEvent(createdevent);
            }

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(ModifyMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            request.UpdatedAt = DateTime.Now;
            var mchInfo = _mapper.Map<MchInfo>(request);

            // 待删除用户登录信息的ID list
            var removeCacheUserIdList = new List<long>();

            // 如果商户状态为禁用状态，清除该商户用户登录信息
            if (mchInfo.State == CS.NO)
            {
                removeCacheUserIdList = _sysUserRepository.GetAll().Where(w => w.SysType.Equals(CS.SYS_TYPE.MCH) && w.BelongInfoId.Equals(mchInfo.MchNo))
                    .Select(w => w.SysUserId).ToList();
            }

            // 判断是否重置密码
            if (request.ResetPass)
            {
                // 待更新的密码
                string updatePwd = request.DefaultPass ? CS.DEFAULT_PWD : Base64Util.DecodeBase64(request.ConfirmPwd);
                // 获取商户超管
                long mchAdminUserId = _sysUserRepository.FindMchAdminUserId(mchInfo.MchNo);

                // 重置超管密码
                _sysUserAuthRepository.ResetAuthInfo(mchAdminUserId, null, null, updatePwd, CS.SYS_TYPE.MCH);
                _sysUserAuthRepository.SaveChanges();

                // 删除超管登录信息
                removeCacheUserIdList.Add(mchAdminUserId);
            }

            // 推送mq删除redis用户认证信息
            if (removeCacheUserIdList.Any())
            {
                mqSender.Send(CleanMchLoginAuthCacheMQ.Build(removeCacheUserIdList));
            }

            // 更新商户信息
            _mchInfoRepository.Update(mchInfo);
            _mchInfoRepository.SaveChanges();

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO, null, mchInfo.MchNo, null));

            Commit();

            return Task.FromResult(new Unit());
        }

        public Task<Unit> Handle(RemoveMchInfoCommand request, CancellationToken cancellationToken)
        {
            // 命令验证
            if (!request.IsValid())
            {
                // 错误信息收集
                NotifyValidationErrors(request);
                // 返回，结束当前线程
                return Task.FromResult(new Unit());
            }

            // 0.当前商户是否存在
            var mchInfo = _mchInfoRepository.GetById(request.MchNo);
            if (mchInfo is null)
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该商户不存在！"));
                return Task.FromResult(new Unit());
            }
            // 1.查看当前商户是否存在交易数据
            if (_payOrderRepository.IsExistOrderUseMchNo(request.MchNo))
            {
                // 引发错误事件
                Bus.RaiseEvent(new DomainNotification("", "该商户已存在交易数据，不可删除！"));
                return Task.FromResult(new Unit());
            }

            // 2.删除当前商户配置的支付通道
            _mchPayPassageRepository.RemoveByMchNo(request.MchNo);

            // 3.删除当前商户支付接口配置参数
            var appIds = _mchAppRepository.GetAll().Where(w => w.MchNo.Equals(request.MchNo)).Select(s => s.AppId).ToList();
            _payInterfaceConfigRepository.RemoveByInfoIds(appIds, CS.INFO_TYPE_MCH_APP);
            foreach (var appId in appIds)
            {
                _mchAppRepository.Remove(appId);
            }

            var sysUsers = _sysUserRepository.GetAll().Where(w => w.BelongInfoId.Equals(request.MchNo) && w.SysType.Equals(CS.SYS_TYPE.MCH));
            foreach (var sysUser in sysUsers)
            {
                var sysUserAuths = _sysUserAuthRepository.GetAll().Where(w => w.UserId.Equals(sysUser.SysUserId));
                // 删除当前商户用户认证信息
                foreach (var sysUserAuth in sysUserAuths)
                {
                    _sysUserAuthRepository.Remove(sysUserAuth.AuthId);
                }
                // 删除当前商户的登录用户
                _sysUserRepository.Remove(sysUser.SysUserId);
            }

            // 4.删除当前商户应用信息
            _mchInfoRepository.Remove(mchInfo.MchNo);

            // 推送mq删除redis用户缓存
            var userIdList = sysUsers.Select(s => s.SysUserId).ToList();
            mqSender.Send(CleanMchLoginAuthCacheMQ.Build(userIdList));

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_MCH_INFO, null, request.MchNo, null));

            Commit();

            return Task.FromResult(new Unit());
        }
    }
}
