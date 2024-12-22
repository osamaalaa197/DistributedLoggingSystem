using DistributedLoggingSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.Core.IRepository
{
    public interface IUserRepository
    {
        //public IUserRepository(DistributedLoggingSystemDbContext context) { }
        Task< ResponseDto> Login(LogInDto logInDto);
        Task<ResponseDto> Register(RegisterDto registerDto);
    }
}
