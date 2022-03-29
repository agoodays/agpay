using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Controllers.PayOrder
{
    /// <summary>
    /// 统一下单
    /// </summary>
    [ApiController]
    public class UnifiedOrderController : ControllerBase
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        /// <returns></returns>
        [Route("/api/pay/unifiedOrder")]
        public IActionResult UnifiedOrder()
        {
            return Ok();
        }
    }
}
