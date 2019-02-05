using Gold.IO.Exchange.API.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class ApiKeyViewModel
    { 
        public long ID { get; set; }
        public string PublicKey { get; set; }
        public string SecretKey { get; set; }
        public bool AccountPermissions { get; set; }
        public bool OrdersPermissions { get; set; }
        public bool FundsPermissions { get; set; }

        public ApiKeyViewModel() { }

        public ApiKeyViewModel(ApiKey apiKey)
        {
            ID = apiKey.ID;
            PublicKey = apiKey.PublicKey;
            SecretKey = apiKey.SecretKey;
            AccountPermissions = apiKey.AccountPermissions;
            OrdersPermissions = apiKey.OrdersPermissions;
            FundsPermissions = apiKey.FundsPermissions;
        }
    }
}
