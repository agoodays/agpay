﻿namespace AGooday.AgPay.Application.DataTransfer
{
    /// <summary>
    /// 支付订单表
    /// </summary>
    public class PayOrderDto
    {
        /// <summary>
        /// 支付订单号
        /// </summary>
        public string PayOrderId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string MchNo { get; set; }

        /// <summary>
        /// 服务商号
        /// </summary>
        public string IsvNo { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MchName { get; set; }

        /// <summary>
        /// 类型: 1-普通商户, 2-特约商户(服务商模式)
        /// </summary>
        public byte MchType { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string MchOrderNo { get; set; }

        /// <summary>
        /// 支付接口代码
        /// </summary>
        public string IfCode { get; set; }

        /// <summary>
        /// 支付方式代码
        /// </summary>
        public string WayCode { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string WayName { get; set; }

        /// <summary>
        /// 支付金额,单位分
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// 商户手续费费率快照
        /// </summary>
        public decimal MchFeeRate { get; set; }

        /// <summary>
        /// 商户手续费,单位分
        /// </summary>
        public long MchFeeAmount { get; set; }

        /// <summary>
        /// 三位货币代码,人民币:cny
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 支付状态: 0-订单生成, 1-支付中, 2-支付成功, 3-支付失败, 4-已撤销, 5-已退款, 6-订单关闭
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        /// 向下游回调状态, 0-未发送,  1-已发送
        /// </summary>
        public byte NotifyState { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 商品描述信息
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 特定渠道发起额外参数
        /// </summary>
        public string ChannelExtra { get; set; }

        /// <summary>
        /// 渠道用户标识,如微信openId,支付宝账号
        /// </summary>
        public string ChannelUser { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderNo { get; set; }

        /// <summary>
        /// 退款状态: 0-未发生实际退款, 1-部分退款, 2-全额退款
        /// </summary>
        public byte RefundState { get; set; }

        /// <summary>
        /// 退款次数
        /// </summary>
        public int RefundTimes { get; set; }

        /// <summary>
        /// 退款总金额,单位分
        /// </summary>
        public long RefundAmount { get; set; }

        /// <summary>
        /// 订单分账模式：0-该笔订单不允许分账, 1-支付成功按配置自动完成分账, 2-商户手动分账(解冻商户金额)
        /// </summary>
        public byte? DivisionMode { get; set; }

        /// <summary>
        /// 0-未发生分账, 1-等待分账任务处理, 2-分账处理中, 3-分账任务已结束(不体现状态)
        /// </summary>
        public byte? DivisionState { get; set; }

        /// <summary>
        /// 最新分账时间
        /// </summary>
        public DateTime? DivisionLastTime { get; set; }

        /// <summary>
        /// 渠道支付错误码
        /// </summary>
        public string ErrCode { get; set; }

        /// <summary>
        /// 渠道支付错误描述
        /// </summary>
        public string ErrMsg { get; set; }

        /// <summary>
        /// 商户扩展参数
        /// </summary>
        public string ExtParam { get; set; }

        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string NotifyUrl { get; set; }

        /// <summary>
        /// 页面跳转地址
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 订单失效时间
        /// </summary>
        public DateTime? ExpiredTime { get; set; }

        /// <summary>
        /// 订单支付成功时间
        /// </summary>
        public DateTime? SuccessTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
