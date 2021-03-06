﻿using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Coin;
using Gold.IO.Exchange.API.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class CoinAddressService : BaseCrudService<CoinAddress>, ICoinAddressService
    {
        public CoinAddressService(IRepository<CoinAddress> repository) : base(repository)
        {
        }
    }
}
