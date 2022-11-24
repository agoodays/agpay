﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Payment.Api.Controllers.Refund;
using AGooday.AgPay.Payment.Api.RQRS.Transfer;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Payment.Api.Controllers.Transfer
{
    /// <summary>
    /// 商户转账单查询
    /// </summary>
    [ApiController]
    public class QueryTransferOrderController : ApiControllerBase
    {
        protected readonly ILogger<QueryRefundOrderController> _logger;
        private readonly ITransferOrderService _transferOrderService;
        private readonly ConfigContextQueryService _configContextQueryService;

        public QueryTransferOrderController(ILogger<QueryRefundOrderController> logger,
            ITransferOrderService transferOrderService, 
            ConfigContextQueryService configContextQueryService,
            RequestIpUtil requestIpUtil)
            : base(requestIpUtil, configContextQueryService)
        {
            _logger = logger;
            _transferOrderService = transferOrderService;
            _configContextQueryService = configContextQueryService;
        }

        [HttpPost, Route("/api/transfer/query")]
        public ApiRes QueryTransferOrder()
        {
            //获取参数 & 验签
            QueryTransferOrderRQ rq = GetRQByWithMchSign<QueryTransferOrderRQ>();

            if (string.IsNullOrWhiteSpace(rq.MchOrderNo) && string.IsNullOrWhiteSpace(rq.TransferId))
            {
                throw new BizException("mchOrderNo 和 transferId不能同时为空");
            }

            TransferOrderDto tansferOrder = _transferOrderService.QueryMchOrder(rq.MchNo, rq.MchOrderNo, rq.TransferId);
            if (tansferOrder == null)
            {
                throw new BizException("订单不存在");
            }

            QueryTransferOrderRS bizRes = QueryTransferOrderRS.BuildByRecord(tansferOrder);
            return ApiRes.OkWithSign(bizRes, _configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
    }
}
