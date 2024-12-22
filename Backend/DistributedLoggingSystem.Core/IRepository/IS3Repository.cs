using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.IRepository
{
    public interface IS3Repository
    {
        Task UploadFileAsync(LogEntry logDto);
        Task<IEnumerable<LogEntry>> RetrieveAllLogsAsync();
     }
}
