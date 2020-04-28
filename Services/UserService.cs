using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyLib.Common;
using MyLib.Controllers.UserController;
using MyLib.Models;
using MyLib.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.Services
{
    public interface IUserService : ITransientService
    {
        ResultDTO Login(string username, string password);
        ResultDTO Create(string username, string password);
        ResultDTO Register(string username, string password);
    }

    public class UserService : IUserService
    {

        private IConfiguration configuration;
        private IUserRepository userRepository;


        public UserService(IConfiguration configuration, IUserRepository userRepository)
        {
            this.configuration = configuration;
            this.userRepository = userRepository;
        }


        public ResultDTO Login(string username, string password)
        {
            Account account = userRepository.GetAccountWithUsername(username);

            if (account == null)
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Tên đăng nhập không tồn tại"
                };
            }

            if (!account.Password.Equals(password))
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Sai mật khẩu"
                };
            }

            return new ResultDTO
            {
                Success = true,
                Message = GenerateToken(account.Id, account.Role)
            };
        }


        public ResultDTO Create(string username, string password)
        {
            if (userRepository.GetAccountWithUsername(username) != null)
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Tài khoản đã tồn tại"
                };
            }

            Account account = new Account
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = password,
                DateCreated = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsActive = false,
                Role = "Member"
            };

            if (!userRepository.CreateAccount(account))
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Hệ thống gặp lỗi! Vui lòng thử lại sau"
                };
            }

            return new ResultDTO
            {
                Success = true,
                Message = GenerateToken(account.Id, "Member")
            };
        }

        public ResultDTO Register(string username, string password)
        {
            if (userRepository.GetAccountWithUsername(username) != null)
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Tài khoản đã tồn tại"
                };
            }

            Account account = new Account
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = password,
                DateCreated = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                IsActive = true,
                Role = "Librarian"
            };

            if (!userRepository.CreateAccount(account))
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Hệ thống gặp lỗi! Vui lòng thử lại sau"
                };
            }

            return new ResultDTO
            {
                Success = true,
                Message = GenerateToken(account.Id, "Librarian")
            };
        }


        private string GenerateToken(Guid accountId, string role)
        {
            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SigningKey"]));
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, accountId.ToString())
                })
            };

            return handler.WriteToken(handler.CreateToken(descriptor));
        }

    }
}
