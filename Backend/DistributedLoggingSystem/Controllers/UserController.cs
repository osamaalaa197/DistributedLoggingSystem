using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.EF.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DistributedLoggingSystem.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository) { _userRepository = userRepository; }
        [HttpPost]
        public async Task<ResponseDto> LogIn(LogInDto logInDto)
        {
           return await _userRepository.Login(logInDto);
        }
        [HttpPost]
        public Task<ResponseDto> Register(RegisterDto registerDto)
        {
           return _userRepository.Register(registerDto);
        }
    }
}
