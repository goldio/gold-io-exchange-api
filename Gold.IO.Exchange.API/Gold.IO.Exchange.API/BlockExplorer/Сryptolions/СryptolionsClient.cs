using Gold.IO.Exchange.API.BlockExplorer.Сryptolions.Models;
using Gold.IO.Exchange.API.BlockExplorer.Сryptolions.Models.Response;
using Gold.IO.Exchange.API.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.BlockExplorer.Сryptolions
{
    public class СryptolionsClient : IDisposable
    {
        private const string BaseUrl = @"https://history.cryptolions.io";
        //private const string Account = "eosiaaaaaaaa";
        private const string Account = "goldioioioio";

        public СryptolionsClient() { }

        public async Task<List<GlobalAction>> GetActions(int limit = 1000)
        {
            using (var client = new HttpClient())
            {
                IDictionary<string, string> properties = new Dictionary<string, string>
                {
                    { "limit", limit.ToString() }
                };

                var builder = new UriBuilder(BaseUrl)
                {
                    Path = $"/v1/history/get_actions/{Account}",
                    Query = properties.ToQueryString()
                };

                var response = await client.GetAsync(builder.Uri);
                var content = await response.Content.ReadAsStringAsync();

                var res = JsonConvert.DeserializeObject<GetActionsResponse>(content);
                var result = res.Actions.Where(x => x.Action.Name.Equals("transfer") && x.Action.Data.To.Equals(Account));

                return result.ToList();
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
        // ~СryptolionsClient() {
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
