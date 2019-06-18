using Gold.IO.Exchange.API.Domain.Enum;
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
        public ApiKeyRole Role { get; set; }
        public DateTime Expired { get; set; }

        public ApiKeyViewModel() { }

        public ApiKeyViewModel(ApiKey apiKey)
        {
            ID = apiKey.ID;
            PublicKey = apiKey.PublicKey;
            Role = apiKey.Role;
            Expired = apiKey.Expired;
        }
    }
}
