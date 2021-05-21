using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Data;
using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("")]
        public ActionResult<TokenInfo> AuthUser([FromBody] AuthInfo authInfo)
        {
            var userToken = _userService.Authenticate(authInfo.Login, authInfo.Password);
            if (userToken == null)
            {
                return BadRequest(new { message = "Wrong username or password" });
            }
            var tokenInfo = new TokenInfo() {Login = authInfo.Login, Token = userToken};
            return Ok(tokenInfo);
        }

        [HttpGet("validate")]
        public ActionResult<bool> Validate([FromBody] TokenInfo tokenInfo)
        {
            return Ok(_userService.ValidateToken(tokenInfo.Login, tokenInfo.Token));
        }

        [HttpPost("register")]
        public ActionResult<bool> RegisterUser([FromBody] UserInfo userInfo)
        {
            return Ok(_userService.Register(userInfo));
        }
        
    }
}