﻿using Gold.IO.Exchange.API.Domain.Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.ViewModels
{
    public class CoinAddressViewModel
    {
        public long ID { get; set; }
        public UserWalletViewModel Wallet { get; set; }
        public string PublicAddress { get; set; }
        public CoinAddressType Type { get; set; }
        public bool IsUsing { get; set; }
        
        public CoinAddressViewModel() { }

        public CoinAddressViewModel(CoinAddress address)
        {
            ID = address.ID;
            Wallet = new UserWalletViewModel(address.Wallet);
            PublicAddress = address.PublicAddress;
            Type = address.Type;
            IsUsing = address.IsUsing;
        }
    }
}
