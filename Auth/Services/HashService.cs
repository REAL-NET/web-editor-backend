using System.Security.Cryptography;
using System.Text;
using Auth.Models;

namespace Auth.Services
{
    public static class HashService
    {
        private static SHA256 _sha256 = SHA256.Create();
        
        public static void HashPassword(UserInfo userInfo)
        {
            userInfo.Password = HashPassword(userInfo.Login, userInfo.Password);
        }
        
        public static string HashPassword(string login, string password)
        {
            var salted = login + password;
            return Encoding.UTF8.GetString(_sha256.ComputeHash(Encoding.UTF8.GetBytes(salted)));
        }
    }
}