using System;
using System.IO;
using Auth.Data;
using Auth.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


namespace Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlite("Data Source=users.db"));
            var key = Configuration.GetSection("JwtSecret").Value;
            services.AddSingleton(Configuration);

            services.AddScoped<UserService, UserService>();
            
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "REAL.NET Auth API",
                    Version = "v1",
                    Description = "Web API to auth users of REAL.NET Repository",
                });

                var basePath = AppContext.BaseDirectory;

                //Set the comments path for the swagger json and ui.
                var xmlPath = Path.Combine(basePath, "Auth.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "REAL.NET API V1");
            });
            
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}