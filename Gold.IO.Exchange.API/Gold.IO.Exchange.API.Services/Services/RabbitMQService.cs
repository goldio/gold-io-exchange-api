using Gold.IO.Exchange.API.BusinessLogic.Interfaces;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gold.IO.Exchange.API.BusinessLogic.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private IModel Channel { get; set; }

        public RabbitMQService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            Channel = factory.CreateConnection().CreateModel();

            Channel.QueueDeclare(queue: "crypto",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }
    }
}
