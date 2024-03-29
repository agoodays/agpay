﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Params.WxPay;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Payment.Api.Channel.AliPay;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using WechatTenpayClientV3 = SKIT.FlurlHttpClient.Wechat.TenpayV3.WechatTenpayClient;

namespace AGooday.AgPay.Payment.Api.Channel.WxPay
{
    /// <summary>
    /// 分账接口： 微信官方
    /// </summary>
    public class WxPayDivisionService : IDivisionService
    {
        private readonly ILogger<AliPayDivisionService> log;
        private readonly ConfigContextQueryService configContextQueryService;

        public WxPayDivisionService(ILogger<AliPayDivisionService> log, ConfigContextQueryService configContextQueryService)
        {
            this.log = log;
            this.configContextQueryService = configContextQueryService;
        }

        public string GetIfCode()
        {
            return CS.IF_CODE.WXPAY;
        }

        public bool IsSupport()
        {
            return false;
        }

        public ChannelRetMsg Bind(MchDivisionReceiverDto mchDivisionReceiver, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);

                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                    channelRetMsg.ChannelErrMsg = "V2暂不支持";
                    return channelRetMsg;
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    AddProfitSharingReceiverRequest request = new AddProfitSharingReceiverRequest();

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams =
                                (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    // 0-个人， 1-商户  (目前仅支持服务商appI获取个人openId, 即： PERSONAL_OPENID， 不支持 PERSONAL_SUB_OPENID )
                    request.Type = mchDivisionReceiver.AccType == 0 ? "PERSONAL_OPENID" : "MERCHANT_ID";
                    request.Account = mchDivisionReceiver.AccNo;
                    request.Name = mchDivisionReceiver.AccName;
                    request.RelationType = mchDivisionReceiver.RelationType;
                    request.CustomRelation = mchDivisionReceiver.RelationTypeName;

                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    var response = client.ExecuteAddProfitSharingReceiverAsync(request).Result;
                    if (response.IsSuccessful())
                    {
                        // 明确成功
                        return ChannelRetMsg.ConfirmSuccess(null);
                    }
                    else
                    {
                        ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                        channelRetMsg.ChannelErrCode = response.ErrorCode;
                        channelRetMsg.ChannelErrMsg = response.ErrorMessage;
                        return channelRetMsg;
                    }
                }
                else
                {
                    ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                    channelRetMsg.ChannelErrMsg = "API_VERSION ERROR";
                    return channelRetMsg;
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "请求微信绑定分账接口异常");
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }

        public ChannelRetMsg SingleDivision(PayOrderDto payOrder, List<PayOrderDivisionRecordDto> recordList, MchAppConfigContext mchAppConfigContext)
        {
            try
            {
                WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
                if (CS.PAY_IF_VERSION.WX_V2.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                    channelRetMsg.ChannelErrMsg = "V2暂不支持";
                    return channelRetMsg;
                }
                else if (CS.PAY_IF_VERSION.WX_V3.Equals(wxServiceWrapper.Config.ApiVersion))
                {
                    CreateProfitSharingOrderRequest request = new CreateProfitSharingOrderRequest();
                    request.TransactionId = payOrder.ChannelOrderNo;

                    //放置isv信息
                    //不是特约商户，无需放置此值
                    if (mchAppConfigContext.IsIsvSubMch())
                    {
                        WxPayIsvSubMchParams isvsubMchParams =
                                (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                        request.SubMerchantId = isvsubMchParams.SubMchId;
                        request.SubAppId = isvsubMchParams.SubMchAppId;
                    }

                    if (!recordList.Any())
                    {
                        request.OutOrderNumber = SeqUtil.GenDivisionBatchId(); // 随机生成一个订单号
                    }
                    else
                    {
                        request.OutOrderNumber = recordList.First().BatchOrderId; //取到批次号
                    }

                    foreach (var record in recordList)
                    {
                        if (record.CalDivisionAmount <= 0)
                        {
                            continue;
                        }
                        var receiver = new CreateProfitSharingOrderRequest.Types.Receiver();
                        receiver.Type = record.AccType == 0 ? "PERSONAL_OPENID" : "MERCHANT_ID";
                        receiver.Account = record.AccNo;
                        receiver.Amount = (int)record.CalDivisionAmount;
                        receiver.Description = $"{record.PayOrderId}分账";
                        request.ReceiverList.Add(receiver);
                    }

                    //不存在接收账号时，订单完结（解除冻结金额）
                    if (!request.ReceiverList.Any())
                    {
                        return this.DivisionFinish(payOrder, mchAppConfigContext);
                    }
                    var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
                    var response = client.ExecuteCreateProfitSharingOrderAsync(request).Result;
                    if (response.IsSuccessful())
                    {
                        // 明确成功
                        return ChannelRetMsg.ConfirmSuccess(response.OrderId);
                    }
                    else
                    {
                        ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                        channelRetMsg.ChannelErrCode = response.ErrorCode;
                        channelRetMsg.ChannelErrMsg = response.ErrorMessage;
                        return channelRetMsg;
                    }
                }
                else
                {
                    ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                    channelRetMsg.ChannelErrMsg = "API_VERSION ERROR";
                    return channelRetMsg;
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "微信分账失败");
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrMsg = e.Message;
                return channelRetMsg;
            }
        }

        private ChannelRetMsg DivisionFinish(PayOrderDto payOrder, MchAppConfigContext mchAppConfigContext)
        {
            SetProfitSharingOrderUnfrozenRequest request = new SetProfitSharingOrderUnfrozenRequest();

            //放置isv信息
            //不是特约商户，无需放置此值
            if (mchAppConfigContext.IsIsvSubMch())
            {
                WxPayIsvSubMchParams isvsubMchParams =
                        (WxPayIsvSubMchParams)configContextQueryService.QueryIsvSubMchParams(mchAppConfigContext.MchNo, mchAppConfigContext.AppId, CS.IF_CODE.WXPAY);

                request.SubMerchantId = isvsubMchParams.SubMchId;
            }

            request.TransactionId = payOrder.ChannelOrderNo;
            request.OutOrderNumber = SeqUtil.GenDivisionBatchId();
            request.Description = "完结分账";

            WxServiceWrapper wxServiceWrapper = configContextQueryService.GetWxServiceWrapper(mchAppConfigContext);
            var client = (WechatTenpayClientV3)wxServiceWrapper.Client;
            var response = client.ExecuteSetProfitSharingOrderUnfrozenAsync(request).Result;
            if (response.IsSuccessful())
            {
                // 明确成功
                return ChannelRetMsg.ConfirmSuccess(response.OrderId);
            }
            else
            {
                ChannelRetMsg channelRetMsg = ChannelRetMsg.ConfirmFail();
                channelRetMsg.ChannelErrCode = response.ErrorCode;
                channelRetMsg.ChannelErrMsg = response.ErrorMessage;
                return channelRetMsg;
            }
        }
    }
}
