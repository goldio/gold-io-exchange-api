using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class BitcoinService : IBitcoinService
    {
        public string GetDepositAddress()
        {
            return new Key().PubKey.Hash.GetAddress(Network.Main).ToString();
        }
    }
}
