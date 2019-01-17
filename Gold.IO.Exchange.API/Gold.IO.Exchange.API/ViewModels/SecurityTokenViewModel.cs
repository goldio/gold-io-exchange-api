using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class SecurityTokenViewModel
    {
        public string Token { get; set; }
        public UserRole Role { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
