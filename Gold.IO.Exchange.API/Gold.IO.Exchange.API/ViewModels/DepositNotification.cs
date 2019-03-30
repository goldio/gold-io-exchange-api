using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class DepositNotification
    {
        public string Address { get; set; }
        public decimal Amount { get; set; }
    }
}
