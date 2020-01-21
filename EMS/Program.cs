using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace EMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            CreateDefaultAccountAsync(host).Wait();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        private static async Task CreateDefaultAccountAsync(IHost host)
        {
            using (IServiceScope scopeFactory = host.Services.CreateScope())
            {
                IServiceProvider services = scopeFactory.ServiceProvider;
                try
                {
                    UserManager<Admin> userManager = services.GetRequiredService<UserManager<Admin>>();
                    EMSContext context = services.GetRequiredService<EMSContext>();
                    context.Database.EnsureCreated();

                    Admin admin = await userManager.FindByNameAsync("admin");
                    if (admin == null)
                    {
                        admin = new Admin
                        {
                            UserName = "admin",
                            PasswordChangeRequired = true
                        };
                        IdentityResult result = await userManager.CreateAsync(admin, "EMSadmin123");
                        if (!result.Succeeded)
                        {
                            throw new InvalidOperationException("Error creating default user");
                        }
                        else
                        {
                            var claim = new Claim("PasswordChangeRequired", "true");
                            await userManager.AddClaimAsync(admin, claim);
                        }                        
                    }
                }
                catch (Exception e)
                {
                    ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Error creating default user", e);
                }
            }
        }
    }
}