using Info.Blockchain.API.Client;
using Info.Blockchain.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockchain
{
    public class BlockchainClient : IDisposable
    {
        private readonly string API_KEY = "d77125c4-78cd-4e36-9b92-8d7aad5852b1";

        public async Task<Address> GetAddress(string walletAddress)
        {
            using (var client = new BlockchainApiHelper(API_KEY))
            {
                var address = await client.blockExplorer.GetBase58AddressAsync(walletAddress);
                return address;
            }
        }

        public async Task<Transaction> GetTransaction(string txHash)
        {
            using (var client = new BlockchainApiHelper(API_KEY))
            {
                var tx = await client.blockExplorer.GetTransactionByHashAsync(txHash);
                return tx;
            }
        }

        public async Task<long> GetBlockCount()
        {
            using (var client = new BlockchainApiHelper(API_KEY))
            {
                var latestBlock = await client.blockExplorer.GetLatestBlockAsync();
                return latestBlock.Height;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~BlockchainClient() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
