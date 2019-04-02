using Binance.Net;
using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using Gold.IO.Exchange.API.Domain.Enum;
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
        private IUserService UserService { get; set; }
        private IOrderService OrderService { get; set; }

        public List<SocketUser> socketUsers = new List<SocketUser>();
        public List<SocketUser> orderBookSubscribers = new List<SocketUser>();

        public NotificationsMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            //var myTimer = new System.Timers.Timer();
            //myTimer.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
            //myTimer.Interval = 3000;
            //myTimer.Start();
        }

        public void SetServices(
            IUserService userService,
            IOrderService orderService)
        {
            if (UserService == null)
                UserService = userService;

            if (OrderService == null)
                OrderService = orderService;
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var socketUser = socketUsers.FirstOrDefault(x => x.ID.Equals(socketId));

            if (socketUser != null)
                socketUsers.Remove(socketUser);

            var orderBookSocketUser = orderBookSubscribers.FirstOrDefault(x => x.ID.Equals(socketId));
            if (orderBookSocketUser != null)
                orderBookSubscribers.Remove(orderBookSocketUser);

            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var recieveJSON = JsonConvert.DeserializeObject<SocketJSON>(message);
                var existSocketUser = socketUsers.FirstOrDefault(x => x.ID.Equals(socketId));

                if (existSocketUser == null)
                {
                    var socketUser = new SocketUser { ID = socketId, Pairs = new List<string>() };
                    socketUser.Pairs.Add(recieveJSON.Pair);

                    socketUsers.Add(socketUser);
                    if (recieveJSON.Type.Equals("orderBook"))
                        orderBookSubscribers.Add(socketUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //public void DisplayTimeEvent(object source, ElapsedEventArgs e)
        //{
        //    foreach (var socket in this.WebSocketConnectionManager.GetAll())
        //    {
        //        if (socket.Value.State == WebSocketState.Open)
        //        {
        //            var existSocketConnection = socketUsers.FirstOrDefault(x => x.ID.Equals(socket.Key));
        //            if (existSocketConnection != null)
        //                SendCurrencyForUser(socket.Value);
        //        }
        //    }
        //}

        //private async void SendCurrencyForUser(WebSocket socket)
        //{
        //    var data = new
        //    {
        //        test = "123"
        //    };

        //    var answer = JsonConvert.SerializeObject(new SocketJSON { Type = "updateCurrency", Message = JsonConvert.SerializeObject(data) });
        //    await SendMessageAsync(socket, answer);

        //    return;
        //}
    }

    public class SocketJSON
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("pair")]
        public string Pair { get; set; }
    }

    public class SocketUser
    {
        public string ID { get; set; }
        public List<string> Pairs { get; set; }
    }
}
