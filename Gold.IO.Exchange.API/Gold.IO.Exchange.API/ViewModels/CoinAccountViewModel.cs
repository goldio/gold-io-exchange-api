using Gold.IO.Exchange.API.Domain.Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CoinAccountViewModel
    {
        public long ID { get; set; }
        public CoinViewModel Coin { get; set; }
        public string AccountKey { get; set; }
        public long Derivations { get; set; }

        public CoinAccountViewModel() { }
        
        public CoinAccountViewModel(CoinAccount account)
        {
            ID = account.ID;
            Coin = new CoinViewModel(account.Coin);
            AccountKey = account.AccountKey;
            Derivations = account.Derivations;
        }
    }
}
