using liblib_backend.Common;
using liblib_backend.Controllers;
using liblib_backend.Controllers.UserController;
using liblib_backend.Models;
using liblib_backend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace liblib_backend.Services
{
    public interface IUserService : ITransientService
    {
        ResultDTO Login(string username, string password);
        ResultDTO Create(string username, string password);
        ResultDTO Register(string username, string password);
        UserDetailDTO Detail(Guid userId);
        ResultDTO UploadImage(Guid userId, IFormFile file);
        IActionResult GetImage(string name);
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


        public ResultDTO Register(string username, string password)
        {
            if (!ValidateUsername(username))
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Tên tài khoản không hợp lệ"
                };
            }

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
                Image = "https://user-images.githubusercontent.com/30195/34457818-8f7d8c76-ed82-11e7-8474-3825118a776d.png",
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

        public ResultDTO Create(string username, string password)
        {
            if (!ValidateUsername(username))
            {
                return new ResultDTO
                {
                    Success = false,
                    Message = "Tên tài khoản không hợp lệ"
                };
            }

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
                Image = "https://user-images.githubusercontent.com/30195/34457818-8f7d8c76-ed82-11e7-8474-3825118a776d.png",
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
                Expires = DateTime.UtcNow.AddYears(1),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, accountId.ToString())
                })
            };

            return handler.WriteToken(handler.CreateToken(descriptor));
        }

        public UserDetailDTO Detail(Guid userId)
        {
            Account account = userRepository.GetAccountWithId(userId);
            if (account == null)
            {
                return null;
            }
            return new UserDetailDTO()
            {
                Username = account.Username,
                Image = account.Image
            };
        }

        public ResultDTO UploadImage(Guid userId, IFormFile file)
        {
            if (file == null)
            {
                return new ResultDTO()
                {
                    Success = false,
                    Message = "Cập nhật ảnh thất bại"
                };
            }
            Account account = userRepository.GetAccountWithId(userId);
            string name = account.Image.Substring(account.Image.LastIndexOf('/') + 1);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Image", name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            name = Path.GetRandomFileName() + ".png";
            path = Path.Combine(Directory.GetCurrentDirectory(), "Image", name);
            FileStream stream = File.Create(path);
            file.CopyTo(stream);
            stream.Close();
            account.Image = "http://blueto0th.ddns.net:5000/api/user/image/" + name;
            userRepository.UpdateAccount(account);
            return new ResultDTO()
            {
                Success = true,
                Message = "Cập nhật ảnh thành công"
            };
        }

        private bool ValidateUsername(string username)
        {
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public IActionResult GetImage(string name)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Image", name);
            if (File.Exists(path))
            {
                return new PhysicalFileResult(path, "image/png");
            }
            return new NotFoundResult();
        }
    }
}
