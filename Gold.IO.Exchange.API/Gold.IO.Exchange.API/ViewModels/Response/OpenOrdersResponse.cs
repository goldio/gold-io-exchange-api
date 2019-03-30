using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class OpenOrdersResponse : ResponseModel
    {
        public List<OrderViewModel> BuyOrders { get; set; }
        public List<OrderViewModel> SellOrders { get; set; }
    }
}
