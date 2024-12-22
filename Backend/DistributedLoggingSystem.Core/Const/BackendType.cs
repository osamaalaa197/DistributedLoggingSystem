using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.Const
{
    public static class BackendType
    {
        public const string FileSystem = "filesystem";
        public const string Database = "database";
        public const string S3 = "s3";
        public const string RabbitMQ = "rabbitmq";

    }
}
