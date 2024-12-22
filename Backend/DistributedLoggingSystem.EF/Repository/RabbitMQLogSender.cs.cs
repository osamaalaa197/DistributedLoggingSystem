using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.EF.Repository
{
    public class RabbitMQLogSender : IRabbitMQLogSender
    {
        private readonly RabbitMQSenderOptions _option;
        private IConnection _connection;
        public RabbitMQLogSender(IOptions<RabbitMQSenderOptions> options)
        {
            _option=options.Value;
        }
        public async void SendMessage(object message, string queueName)
        {
            if (_connection == null)
            {
                var factory = new ConnectionFactory()
                {
                    UserName = _option.UserName,
                    Password = _option.Password,
                    HostName = _option.HostName,
                };
                _connection = await factory.CreateConnectionAsync();
            }
            var channel = await _connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queueName, false, false, false, null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                await channel.BasicPublishAsync(exchange: "", routingKey: queueName, body: body);
            
        }
    }
}
