using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class BitcoinBlockchainHelper
    {
        public static string GetAddress(string publicKey, uint derivations)
        {
            var pubkey = ExtPubKey.Parse(publicKey);
            return pubkey.Derive(derivations).PubKey.GetAddress(Network.Main).ToString();
        }
    }
}
