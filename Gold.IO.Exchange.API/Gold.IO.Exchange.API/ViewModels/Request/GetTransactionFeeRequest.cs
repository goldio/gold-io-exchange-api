using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class GetTransactionFeeRequest
    {
        [Required]
        public string Coin { get; set; }

        [Required]
        public double Amount { get; set; }
    }
}
