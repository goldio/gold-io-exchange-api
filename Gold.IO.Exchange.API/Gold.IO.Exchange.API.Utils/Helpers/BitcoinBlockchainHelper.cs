using NBitcoin;
using NBitcoin.RPC;
using NBXplorer;
using NBXplorer.DerivationStrategy;
using NBXplorer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Gold.IO.Exchange.API.Utils.Helpers
{
    public static class BitcoinBlockchainHelper
    {
        public static string GetAddress()
        {
            using (var client = new HttpClient())
            {
                var message = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("http://188.42.174.122:5001/api/wallet/deposit")
                };

                message.Headers.Add("API_KEY", "5581292B69E456097282825F1C77B506");
                var request = client.SendAsync(message);

                request.Wait();
                var response = request.Result;

                if (!response.IsSuccessStatusCode)
                    return null;

                var addressTask = response.Content.ReadAsStringAsync();
                addressTask.Wait();

                var result = JsonConvert.DeserializeObject<DepositResponse>(addressTask.Result);
                if (!result.Success)
                    return null;

                return result.Address;
            }
        }

        public static string SendTransaction(string addressTo, decimal amount)
        {
            var request = new WithdrawalRequest
            {
                AddressTo = addressTo,
                Amount = amount
            };

            var response = SendPostAsync("http://188.42.174.122:5001/api/wallet/withdraw", request);
            response.Wait();

            if (!response.Result.IsSuccessStatusCode)
                return null;

            var responseReader = response.Result.Content.ReadAsStringAsync();
            responseReader.Wait();

            return responseReader.Result;
        }

        private static async Task<HttpResponseMessage> SendPostAsync(string url, object body)
        {
            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(body);

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };

                request.Headers.Add("API_KEY", "5581292B69E456097282825F1C77B506");
                var response = await client.SendAsync(request);

                return response;
            }
        }

        class DepositResponse
        {
            public string Address { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        class WithdrawalRequest
        {
            public string AddressTo { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
