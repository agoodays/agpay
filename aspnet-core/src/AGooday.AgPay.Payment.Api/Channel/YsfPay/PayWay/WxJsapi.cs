﻿
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder;
using AGooday.AgPay.Payment.Api.RQRS;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.RQRS.PayOrder.PayWay;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Utils;
using Aop.Api.Request;
using Aop.Api.Domain;
using Aop.Api.Response;
using AGooday.AgPay.Payment.Api.Services;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Aop.Api.Util;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;

namespace AGooday.AgPay.Payment.Api.Channel.YsfPay.PayWay
{
    public class WxJsapi : YsfPayPaymentService
    {
        /// <summary>
        /// 云闪付 微信jsapi
        /// </summary>
        /// <param name="serviceProvider"></param>
        public WxJsapi(IServiceProvider serviceProvider,
            ISysConfigService sysConfigService,
            ConfigContextQueryService configContextQueryService)
            : base(serviceProvider, sysConfigService, configContextQueryService)
        {
        }

        public override AbstractRS Pay(UnifiedOrderRQ rq, PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            string logPrefix = "【云闪付(wechatJs)jsapi支付】";
            JObject reqParams = new JObject();
            WxJsapiOrderRS res = ApiResBuilder.BuildSuccess<WxJsapiOrderRS>();
            ChannelRetMsg channelRetMsg = new ChannelRetMsg();
            res.ChannelRetMsg = channelRetMsg;

            // 请求参数赋值
            JsapiParamsSet(reqParams, payOrder, GetNotifyUrl(), GetReturnUrl());

            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            //云闪付扫一扫支付， 需要传入buyerUserId参数
            reqParams.Add("userId", bizRQ.Openid);// openId

            //客户端IP
            reqParams.Add("customerIp", !string.IsNullOrWhiteSpace(payOrder.ClientIp) ? payOrder.ClientIp : "127.0.0.1");

            // 获取微信官方配置 的appId
            WxPayIsvParams wxpayIsvParams = (WxPayIsvParams)_configContextQueryService.QueryIsvParams(mchAppConfigContext.MchInfo.IsvNo, CS.IF_CODE.WXPAY);
            reqParams.Add("subAppId", wxpayIsvParams.AppId); //用户ID

            // 发送请求
            JObject resJSON = PackageParamAndReq("/gateway/api/pay/unifiedorder", reqParams, logPrefix, mchAppConfigContext);
            //请求 & 响应成功， 判断业务逻辑
            string respCode = resJSON.GetValue("respCode").ToString(); //应答码
            string respMsg = resJSON.GetValue("respMsg").ToString(); //应答信息
            try
            {
                //00-交易成功， 02-用户支付中 , 12-交易重复， 需要发起查询处理    其他认为失败
                if ("00".Equals(respCode))
                {
                    //付款信息
                    res.PayInfo = resJSON.GetValue("payData").ToString();
                    channelRetMsg.ChannelState = ChannelState.WAITING;
                }
                else
                {
                    channelRetMsg.ChannelState = ChannelState.CONFIRM_FAIL;
                    channelRetMsg.ChannelErrCode = respCode;
                    channelRetMsg.ChannelErrMsg = respMsg;
                }
            }
            catch (Exception e)
            {
                channelRetMsg.ChannelErrCode = respCode;
                channelRetMsg.ChannelErrMsg = respMsg;
            }
            return res;
        }

        public override string PreCheck(UnifiedOrderRQ rq, PayOrderDto payOrder)
        {
            WxJsapiOrderRQ bizRQ = (WxJsapiOrderRQ)rq;
            if (string.IsNullOrWhiteSpace(bizRQ.Openid))
            {
                throw new BizException("[openId]不可为空");
            }

            return null;
        }
    }
}
