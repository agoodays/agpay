﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Manager.Api.Controllers
{
    /// <summary>
    /// 首页统计类
    /// </summary>
    [Route("api/mainChart")]
    [ApiController, Authorize, NoLog]
    public class MainChartController : ControllerBase
    {
        private readonly ILogger<MainChartController> _logger;
        private readonly IPayOrderService _payOrderService;

        public MainChartController(ILogger<MainChartController> logger, IPayOrderService payOrderService)
        {
            _logger = logger;
            _payOrderService = payOrderService;
        }

        /// <summary>
        /// 周交易总金额
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payAmountWeek")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_AMOUNT_WEEK)]
        public ApiRes PayAmountWeek()
        {
            return ApiRes.Ok(_payOrderService.MainPageWeekCount(null));
        }

        /// <summary>
        /// 商户总数量、服务商总数量、总交易金额、总交易笔数
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("numCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_NUMBER_COUNT)]
        public ApiRes NumCount()
        {
            return ApiRes.Ok(_payOrderService.MainPageNumCount(null));
        }

        /// <summary>
        /// 交易统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_COUNT)]
        public ApiRes PayCount(string createdStart, string createdEnd)
        {
            return ApiRes.Ok(_payOrderService.MainPagePayCount(null, createdStart, createdEnd));
        }

        /// <summary>
        /// 支付方式统计
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("payTypeCount")]
        [PermissionAuth(PermCode.MGR.ENT_C_MAIN_PAY_TYPE_COUNT)]
        public ApiRes PayWayCount(string createdStart, string createdEnd)
        {
            return ApiRes.Ok(_payOrderService.MainPagePayTypeCount(null, createdStart, createdEnd));
        }
    }
}
