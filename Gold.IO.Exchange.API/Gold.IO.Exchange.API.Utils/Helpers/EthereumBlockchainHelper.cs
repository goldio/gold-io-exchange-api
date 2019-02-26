using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Nethereum.Util;
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
        public static HexBigInteger LastMaxBlockNumber = new HexBigInteger(0);

        public static EthECKey GetECKey()
        {
            return EthECKey.GenerateKey();
        }

        public static string GetAddress()
        {
            var initaddr = new Sha3Keccack().CalculateHash(GetECKey().GetPubKeyNoPrefix());
            var addr = new byte[initaddr.Length - 12];
            Array.Copy(initaddr, 12, addr, 0, initaddr.Length - 12);
            return new AddressUtil().ConvertToChecksumAddress(addr.ToHex());
        }
    }
}
