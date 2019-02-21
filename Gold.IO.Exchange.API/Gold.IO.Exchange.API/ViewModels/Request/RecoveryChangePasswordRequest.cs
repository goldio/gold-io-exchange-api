using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class RecoveryChangePasswordRequest
    {
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string Key { get; set; }
    }
}
