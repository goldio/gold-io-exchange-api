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

        public static string SendTransaction(string key, string addressTo, double amount)
        {
            var secret = new BitcoinSecret(key);
            var to = BitcoinAddress.Create(addressTo);
            var txBuilder = new TransactionBuilder();
            var tx = txBuilder
                .AddKeys(secret)
                .Send(to, amount.ToString())
                .SendFees("0.00005")
                .SetChange(secret.GetAddress())
                .BuildTransaction(true);

            if (!txBuilder.Verify(tx))
                return null;

            var txRepo = new NoSqlTransactionRepository();
            txRepo.Put(tx);

            return tx.GetHash().ToString();
        }
    }
}
