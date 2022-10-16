﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Components.MQ.Vender;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Payment.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Common.Enumerator;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 统一下单
    /// </summary>
    [ApiController]
    public class UnifiedOrderController : AbstractPayOrderController
    {
        private readonly IPayWayService payWayService;
        private readonly ConfigContextQueryService configContextQueryService;

        public UnifiedOrderController(IMQSender mqSender,
            Func<string, IPaymentService> paymentServiceFactory,
            ConfigContextQueryService configContextQueryService,
            PayOrderProcessService payOrderProcessService,
            RequestIpUtil requestIpUtil,
            ILogger<UnifiedOrderController> logger,
            IMchPayPassageService mchPayPassageService,
            IPayOrderService payOrderService,
            ISysConfigService sysConfigService,
            IPayWayService payWayService)
            : base(mqSender, paymentServiceFactory, configContextQueryService, payOrderProcessService, requestIpUtil, logger, mchPayPassageService, payOrderService, sysConfigService)
        {
            this.payWayService = payWayService;
            this.configContextQueryService = configContextQueryService;
        }

        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/unifiedOrder")]
        public ActionResult<ApiRes> UnifiedOrder()
        {
            //获取参数 & 验签
            UnifiedOrderRQ rq = GetRQByWithMchSign<UnifiedOrderRQ>();

            UnifiedOrderRQ bizRQ = BuildBizRQ(rq);

            //实现子类的res
            ApiRes apiRes = UnifiedOrder(bizRQ.WayCode, bizRQ);
            if (apiRes.Data == null)
            {
                return apiRes;
            }

            UnifiedOrderRS bizRes = (UnifiedOrderRS)apiRes.Data;

            //聚合接口，返回的参数
            UnifiedOrderRS res = new UnifiedOrderRS();
            CopyUtil.CopyProperties(bizRes, res);

            //只有 订单生成（QR_CASHIER） || 支付中 || 支付成功返回该数据
            if (bizRes.OrderState != null && (bizRes.OrderState == (byte)PayOrderState.STATE_INIT || bizRes.OrderState == (byte)PayOrderState.STATE_ING || bizRes.OrderState == (byte)PayOrderState.STATE_SUCCESS))
            {
                res.PayDataType = bizRes.BuildPayDataType();
                res.PayData = bizRes.BuildPayData();
            }

            return ApiRes.OkWithSign(res, configContextQueryService.QueryMchApp(rq.MchNo, rq.AppId).AppSecret);
        }
        private UnifiedOrderRQ BuildBizRQ(UnifiedOrderRQ rq)
        {

            //支付方式  比如： ali_bar
            string wayCode = rq.WayCode;

            //jsapi 收银台聚合支付场景 (不校验是否存在payWayCode)
            if (CS.PAY_WAY_CODE.QR_CASHIER.Equals(wayCode))
            {
                return rq.BuildBizRQ();
            }

            //如果是自动分类条码
            if (CS.PAY_WAY_CODE.AUTO_BAR.Equals(wayCode))
            {
                AutoBarOrderRQ bizRQ = (AutoBarOrderRQ)rq.BuildBizRQ();
                wayCode = AgPayUtil.GetPayWayCodeByBarCode(bizRQ.AuthCode);
                rq.WayCode = wayCode.Trim();
            }

            if (!payWayService.IsExistPayWayCode(wayCode))
            {
                throw new BizException("不支持的支付方式");
            }

            //转换为 bizRQ
            return rq.BuildBizRQ();
        }
    }
}
