using AutoMapper;
using DistributedLoggingSystem.Core.Const;
using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using DistributedLoggingSystem.EF.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLoggingSystem.Controllers
{
    [ApiController]
    [Route("v1/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogRepository _logRepository;

        public LogsController(ILogRepository logRepository) {
            _logRepository=logRepository;
        }
        [HttpPost]
        public async Task<ResponseDto> AddLog(LogDto logEntry)
        {
            if (string.IsNullOrEmpty(logEntry.Service) || string.IsNullOrEmpty(logEntry.Level) || string.IsNullOrEmpty(logEntry.Message))
                return new ResponseDto(){IsSuccess=false, Message="Service, Level, and Message are required fields." };
            logEntry.Timestamp = logEntry.Timestamp == default ? DateTime.UtcNow : logEntry.Timestamp;
            return await _logRepository.AddLog(logEntry);
        }
        [HttpGet]
        public async Task<ResponseDto> GetLogs([FromQuery] LogParameters logParameters)
        {
            return await _logRepository.Getlogs(logParameters);
        }
        [HttpGet("GetLogById")]
        public async Task<ResponseDto> GetLogById(int? id = null, string? service = null) 
        {
            var response = new ResponseDto();
            var log = await _logRepository.GetLogByIdAsync(id,service);
            if (log is not null)
            {
                response.IsSuccess = true;
                response.Result = log;
                return response;
            }
            response.IsSuccess = false;
            response.Message = "Not Found";
            return response;
        }
    }
}
