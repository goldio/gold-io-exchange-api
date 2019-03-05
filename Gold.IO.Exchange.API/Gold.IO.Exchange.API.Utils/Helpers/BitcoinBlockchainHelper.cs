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
        public static string GetAddress()
        {
            var secret = new BitcoinSecret("L2vkR3XsDpPK9xJJ5yy6Wt8957N67RnEhkcXZUpen15pz6LH1LZp");
            return secret.PubKey.GetAddress(Network.Main).ToString();
        }

        public static string GeneratePrivateKey()
        {
            return new Key().GetWif(Network.Main).ToWif();
        }
    }
}
