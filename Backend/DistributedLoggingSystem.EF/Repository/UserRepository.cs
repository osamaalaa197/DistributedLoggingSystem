using DistributedLoggingSystem.Core.DTO;
using DistributedLoggingSystem.Core.IRepository;
using DistributedLoggingSystem.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.EF.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly JwtOptions _jwtoptions;

        public UserRepository(UserManager<ApplicationUser> userManager,IConfiguration configuration,IOptions<JwtOptions> jwtoptions)
        {
            _userManager= userManager;
            _configuration= configuration;
            _jwtoptions=jwtoptions.Value;
        }
        public async Task<ResponseDto> Login(LogInDto logInDto)
        {
            var response = new ResponseDto();
            var user = await _userManager.FindByEmailAsync(logInDto.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "Email not Valid ";
                return response;
            }
            var isexist = await _userManager.CheckPasswordAsync(user, logInDto.Password);
            if (!isexist)
            {
                response.IsSuccess = false;
                response.Message = "Wrong Password ";
                return response;
            }
            var token = GenerateToken(user);

            response.Result = new LogInResponseDto
            {
                Token = token,
                UserId = user.Id,
                UserName=user.UserName
            };
            return response;
        }

        public async Task<ResponseDto> Register(RegisterDto registerDto)
        {
            var response = new ResponseDto();
            var appUser = new ApplicationUser();
            appUser.UserName = registerDto.Name;
            appUser.Email = registerDto.Email;
            appUser.PhoneNumber = registerDto.PhoneNumber;
            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Message = result.Errors.FirstOrDefault().Description;
                return response;
            }
            response.IsSuccess = true;
            response.Message = "Account Register Successfully";
            return response;
        }
        private string GenerateToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtoptions.Secret);
            var claimList = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.Role,"User")
            };
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience =_jwtoptions.Audience,
                Issuer = _jwtoptions.Issuer,
                Expires = DateTime.UtcNow.AddDays(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(claimList)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
