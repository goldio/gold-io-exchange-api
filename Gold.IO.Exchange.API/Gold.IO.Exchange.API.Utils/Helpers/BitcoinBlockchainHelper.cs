using NBitcoin;
using NBXplorer;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class BitcoinBlockchainHelper
    {
        public static string GetAddress()
        {
            Key privateKey = new Key();
            PubKey publicKey = privateKey.PubKey;

            return publicKey.GetAddress(Network.Main).ToString();
        }
    }
}
