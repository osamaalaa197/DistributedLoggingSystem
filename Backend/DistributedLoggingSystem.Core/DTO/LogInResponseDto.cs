using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.DTO
{
    public class LogInResponseDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
