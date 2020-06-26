using liblib_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace liblib_backend.Controllers.UserController
{
    [Route("api/[controller]/")]
    public class AccountController : ControllerBase
    {
        private IAccountService userService;

        public AccountController(IAccountService userService)
        {
            this.userService = userService;
        }

        [Route("login")]
        [HttpPost]
        public ResultDTO Login([FromBody] UserDTO user)
        {
            if (user == null)
            {
                return new ResultDTO() {
                    Success = false,
                    Message = "Thông tin định dạng sai" 
                };
            }
            return userService.Login(user.Username, user.Password);
        }

        [Route("register")]
        [HttpPost]
        public ResultDTO Register([FromBody] UserDTO user)
        {
            return userService.Register(user.Username, user.Password);
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("create")]
        [HttpPost]
        public ResultDTO Create([FromBody] UserDTO user)
        {
            return userService.Create(user.Username, user.Password);
        }

        [Authorize]
        [Route("detail")]
        [HttpGet]
        public UserDetailDTO Detail()
        {
            return userService.Detail(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }

        [Authorize]
        [Route("upload")]
        [HttpPost]
        public ResultDTO UploadImage(IFormFile image)
        {
            return userService.UploadImage(Guid.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)), image);
        }

        [Route("image/{name}")]
        [HttpGet]
        public IActionResult GetImage(string name)
        {
            return userService.GetImage(name);
        }
    }
}
