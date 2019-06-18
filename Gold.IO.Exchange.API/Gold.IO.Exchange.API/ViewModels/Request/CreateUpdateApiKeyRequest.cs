using Gold.IO.Exchange.API.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels.Request
{
    public class CreateUpdateApiKeyRequest
    {
        public ApiKeyRole Role { get; set; }
    }
}
