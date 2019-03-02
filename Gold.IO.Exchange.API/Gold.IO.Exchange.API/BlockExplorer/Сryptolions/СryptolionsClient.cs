using EosSharp;
using EosSharp.Core;
using EosSharp.Core.Api.v1;
using EosSharp.Core.Providers;
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
        private const string DepositAccount = "eosiaaaaaaaa";
        private const string WithdrawAccount = "zengjianhong";

        private Eos eos = new Eos(new EosConfigurator()
        {
            HttpEndpoint = "https://mainnet.eoscannon.io",
            ChainId = "aca376f206b8fc25a6ed44dbdc66547c36c6c33e3a119ffbeaef943642f0e906",
            ExpireSeconds = 60,
            SignProvider = new DefaultSignProvider("5HpT3dWxscoCTrWE3r9H13NuupAxLH77y2CUhsfsKFKm48VXVhF")
        });

        public СryptolionsClient() { }

        public async Task<List<Models.GlobalAction>> GetActions(int limit = 1000)
        {
            using (var client = new HttpClient())
            {
                IDictionary<string, string> properties = new Dictionary<string, string>
                {
                    { "limit", limit.ToString() }
                };

                var builder = new UriBuilder(BaseUrl)
                {
                    Path = $"/v1/history/get_actions/{DepositAccount}",
                    Query = properties.ToQueryString()
                };

                var response = await client.GetAsync(builder.Uri);
                var content = await response.Content.ReadAsStringAsync();

                var res = JsonConvert.DeserializeObject<Models.Response.GetActionsResponse>(content);
                var result = res.Actions.Where(x => x.Action.Name.Equals("transfer") && x.Action.Data.To.Equals(DepositAccount));

                return result.ToList();
            }
        }

        public async Task<string> CreateWithdrawalRequest(string to, double amount, string coin)
        {
            string amountStr = $"{String.Format("{0:0.0000}", amount).Replace(",", ".")} {coin}";

            var result = await eos.CreateTransaction(new Transaction()
            {
                actions = new List<EosSharp.Core.Api.v1.Action>()
                {
                    new EosSharp.Core.Api.v1.Action()
                    {
                        account = "zengjianhong",
                        authorization = new List<PermissionLevel>()
                        {
                            new PermissionLevel() {actor = "zengjianhong", permission = "active" }
                        },
                        name = "transfer",
                        data = new Dictionary<string, string>()
                        {
                            { "from", "zengjianhong" },
                            { "to", to },
                            { "quantity", amountStr },
                            { "memo", "" }
                        }
                    }
                }
            });

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
