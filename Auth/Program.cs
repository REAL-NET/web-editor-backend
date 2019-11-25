using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth.Data;
using Auth.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
             /* Code for database manual users adding and reading
             
            var options = new DbContextOptionsBuilder<UsersDbContext>()
                .UseSqlite("Data Source=users.db")
                .Options;
            using (var db = new UsersDbContext(options))
            {
                db.Add(new UserInfo {Login = "sample_user", Password = "sample_password", Name = "Sample User"});
                db.SaveChanges();

                var user = db.Users
                    .OrderBy(u => u.Id)
                    .First();
                
                Console.WriteLine(user.Name);
            }
            */
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}