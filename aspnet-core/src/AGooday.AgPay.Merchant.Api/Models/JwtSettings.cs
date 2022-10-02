﻿namespace AGooday.AgPay.Merchant.Api.Models
{
    public class JwtSettings
    {
        /// <summary>
        /// 应用密钥(真实项目中可能区分应用,不同的应用对应惟一的密钥)
        /// </summary>
        public string Secret { get; set; }
    }
}
