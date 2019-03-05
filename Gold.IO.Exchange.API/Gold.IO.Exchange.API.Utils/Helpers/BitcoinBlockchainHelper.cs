using NBitcoin;
using NBitcoin.RPC;
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
        public static string GetAddress(string key)
        {
            var secret = new BitcoinSecret(key);
            return secret.PubKey.GetAddress(Network.Main).ToString();
        }

        public static string GeneratePrivateKey()
        {
            return new Key().GetWif(Network.Main).ToWif();
        }
    }
}
