﻿using AGooday.AgPay.Common.Constants;
using System.Security.Claims;

namespace AGooday.AgPay.Manager.Api.Extensions.AuthContext
{
    public static class AuthContextService
    {
        private static IHttpContextAccessor _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor;
        }
        /// <summary>
        /// 
        /// </summary>
        public static HttpContext Current => _context.HttpContext;
        /// <summary>
        /// 
        /// </summary>
        public static AuthContextUser CurrentUser
        {
            get
            {
                var user = new AuthContextUser
                {
                    SysUserId = Convert.ToInt64(Current.User.FindFirstValue(ClaimAttributes.SysUserId)),
                    AvatarUrl = Current.User.FindFirstValue(ClaimAttributes.AvatarUrl),
                    Realname = Current.User.FindFirstValue(ClaimAttributes.Realname),
                    LoginUsername = Current.User.FindFirstValue(ClaimAttributes.LoginUsername),
                    Telphone = Current.User.FindFirstValue(ClaimAttributes.Telphone),
                    UserNo = Current.User.FindFirstValue(ClaimAttributes.UserNo),
                    Sex = Convert.ToByte( Current.User.FindFirstValue(ClaimAttributes.Sex)),
                    State = Convert.ToByte(Current.User.FindFirstValue(ClaimAttributes.State)),
                    IsAdmin = Convert.ToByte(Current.User.FindFirstValue(ClaimAttributes.IsAdmin)),
                    SysType = Current.User.FindFirstValue(ClaimAttributes.SysType),
                    BelongInfoId = Current.User.FindFirstValue(ClaimAttributes.BelongInfoId),
                    CreatedAt = Convert.ToDateTime(Current.User.FindFirstValue(ClaimAttributes.CreatedAt)),
                    UpdatedAt = Convert.ToDateTime(Current.User.FindFirstValue(ClaimAttributes.UpdatedAt)),
                    CacheKey = Current.User.FindFirstValue(ClaimAttributes.CacheKey)
                };
                return user;
            }
        }

        /// <summary>
        /// 是否已授权
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return Current.User.Identity.IsAuthenticated;
            }
        }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                return (Convert.ToSByte(Current.User.FindFirstValue("isAdmin")) == CS.YES);
            }
        }
    }
}
