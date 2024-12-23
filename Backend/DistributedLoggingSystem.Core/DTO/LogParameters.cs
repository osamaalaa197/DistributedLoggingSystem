﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.DTO
{
    public class LogParameters
    {
        public string? Service { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10;  
    

}
}
