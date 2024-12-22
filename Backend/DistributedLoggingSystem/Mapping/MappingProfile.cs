using AutoMapper;
using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.Models;

namespace DistributedLoggingSystem.Mapping
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<LogEntry, LogDto>().ReverseMap();
        }
    }
}
