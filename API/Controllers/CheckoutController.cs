using API.Interfaces;
using API.Requests;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IPaypalService paypalService;

        public CheckoutController(IPaypalService paypalService)
        {
            this.paypalService = paypalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            //return Ok("Yeeih");
            var response = await paypalService.CreateOrder(createOrderRequest);

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            return Ok(await paypalService.CaptureOrder(orderId));
        }

        [HttpPost]
        public async Task<IActionResult> AuthorizeOrder(string orderId)
        {
            return Ok(await paypalService.AuthorizeOrder(orderId));
        }

        [HttpPost]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            return Ok(await paypalService.GetOrder(orderId));
        }

        [HttpPost]
        public async Task<IActionResult> GetToken()
        {
            return Ok(await paypalService.GetToken());
        }

        [HttpPost]
        public async Task<IActionResult> GetRefreshToken(string code)
        {
            return Ok(await paypalService.GetRefreshToken(code));
        }

    }
}
