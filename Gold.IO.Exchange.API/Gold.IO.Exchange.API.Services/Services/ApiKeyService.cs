using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.User;
using Gold.IO.Exchange.API.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class ApiKeyService : BaseCrudService<ApiKey>, IApiKeyService
    {
        public ApiKeyService(IRepository<ApiKey> repository) : base(repository)
        {
        }
    }
}
