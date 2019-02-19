using Gold.IO.Exchange.API.BlockExplorer.Blockcypher.Models;
using Gold.IO.Exchange.API.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Blockcypher
{
    public class BlockcypherClient : IDisposable
    {
        private const string BaseUrl = @"https://api.blockcypher.com";

        public async Task<BlockcypherTransactionStatus> CheckTx(string txid)
        {
            var http = new HttpClient();
            IDictionary<string, string> properties = new Dictionary<string, string>
            {
                { "module", "transaction" },
                { "action", "gettxreceiptstatus" },
                { "txhash", txid },
                { "apikey", "G9UNKDA9MXDNV4UAB22FDIG5HA47RWNW3J" }
            };

            var builder = new UriBuilder(BaseUrl)
            {
                Path = $"/api/",
                Query = properties.ToQueryString()
            };

            var response = await http.GetAsync(builder.Uri);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BlockcypherTransactionStatusResponse>(content);

                return new BlockcypherTransactionStatus { Status = result.Status };
            }

            return new BlockcypherTransactionStatus();
        }

        public async Task<BlockcypherResponse<BlockcypherTransaction>> CheckAddress(string address)
        {
            var http = new HttpClient();
            //IDictionary<string, string> properties = new Dictionary<string, string>
            //{
            //    { "module", "account" },
            //    { "action", "txlist" },
            //    { "address", address },
            //    { "startblock", "0" },
            //    { "endblock", "99999999" },
            //    { "sort", "desc" },
            //    { "apikey", "G9UNKDA9MXDNV4UAB22FDIG5HA47RWNW3J" }
            //};

            var builder = new UriBuilder(BaseUrl)
            {
                Path = $"/v1/eth/main/addrs/{address}",
                //Query = properties.ToQueryString()
            };

            var response = await http.GetAsync(builder.Uri);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<BlockcypherResponse<BlockcypherTransaction>>(content);

            return result;
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
        // ~BlockcypherClient() {
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
