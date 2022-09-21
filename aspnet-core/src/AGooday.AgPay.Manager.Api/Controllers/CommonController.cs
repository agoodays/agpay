﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Services;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Manager.Api.Extensions.AuthContext;
using AGooday.AgPay.Manager.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class CommonController : ControllerBase
    {
        private readonly ILogger<CommonController> _logger;
        private readonly IDatabase _redis;
        private readonly IServer _redisServer;
        private readonly ISysUserService _sysUserService;
        private readonly ISysRoleEntRelaService _sysRoleEntRelaService;
        private readonly ISysUserRoleRelaService _sysUserRoleRelaService;

        public CommonController(ILogger<CommonController> logger, RedisUtil client,
            ISysUserService sysUserService,
            ISysRoleEntRelaService sysRoleEntRelaService,
            ISysUserRoleRelaService sysUserRoleRelaService)
        {
            _logger = logger;
            _redis = client.GetDatabase();
            _redisServer = client.GetServer();
            _sysUserService = sysUserService;
            _sysRoleEntRelaService = sysRoleEntRelaService;
            _sysUserRoleRelaService = sysUserRoleRelaService;
        }

        public CurrentUser GetCurrentUser()
        {
            string currentUser = _redis.StringGet(AuthContextService.CurrentUser.CacheKey);
            return JsonConvert.DeserializeObject<CurrentUser>(currentUser);
        }
        /// <summary>
        /// 根据用户ID 删除用户缓存信息
        /// </summary>
        /// <param name="sysUserIdList"></param>
        public void DelAuthentication(List<long> sysUserIdList)
        {
            if (sysUserIdList == null || sysUserIdList.Count<=0)
            {
                return;
            }
            foreach (var sysUserId in sysUserIdList)
            {
                var redisKeys = _redisServer.Keys(1, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    _redis.KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// 根据用户ID 更新缓存中的权限集合， 使得分配实时生效
        /// </summary>
        /// <param name="sysUserIdList"></param>
        public void RefAuthentication(List<long> sysUserIdList)
        {
            var sysUserMap = _sysUserService.GetAll(sysUserIdList);
            sysUserIdList.ForEach(sysUserId =>
            {
                var redisKeys = _redisServer.Keys(1, CS.GetCacheKeyToken(sysUserId, "*"));
                foreach (var key in redisKeys)
                {
                    //用户不存在 || 已禁用 需要删除Redis
                    if (!sysUserMap.Any(a => a.SysUserId.Equals(sysUserId))
                    || sysUserMap.Any(a => a.SysUserId.Equals(sysUserId) || a.State.Equals(CS.PUB_DISABLE)))
                    {
                        _redis.KeyDelete(key);
                        continue;
                    }
                    string currentUserJson = _redis.StringGet(AuthContextService.CurrentUser.CacheKey);
                    var currentUser = JsonConvert.DeserializeObject<CurrentUser>(currentUserJson);
                    if (currentUser == null)
                    {
                        continue;
                    }
                    var auth = sysUserMap.Where(w => w.SysUserId.Equals(sysUserId)).First();
                    var authorities = _sysUserRoleRelaService.SelectRoleIdsByUserId(auth.SysUserId).ToList();
                    authorities.AddRange(_sysRoleEntRelaService.SelectEntIdsByUserId(auth.SysUserId, auth.IsAdmin, auth.SysType));
                    currentUser.Authorities = authorities;
                    currentUserJson = JsonConvert.SerializeObject(currentUser);
                    //保存token  失效时间不变
                    _redis.StringSet(key, currentUserJson);
                }
            });
        }
    }
}
