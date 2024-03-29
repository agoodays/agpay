﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Components.MQ.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers.Isv
{
    /// <summary>
    /// 服务商管理类
    /// </summary>
    [Route("/api/isvInfo")]
    [ApiController, Authorize]
    public class IsvInfoController : CommonController
    {
        private readonly IMQSender mqSender;
        private readonly ILogger<IsvInfoController> _logger;
        private readonly IIsvInfoService _isvInfoService;

        public IsvInfoController(IMQSender mqSender, ILogger<IsvInfoController> logger, RedisUtil client,
            IIsvInfoService isvInfoService,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
            : base(logger, client, sysUserService, sysRoleEntRelaService, sysUserRoleRelaService)
        {
            this.mqSender = mqSender;
            _logger = logger;
            _isvInfoService = isvInfoService;
        }

        /// <summary>
        /// 查询服务商信息列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_LIST)]
        public ApiRes List([FromQuery] IsvInfoQueryDto dto)
        {
            var data = _isvInfoService.GetPaginatedData(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新增服务商
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route(""), MethodLog("新增服务商")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_ADD)]
        public ApiRes Add(IsvInfoDto dto)
        {
            dto.CreatedUid = GetCurrentUser().SysUser.SysUserId;
            dto.CreatedBy = GetCurrentUser().SysUser.Realname;
            var result = _isvInfoService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除服务商
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpDelete, Route("{isvNo}"), MethodLog("删除服务商")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_DEL)]
        public ApiRes Delete(string isvNo)
        {
            _isvInfoService.Remove(isvNo);

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, isvNo, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新服务商信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{isvNo}"), MethodLog("更新服务商信息")]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public ApiRes Update(string isvNo, IsvInfoDto dto)
        {
            _isvInfoService.Update(dto);

            // 推送mq到目前节点进行更新数据
            mqSender.Send(ResetIsvMchAppInfoConfigMQ.Build(ResetIsvMchAppInfoConfigMQ.RESET_TYPE_ISV_INFO, dto.IsvNo, null, null));

            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看服务商信息
        /// </summary>
        /// <param name="isvNo"></param>
        /// <returns></returns>
        [HttpGet, Route("{isvNo}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_ISV_INFO_VIEW, PermCode.MGR.ENT_ISV_INFO_EDIT)]
        public ApiRes Detail(string isvNo)
        {
            var isvInfo = _isvInfoService.GetById(isvNo);
            if (isvInfo == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(isvInfo);
        }
    }
}
