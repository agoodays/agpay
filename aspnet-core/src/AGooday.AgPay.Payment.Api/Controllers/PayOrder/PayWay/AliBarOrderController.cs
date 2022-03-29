using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Controllers.PayOrder.PayWay
{
    [ApiController]
    public class AliBarOrderController : AbstractPayOrderController
    {
        [Route("api/pay/aliBarOrder")]
        public IActionResult aliBarOrder()
        {
            // 统一下单接口
            return UnifiedOrder("", null);
        }
    }
}
