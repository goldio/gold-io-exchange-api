using Gold.IO.Exchange.API.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class WalletAddressViewModel
    {
        public long ID { get; set; }
        public CoinViewModel Coin { get; set; }
        public string Address { get; set; }
        public bool IsUsed { get; set; }

        public WalletAddressViewModel() { }

        public WalletAddressViewModel(WalletAddress walletAddress)
        {
            ID = walletAddress.ID;
            Coin = new CoinViewModel(walletAddress.Coin);
            Address = walletAddress.Address;
            IsUsed = walletAddress.IsUsed;
        }
    }
}
