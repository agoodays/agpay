﻿using AGooday.AgPay.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    /// <summary>
    /// 首页统计类
    /// </summary>
    [Route("api/mainChart")]
    [ApiController]
    public class MainChartController : ControllerBase
    {
        /// <summary>
        /// 周交易总金额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payAmountWeek")]
        public ApiRes PayAmountWeek()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 商户总数量、服务商总数量、总交易金额、总交易笔数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("numCount")]
        public ApiRes NumCount()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        public ApiRes PayCount()
        {
            return ApiRes.Ok();
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        public ApiRes PayWayCount()
        {
            return ApiRes.Ok();
        }
    }
}
