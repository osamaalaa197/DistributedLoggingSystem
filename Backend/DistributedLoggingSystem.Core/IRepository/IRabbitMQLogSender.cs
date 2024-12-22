using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.IRepository
{
    public interface IRabbitMQLogSender
    {
        void SendMessage(object message,string queueName);
    }
}
