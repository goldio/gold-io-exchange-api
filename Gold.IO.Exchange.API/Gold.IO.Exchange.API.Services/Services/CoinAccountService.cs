using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Coin;
using Gold.IO.Exchange.API.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class CoinAccountService : BaseCrudService<CoinAccount>, ICoinAccountService
    {
        public CoinAccountService(IRepository<CoinAccount> repository) : base(repository)
        {
        }
    }
}
