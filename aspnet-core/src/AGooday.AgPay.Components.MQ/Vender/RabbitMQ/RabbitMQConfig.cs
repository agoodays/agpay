﻿namespace AGooday.AgPay.Components.MQ.Vender.RabbitMQ
{
    public class RabbitMQConfig
    {
        /// <summary>
        /// 全局定义延迟交换机名称
        /// </summary>
        public static readonly string DELAYED_EXCHANGE_NAME = "delayedExchange";

        /// <summary>
        /// 扇形交换机前缀（activeMQ中的topic模式）， 需根据queue动态拼接
        /// </summary>
        public static readonly string FANOUT_EXCHANGE_NAME_PREFIX = "fanout_exchange_";

        public static MQConfig MQ { get; set; } = new MQConfig();

        public class MQConfig
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public int Port { get; set; }
        }
    }
}
