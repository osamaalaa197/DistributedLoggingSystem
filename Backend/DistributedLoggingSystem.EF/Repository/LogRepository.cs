using AutoMapper;
using DistributedLoggingSystem.Core.Const;
using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using DistributedLoggingSystem.EF.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DistributedLoggingSystem.EF.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly DistributedLoggingSystemDbContext _context;
        private ResponseDto _responseDto;
        private readonly IMapper _mapper;
        private readonly IS3Repository _s3Repository;
        private readonly string _fileDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
        private readonly string _defaultBackend;
        private readonly IRabbitMQLogSender _rabbitMQLogSender;
        private readonly ILogConsumerService _logConsumerService;

        public LogRepository(DistributedLoggingSystemDbContext context,IMapper imapper,IConfiguration configuration, IS3Repository s3Repository,IRabbitMQLogSender rabbitMQLogSender,ILogConsumerService logConsumerService)
        {
            _context=context;
            _responseDto=new ResponseDto();
            _mapper=imapper;
            _s3Repository = s3Repository;
            if (!Directory.Exists(_fileDirectory))
            {
                Directory.CreateDirectory(_fileDirectory);
            }
            _defaultBackend = configuration.GetValue<string>("DefaultStorageBackend") ?? BackendType.Database;
            _rabbitMQLogSender = rabbitMQLogSender;
            _logConsumerService= logConsumerService;

        }

        public async Task<ResponseDto> AddLog(LogDto logEntryDto)
        {
            try
            {
                var logEntry = _mapper.Map<LogEntry>(logEntryDto);
                switch (_defaultBackend.ToLower())
                {
                    case BackendType.FileSystem:
                        var fileName = $"{logEntry.Service}_{logEntry.Timestamp:yyyy-MM-dd_HH-mm-ss}.json";
                        var filePath = Path.Combine(_fileDirectory, fileName);
                        Random rnd = new Random();
                        logEntry.Id = rnd.Next(1,int.MaxValue);
                        var jsonContent = JsonSerializer.Serialize(logEntry);
                        await File.WriteAllTextAsync(filePath, jsonContent);
                        logEntry.BackendType = BackendType.FileSystem.ToString();
                        _responseDto.Message = "Log saved in local file Successfully";
                        _responseDto.IsSuccess = true;
                        _responseDto.Result = logEntry;
                        break;
                    case BackendType.Database:
                        logEntry.BackendType = BackendType.Database.ToString();
                        await _context.LogEntries.AddAsync(logEntry);
                        await _context.SaveChangesAsync();
                        _responseDto.Message = "Log saved in database Successfully";
                        _responseDto.IsSuccess = true;
                        _responseDto.Result = logEntry;
                        break;
                    case BackendType.S3:
                        await _s3Repository.UploadFileAsync(logEntry);
                        _responseDto.Message = "Log saved to S3 successfully.";
                        _responseDto.IsSuccess = true;
                        break;
                    case BackendType.RabbitMQ:
                        _rabbitMQLogSender.SendMessage(logEntry, "LogQueue");
                        _responseDto.Message = "Log send to RabbitMQ successfully.";
                        _responseDto.IsSuccess = true;
                        break;
                }
            }
            catch (Exception ex) 
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
  
            }
            return _responseDto;
        }

        public async Task<ResponseDto> Getlogs(LogParameters logParameters)
        {
            var logEntries = new List<LogEntry>();
            try
            {
                switch (_defaultBackend.ToLower())
                {
                    case BackendType.FileSystem:
                        var logFiles = Directory.GetFiles(_fileDirectory, "*.json");
                        foreach (var logFile in logFiles)
                        {
                            var content = await File.ReadAllTextAsync(logFile);
                            var logEntry = JsonSerializer.Deserialize<LogEntry>(content);

                            if (logEntry != null)  
                            {
                                logEntries.Add(logEntry);
                            }
                        }
                        _responseDto.TotalRecords = logEntries.Count();
                        logEntries = FilterLogEntries(logEntries, logParameters);
                        _responseDto.Result = _mapper.Map<List<LogDto>>(logEntries);
                        _responseDto.PageNumber = logParameters.PageNumber;
                        _responseDto.PageSize = logParameters.PageSize;
                        break;

                    case BackendType.Database:
                        var query = _context.LogEntries.AsQueryable();
                        if (!string.IsNullOrEmpty(logParameters.Service))
                            query = query.Where(log => log.Service == logParameters.Service);

                        if (!string.IsNullOrEmpty(logParameters.Level))
                            query = query.Where(log => log.Level == logParameters.Level);

                        if (logParameters.StartTime.HasValue)
                            query = query.Where(log => log.Timestamp >= logParameters.StartTime);

                        if (logParameters.EndTime.HasValue)
                            query = query.Where(log => log.Timestamp <= logParameters.EndTime);
                        var pagedLogs = await query
                            .Skip((logParameters.PageNumber - 1) * logParameters.PageSize)
                            .Take(logParameters.PageSize)
                            .ToListAsync();
                        _responseDto.Result = _mapper.Map<List<LogDto>>(pagedLogs);
                        _responseDto.TotalRecords = await query.CountAsync(); 
                        _responseDto.PageNumber = logParameters.PageNumber; 
                        _responseDto.PageSize = logParameters.PageSize;
                        break;
                    case BackendType.S3:
                        var log = await _s3Repository.RetrieveAllLogsAsync();
                        logEntries = FilterLogEntries(logEntries, logParameters);
                        _responseDto.TotalRecords = log.ToList().Count();
                        _responseDto.PageNumber = logParameters.PageNumber;
                        _responseDto.PageSize = logParameters.PageSize;
                        _responseDto.Result = _mapper.Map<List<LogDto>>(logEntries);
                        break;
                    case BackendType.RabbitMQ:
                        var logMessage= await _logConsumerService.GetLogs();
                        logEntries=FilterLogEntries(logMessage, logParameters);
                        _responseDto.TotalRecords = logMessage.ToList().Count();
                        _responseDto.PageNumber = logParameters.PageNumber;
                        _responseDto.PageSize = logParameters.PageSize;
                        _responseDto.Result = _mapper.Map<List<LogDto>>(logEntries);
                        break;
                    default:
                        _responseDto.Message = "Unsupported backend type.";
                        return _responseDto;
                }
                _responseDto.Message = "Logs retrieved successfully.";
                _responseDto.IsSuccess = true;
            }
            catch (Exception ex) 
            {
                _responseDto.Message = ex.Message.ToString();
                _responseDto.IsSuccess = false;
            }
            return _responseDto;
        }

        public async Task<LogDto?> GetLogByIdAsync(int? logId=null, string? service=null)
        {
            var backendType = _defaultBackend.ToLower();
            switch (backendType)
            {
                case BackendType.Database:
                    var logFromDb = await _context.LogEntries.FirstOrDefaultAsync(e => e.Id == logId);
                    if (logFromDb != null)
                    {
                        return _mapper.Map<LogDto>(logFromDb);
                    }
                    break;
                case BackendType.FileSystem:
                    var logEntries = new List<LogEntry>();
                    var logFiles = Directory.GetFiles(_fileDirectory, "*.json");
                    foreach (var logFile in logFiles)
                    {
                        var content = await File.ReadAllTextAsync(logFile);
                        var logEntry = JsonSerializer.Deserialize<LogEntry>(content);

                        if (logEntry != null)
                        {
                            logEntries.Add(logEntry);
                        }
                    }
                    var logFromFileSystem = logEntries.FirstOrDefault(e=>e.Service == service);
                    if (logFromFileSystem != null) 
                    {
                        return _mapper.Map<LogDto>(logFromFileSystem);
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unsupported backend type.");
            }
            return null;
        }
        private List<LogEntry> FilterLogEntries(List<LogEntry> logEntries, LogParameters logParameters)
        {
            if (!string.IsNullOrEmpty(logParameters.Service))
                logEntries = logEntries.Where(log => log.Service == logParameters.Service).ToList();

            if (!string.IsNullOrEmpty(logParameters.Level))
                logEntries = logEntries.Where(log => log.Level == logParameters.Level).ToList();

            if (logParameters.StartTime.HasValue)
                logEntries = logEntries.Where(log => log.Timestamp >= logParameters.StartTime).ToList();

            if (logParameters.EndTime.HasValue)
                logEntries = logEntries.Where(log => log.Timestamp <= logParameters.EndTime).ToList();

            var pagedLogs = logEntries
                    .Skip((logParameters.PageNumber - 1) * logParameters.PageSize)
                    .Take(logParameters.PageSize);
            return pagedLogs.ToList();
        }

    }
}
