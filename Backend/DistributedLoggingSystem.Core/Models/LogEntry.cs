using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.Models
{
    public class LogEntry
    {
        public int Id { get; set; } 
        public string Service { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } 
        public string BackendType { get; set; }
        public string? LogFilePath { get; set; }

    }
}
