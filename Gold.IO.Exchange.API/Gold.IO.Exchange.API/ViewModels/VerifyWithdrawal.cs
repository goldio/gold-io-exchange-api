using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class VerifyWithdrawal
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }
}
