using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.WebApi.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Order()
        {
            _orderService.SendOrderMessage();
            return Ok();
        }
        [HttpGet("test")]
        public IActionResult Test(string message)
        {
            _orderService.SendTestMessage(message);
            return Ok();
        }

    }
}
