using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class EthereumBlockchainHelper
    {
        private static readonly string PrivateKey = "0A9A9E9DC8EB371BF1EAA48F79363866B74C7E61F05BD5C16752F1B83BF9573D";

        public static HexBigInteger LastMaxBlockNumber = new HexBigInteger(0);

        public static async Task<string> GetAddress()
        {
            return Nethereum.Signer.EthECKey.GenerateKey().GetPublicAddress();
        }
    }
}
