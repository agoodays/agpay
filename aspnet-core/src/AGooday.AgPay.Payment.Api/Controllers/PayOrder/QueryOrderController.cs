using Microsoft.AspNetCore.Mvc;

namespace AGooday.AgPay.Controllers.PayOrder
{
    /// <summary>
    /// 商户查单
    /// </summary>
    [ApiController]
    public class QueryOrderController : ControllerBase
    {
        [Route("api/pay/query")]
        public IActionResult QueryOrder()
        {
            return Ok();
        }
    }
}
