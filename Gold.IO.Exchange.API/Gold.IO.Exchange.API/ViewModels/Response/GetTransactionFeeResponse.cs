using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Response
{
    public class GetTransactionFeeResponse : ResponseModel
    {
        public double Fee { get; set; }
        public double FinalAmount { get; set; }
    }
}
