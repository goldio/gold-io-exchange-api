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

        public static string GetECKey()
        {
            return EthECKey.GenerateKey().GetPrivateKey();
        }

        public static string GetAddress(string key)
        {
            var initaddr = new Sha3Keccack().CalculateHash(new EthECKey(key).GetPubKeyNoPrefix());
            var addr = new byte[initaddr.Length - 12];
            Array.Copy(initaddr, 12, addr, 0, initaddr.Length - 12);
            return new AddressUtil().ConvertToChecksumAddress(addr.ToHex());
        }

        public static async Task<string> SendTransaction(string key, string addressTo, double amount)
        {
            var address = GetAddress(key);
            var web3 = new Web3();
            var txCount = await web3.Eth.Transactions.GetTransactionCount.SendRequestAsync(address);
            var encoded = Web3.OfflineTransactionSigner.SignTransaction(key, addressTo, UnitConversion.Convert.ToWei(amount, UnitConversion.EthUnit.Ether), txCount.Value);

            if (!Web3.OfflineTransactionSigner.VerifyTransaction(encoded))
                return null;

            var txId = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync($"0x{encoded}");

            return txId;
        }
    }
}
