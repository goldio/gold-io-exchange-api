using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class CreateUpdateApiKeyRequest
    {
        public bool AccountPermissions { get; set; }
        public bool OrdersPermissions { get; set; }
        public bool FundsPermissions { get; set; }
    }
}
