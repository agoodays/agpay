﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Common.Exceptions;
using AGooday.AgPay.Domain.Communication;
using AGooday.AgPay.Payment.Api.Channel;
using AGooday.AgPay.Payment.Api.Models;
using AGooday.AgPay.Payment.Api.RQRS.Msg;
using AGooday.AgPay.Payment.Api.Services;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Net;

namespace AGooday.AgPay.Payment.Api.Controllers.PayOrder
{
    /// <summary>
    /// 渠道侧的通知入口Controller 【分为同步跳转（doReturn）和异步回调(doNotify) 】
    /// </summary>
    [ApiController]
    [Route("api/pay")]
    public class ChannelNoticeController : Controller
    {
        private readonly ILogger<AbstractPayOrderController> log;
        private readonly IPayOrderService payOrderService;
        protected readonly Func<string, IChannelNoticeService> _channelNoticeServiceFactory;
        private readonly ConfigContextQueryService configContextQueryService;
        private readonly PayMchNotifyService payMchNotifyService;
        private readonly PayOrderProcessService payOrderProcessService;

        public ChannelNoticeController(ILogger<AbstractPayOrderController> logger,
            IPayOrderService payOrderService,
            ConfigContextQueryService configContextQueryService,
            PayMchNotifyService payMchNotifyService,
            PayOrderProcessService payOrderProcessService)
        {
            this.log = logger;
            this.payOrderService = payOrderService;
            this.configContextQueryService = configContextQueryService;
            this.payMchNotifyService = payMchNotifyService;
            this.payOrderProcessService = payOrderProcessService;
        }

