using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.DTO
{
    public class S3LogMetadata
    {
        public string ObjectKey { get; set; }
        public DateTime LastModified { get; set; }
    }
}
