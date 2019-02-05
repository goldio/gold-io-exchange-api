using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class UpdateUserNotificationsRequest
    {
        public bool EmailNews { get; set; }
        public bool EmailLogins { get; set; }
        public bool EmailCoinsRemovals { get; set; }
        public bool EmailMarketRemovals { get; set; }
    }
}
