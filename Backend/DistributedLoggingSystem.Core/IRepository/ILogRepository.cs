using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.IRepository
{
    public interface ILogRepository
    {
        Task<ResponseDto> AddLog(LogDto logEntry);
        Task<ResponseDto> Getlogs(LogParameters logParameters);
        Task<LogDto> GetLogByIdAsync(int? logId,string? service=null);


    }
}
