using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class WithdrawRequest
    {
        public string Address { get; set; }
        public double Amount { get; set; }
    }
}