        /// <summary>
        /// 同步通知入口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/return/{ifCode}")]
        [Route("/api/pay/return/{ifCode}/{payOrderId}")]
        public ActionResult DoReturn(string ifCode, string payOrderId)
        {
            string urlOrderId = payOrderId;
            string logPrefix = $"进入[{ifCode}]支付同步跳转：urlOrderId：[{payOrderId}] ";
            log.LogInformation($"===== {logPrefix} =====");

            try
            {
                if (string.IsNullOrWhiteSpace(ifCode))
                {
                    return this.ToReturnPage("ifCode is empty");
                }
                //查询支付接口是否存在
                IChannelNoticeService payNotifyService = _channelNoticeServiceFactory(ifCode);

                // 支付通道接口实现不存在
                if (payNotifyService == null)
                {
                    log.LogError($"{logPrefix}, interface not exists ");
                    return this.ToReturnPage("[" + ifCode + "] interface not exists");
                }

                // 解析订单号 和 请求参数
                Dictionary<string, object> mutablePair = payNotifyService.ParseParams(Request, urlOrderId, IChannelNoticeService.NoticeTypeEnum.DO_RETURN);
                if (mutablePair == null)
                { // 解析数据失败， 响应已处理
                    log.LogError($"{logPrefix}, mutablePair is null ", logPrefix);
                    throw new BizException("解析数据异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                //解析到订单号
                payOrderId = mutablePair.First().Key;
                log.LogInformation($"{logPrefix}, 解析数据为：payOrderId:{payOrderId}, params:{mutablePair.First().Value}");

                if (!string.IsNullOrWhiteSpace(urlOrderId) && !urlOrderId.Equals(payOrderId))
                {
                    log.LogError($"{logPrefix}, 订单号不匹配. urlOrderId={urlOrderId}, payOrderId={payOrderId} ");
                    throw new BizException("订单号不匹配！");
                }

                //获取订单号 和 订单数据
                PayOrderDto payOrder = payOrderService.GetById(payOrderId);

                // 订单不存在
                if (payOrder == null)
                {
                    log.LogError($"{logPrefix}, 订单不存在. payOrderId={payOrderId} ");
                    return this.ToReturnPage("支付订单不存在");
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);

                //调起接口的回调判断
                ChannelRetMsg notifyResult = payNotifyService.DoNotice(Request, mutablePair.First().Value, payOrder, mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum.DO_RETURN);

                // 返回null 表明出现异常， 无需处理通知下游等操作。
                if (notifyResult == null || notifyResult.ChannelState == null || notifyResult.ResponseEntity == null)
                {
                    log.LogError($"{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult ={notifyResult} ");
                    throw new BizException("处理回调事件异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                //判断订单状态
                if (notifyResult.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    payOrder.State = (byte)PayOrderState.STATE_SUCCESS;
                }
                else if (notifyResult.ChannelState == ChannelState.CONFIRM_FAIL)
                {
                    payOrder.State = (byte)PayOrderState.STATE_FAIL;
                }

                bool hasReturnUrl = string.IsNullOrWhiteSpace(payOrder.ReturnUrl);
                log.LogInformation($"===== {logPrefix}, 订单通知完成。 payOrderId={payOrderId}, parseState = {notifyResult.ChannelState}, hasReturnUrl={hasReturnUrl} =====");

                //包含通知地址时
                if (hasReturnUrl)
                {
                    // 重定向
                    return Redirect(payMchNotifyService.CreateReturnUrl(payOrder, mchAppConfigContext.MchApp.AppSecret));
                }
                else
                {
                    //跳转到支付成功页面
                    return this.ToReturnPage(null);
                }
            }
            catch (BizException e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, BizException");
                return this.ToReturnPage(e.Message);
            }
            catch (WebException e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, ResponseException");
                return this.ToReturnPage(e.Message);

            }
            catch (Exception e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, 系统异常");
                return this.ToReturnPage(e.Message);
            }
        }

        /// <summary>
        /// 异步回调入口
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/pay/notify/{ifCode}")]
        [Route("/api/pay/notify/{ifCode}/{payOrderId}")]
        public ActionResult DoNotify(string ifCode, string payOrderId)
        {
            string urlOrderId = payOrderId;
            string logPrefix = $"进入[{ifCode}]支付回调：urlOrderId：[{payOrderId}] ";
            log.LogInformation($"===== {logPrefix} =====", logPrefix);

            try
            {
                if (string.IsNullOrWhiteSpace(ifCode))
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "ifCode is empty");
                }
                //查询支付接口是否存在
                IChannelNoticeService payNotifyService = _channelNoticeServiceFactory(ifCode);

                // 支付通道接口实现不存在
                if (payNotifyService == null)
                {
                    log.LogError($"{logPrefix}, interface not exists ");
                    return BadRequest($"[{ifCode}] interface not exists");
                }

                // 解析订单号 和 请求参数
                Dictionary<string, object> mutablePair = payNotifyService.ParseParams(Request, urlOrderId, IChannelNoticeService.NoticeTypeEnum.DO_RETURN);
                if (mutablePair == null)
                { // 解析数据失败， 响应已处理
                    log.LogError($"{logPrefix}, mutablePair is null ", logPrefix);
                    throw new BizException("解析数据异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                //解析到订单号
                payOrderId = mutablePair.First().Key;
                log.LogInformation($"{logPrefix}, 解析数据为：payOrderId:{payOrderId}, params:{mutablePair.First().Value}");

                if (!string.IsNullOrWhiteSpace(urlOrderId) && !urlOrderId.Equals(payOrderId))
                {
                    log.LogError($"{logPrefix}, 订单号不匹配. urlOrderId={urlOrderId}, payOrderId={payOrderId} ");
                    throw new BizException("订单号不匹配！");
                }

                //获取订单号 和 订单数据
                PayOrderDto payOrder = payOrderService.GetById(payOrderId);

                // 订单不存在
                if (payOrder == null)
                {
                    log.LogError($"{logPrefix}, 订单不存在. payOrderId={payOrderId} ");
                    payNotifyService.DoNotifyOrderNotExists(Request);
                    throw new BizException("订单不存在！");
                }

                //查询出商户应用的配置信息
                MchAppConfigContext mchAppConfigContext = configContextQueryService.QueryMchInfoAndAppInfo(payOrder.MchNo, payOrder.AppId);

                //调起接口的回调判断
                ChannelRetMsg notifyResult = payNotifyService.DoNotice(Request, mutablePair.First().Value, payOrder, mchAppConfigContext, IChannelNoticeService.NoticeTypeEnum.DO_NOTIFY);

                // 返回null 表明出现异常， 无需处理通知下游等操作。
                if (notifyResult == null || notifyResult.ChannelState == null || notifyResult.ResponseEntity == null)
                {
                    log.LogError($"{logPrefix}, 处理回调事件异常  notifyResult data error, notifyResult ={notifyResult} ");
                    throw new BizException("处理回调事件异常！"); //需要实现类自行抛出ResponseException, 不应该在这抛此异常。
                }

                bool updateOrderSuccess = true; //默认更新成功
                // 订单是 【支付中状态】
                if (payOrder.State == (byte)PayOrderState.STATE_ING)
                {
                    //明确成功
                    if (ChannelState.CONFIRM_SUCCESS == notifyResult.ChannelState)
                    {
                        updateOrderSuccess = payOrderService.UpdateIng2Success(payOrderId, notifyResult.ChannelOrderId, notifyResult.ChannelUserId);
                    }
                    //明确失败
                    else if (ChannelState.CONFIRM_FAIL == notifyResult.ChannelState)
                    {
                        updateOrderSuccess = payOrderService.UpdateIng2Fail(payOrderId, notifyResult.ChannelOrderId, notifyResult.ChannelUserId, notifyResult.ChannelErrCode, notifyResult.ChannelErrMsg);
                    }
                }

                // 更新订单 异常
                if (!updateOrderSuccess)
                {
                    log.LogError($"{logPrefix}, updateOrderSuccess = {updateOrderSuccess} ");
                    return payNotifyService.DoNotifyOrderStateUpdateFail(Request);
                }

                //订单支付成功 其他业务逻辑
                if (notifyResult.ChannelState == ChannelState.CONFIRM_SUCCESS)
                {
                    payOrderProcessService.ConfirmSuccess(payOrder);
                }

                log.LogInformation($"===== {logPrefix}, 订单通知完成。 payOrderId={payOrderId}, parseState = {notifyResult.ChannelState} =====");

                //return StatusCode((int)HttpStatusCode.BadRequest, new
                //{
                //    message = "id must larger than zero."
                //});
                //return Content("id must larger than zero.", "text/html");
                return notifyResult.ResponseEntity;
            }
            catch (BizException e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, BizException");
                return BadRequest(e.Message);
            }
            catch (WebException e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, ResponseException");
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                log.LogError(e, $"{logPrefix}, payOrderId={payOrderId}, 系统异常");
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 跳转到支付成功页面
        /// </summary>
        /// <param name="errInfo"></param>
        /// <returns></returns>
        private ActionResult ToReturnPage(string errInfo)
        {
            return View("~/Views/Cashier/ReturnPage.cshtml");
        }
    }
}
