using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Auth.Data;
using Auth.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Services
{
    public class UserService
    {
        private readonly string _secret;
        private readonly UsersDbContext _usersDb;

        public UserService(UsersDbContext usersDbContext, IConfiguration configuration)
        {
            _usersDb = usersDbContext;
            _secret = configuration.GetSection("JwtSecret").Value;
        }
        public string Authenticate([NotNull] string login, [NotNull] string password)
        {
            var users = _usersDb.Users.Where(u => u.Login.Equals(login) && u.Password.Equals(password));
            if (!users.Any())
            {
                return null;
            }
            var user = users.First();
            
            var key = Encoding.ASCII.GetBytes(_secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool Register([NotNull] UserInfo userInfo)
        {
            var users = _usersDb.Users.Where(u => u.Login.Equals(userInfo.Login));
            if (users.Any())
            {
                return false;
            }
            _usersDb.Add(userInfo);
            _usersDb.SaveChanges();
            return true;
        }

        public bool ValidateToken([NotNull] string login, [NotNull] string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
                return false;
            ClaimsIdentity identity;
            try
            {
                identity = (ClaimsIdentity) principal.Identity;
            }
            catch (NullReferenceException)
            {
                return false;
            }
            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            var id = int.Parse(usernameClaim.Value);
            var user = _usersDb.Users.Find(id);
            return user != null && login.Equals(user.Login);
        }
        
        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken) tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                var key = Encoding.ASCII.GetBytes(_secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                IdentityModelEventSource.ShowPII = true;
                var principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}