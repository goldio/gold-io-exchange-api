using Binance.Net;
using Gold.IO.Exchange.API.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Gold.IO.Exchange.API.WebSocketManager
{
    public class NotificationsMessageHandler : WebSocketHandler
    {
        private BinanceSocketClient BinanceSocketClient;

        public List<SocketUser> socketUsers = new List<SocketUser>();

        public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            BinanceSocketClient = new BinanceSocketClient();
            BinanceSocketClient.SetApiCredentials("ryHIGtf0risXmrDLlsorJgtCCp395HGtEWdRIOETMcLJq45AbK5hFx4xDYt8p0aE", "ZOONzdSsQFG1opcll62ueeU8Vn4wInrHTyxUbY3kyk4HjNEHBLBc3Jf4FUcjxx4X");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var recieveJSON = JsonConvert.DeserializeObject<SocketJSON>(message);
                if (recieveJSON.Type.Equals(SubscribeType.Depth))
                {
                    var existSocketUser = socketUsers.FirstOrDefault(x => x.ID.Equals(socketId));
                    if (existSocketUser != null)
                    {
                        existSocketUser.Type = SubscribeType.Depth;
                        existSocketUser.Pair = recieveJSON.Message;
                    }
                    else
                    {
                        socketUsers.Add(new SocketUser {
                            ID = socketId,
                            Type = SubscribeType.Depth,
                            Pair = recieveJSON.Message
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        {
            foreach (var socket in this.WebSocketConnectionManager.GetAll())
            {
                if (socket.Value.State == WebSocketState.Open)
                {
                    var existSocketConnection = socketUsers.FirstOrDefault(x => x.ID.Equals(socket.Key));
                    if (existSocketConnection != null)
                        SendCurrencyForUser(socket.Value);
                }
            }
        }

        private async void SendCurrencyForUser(WebSocket socket)
        {
            //var settings = GetSettings();

            //if (settings.RateType == RateType.Fixed)
            //{
            //    var data = new
            //    {
            //        sell = settings.RateSell,
            //        buy = settings.RateBuy
            //    };

            //    var result = JsonConvert.SerializeObject(new SocketJSON { Type = "updateCurrency", Message = JsonConvert.SerializeObject(data) });
            //    await SendMessageAsync(socket, result);

            //    return;
            //}

            //if (settings.RateType == RateType.Market)
            //{
            //    var data = new
            //    {
            //        sell = _blockchainCurrencyService.GetCurrency(Domain.Enum.CurrencyTypeEnum.RUB).Sell * (1 - settings.Commission),
            //        buy = _blockchainCurrencyService.GetCurrency(Domain.Enum.CurrencyTypeEnum.RUB).Buy * (1 + settings.Commission)
            //    };

            //    var answer = JsonConvert.SerializeObject(new SocketJSON { Type = "updateCurrency", Message = JsonConvert.SerializeObject(data) });
            //    await SendMessageAsync(socket, answer);

            //    return;
            //}

            return;
        }
    }



    public class SocketJSON
    {
        [JsonProperty("type")]
        public SubscribeType Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class SocketUser
    {
        public string ID { get; set; }
        public SubscribeType Type { get; set; }
        public string Pair { get; set; }
    }
}
