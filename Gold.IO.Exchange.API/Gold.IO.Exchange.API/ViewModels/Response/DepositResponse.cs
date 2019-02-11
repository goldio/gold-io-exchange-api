using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class DepositResponse : ResponseModel
    {
        public string Address { get; set; }
    }
}
