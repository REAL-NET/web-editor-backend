using Auth.Models;
using Auth.Services;
using Microsoft.AspNetCore.Mvc;

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

        
        /// <summary>
        /// Authenticates user by login and password.
        /// </summary>
        /// <param name="authInfo">Login and password</param>
        /// <returns>Login and JWT token</returns>
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

        /// <summary>
        /// Checks token for user.
        /// </summary>
        /// <param name="tokenInfo">User and token</param>
        /// <returns>if token is valid</returns>
        [HttpGet("validate")]
        public ActionResult<bool> Validate([FromBody] TokenInfo tokenInfo)
        {
            return Ok(_userService.ValidateToken(tokenInfo.Login, tokenInfo.Token));
        }

        /// <summary>
        /// Register new user by login, password and name.
        /// Id is not to be specified.
        /// </summary>
        /// <param name="userInfo">Login, password and name</param>
        /// <returns>if user is successfully registered</returns>
        [HttpPost("register")]
        public ActionResult<bool> RegisterUser([FromBody] UserInfo userInfo)
        {
            return Ok(_userService.Register(userInfo));
        }
        
    }
}