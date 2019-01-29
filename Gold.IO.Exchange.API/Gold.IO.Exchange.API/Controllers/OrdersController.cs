using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.ViewModels;
using Gold.IO.Exchange.API.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gold.IO.Exchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private IOrderService OrderService { get; set; }

        public OrdersController([FromServices]
            IOrderService orderService)
        {
            OrderService = orderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var orders = OrderService.GetAll()
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(long id)
        {
            var order = OrderService.Get(id);
            if (order == null)
                return Json(new ResponseModel { Success = false, Message = "Order not found" });

            return Json(new DataResponse<OrderViewModel> { Data = new OrderViewModel(order) });
        }

        [HttpGet("symbol/{baseAsset}/{quoteAsset}")]
        public async Task<IActionResult> GetOrdersBySymbol(string baseAsset, string quoteAsset)
        {
            var orders = OrderService.GetAll()
                .Where(x => x.BaseAsset.ShortName.Equals(baseAsset) &&
                    x.QuoteAsset.ShortName.Equals(quoteAsset))
                .Select(x => new OrderViewModel(x))
                .ToList();

            return Json(new DataResponse<List<OrderViewModel>> { Data = orders });
        }
    }
}