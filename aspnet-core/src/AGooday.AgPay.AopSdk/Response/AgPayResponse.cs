﻿using AGooday.AgPay.AopSdk.Utils;
using Newtonsoft.Json.Linq;

namespace AGooday.AgPay.AopSdk.Response
{
    /// <summary>
    /// AgPay响应抽象类
    /// </summary>
    public abstract class AgPayResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string sign { get; set; }
        public JObject data { get; set; }
        public bool CheckSign(string apiKey)
        {
            if (data == null && string.IsNullOrWhiteSpace(sign)) return true;
            return sign.Equals(AgPayUtil.GetSign(data.ToObject<Dictionary<string, object>>(), apiKey));
        }
        public virtual bool IsSuccess(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return code == 0;
            return code == 0 && CheckSign(apiKey);
        }
    }
}
