using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.EF.Repository
{
    public class LogConsumerService: ILogConsumerService
    {
        private readonly RabbitMQSenderOptions _option;
        private readonly ConnectionFactory _factory;
        private IConnection _connection;
        private IChannel _channel;
        private readonly ConcurrentQueue<LogEntry> _logQueue = new();
        public LogConsumerService(IOptions<RabbitMQSenderOptions> options)
        {
            _option=options.Value;

        }
        private async void CreateConnection()
        {

            var factory = new ConnectionFactory()
            {
                UserName = _option.UserName,
                Password = _option.Password,
                HostName = _option.HostName,
            };
            _connection = await factory.CreateConnectionAsync();


        }
        private bool IsConnectionExist()
        {
            if (_connection == null)
            {
                return true;
            }
            CreateConnection();
            return true;
        }
      
  
        private async Task InitializeConsumer(string queueName)
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

            _channel = await _connection.CreateChannelAsync();
                await _channel.QueueDeclareAsync(queue: queueName,
                                                 durable: false,   
                                                 exclusive: false,
                                                 autoDelete: false,
                                                 arguments: null);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var logEntry = JsonConvert.DeserializeObject<LogEntry>(message);
                        if (logEntry != null)
                        {
                            _logQueue.Enqueue(logEntry);
                            Console.WriteLine($"Log received: {logEntry.Service} - {logEntry.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to process log: {ex.Message}");
                    }


                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                await _channel.BasicConsumeAsync(queue: queueName,
                                      autoAck: false, 
                                      consumer: consumer);

                Console.WriteLine($"Consumer initialized and listening to queue: {queueName}");
            
        }
        public async Task<List<LogEntry>> GetLogs()
        {
            await InitializeConsumer("LogQueue");
            var logs = new List<LogEntry>();

            while (_logQueue.TryDequeue(out var log))
            {
                logs.Add(log);
            }

            return logs;
        }
    }
}
