using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class PriceResponse : ResponseModel
    {
        public double Price { get; set; }
        public bool IsHigher { get; set; }
    }
}
