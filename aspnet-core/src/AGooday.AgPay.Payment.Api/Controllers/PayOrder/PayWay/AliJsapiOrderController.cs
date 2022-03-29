using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Controllers.PayOrder.PayWay
{
    [ApiController]
    public class AliJsapiOrderController : AbstractPayOrderController
    {
        /// <summary>
        /// 统一下单接口
        /// </summary>
        [Route("api/pay/aliJsapiOrder")]
        public IActionResult aliJsapiOrder()
        {
            // 统一下单接口
            return UnifiedOrder("", null);
        }
    }
}
