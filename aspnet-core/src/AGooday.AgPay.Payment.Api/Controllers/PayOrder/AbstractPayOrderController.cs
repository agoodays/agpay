using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Controllers.PayOrder
{
    /// <summary>
    /// 创建支付订单抽象类
    /// </summary>
    public abstract class AbstractPayOrderController : ControllerBase
    {
        /// <summary>
        /// 统一下单 (新建订单模式)
        /// </summary>
        /// <param name="wayCode"></param>
        /// <param name="bizRQ"></param>
        /// <returns></returns>
        protected IActionResult UnifiedOrder(String wayCode, object? bizRQ)
        {
            return Ok();
        }
    }
}
