using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;

namespace PBDASHBOARD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // here starts dotnet application. 

            var host = CreateHostBuilder(args).Build(); // we store the infrastructure to start dotnet app her.

            using (var scope = host.Services.CreateScope())
            {
                // everything happening in this scope is before the application is running, it is a set up..
                var services = scope.ServiceProvider; // IserviceProvider is an object used to get list of 
                //registered services from the services container at the startup.
                var context = services.GetRequiredService<ApplicationDbContext>(); /*here Iservices gets the service 
               applicationDbContext registered at the startup as a service.*/
                var environment = services.GetService<IWebHostEnvironment>();
                var um = services.GetService<Microsoft.AspNetCore.Identity.UserManager<CurrentReGroupMember>>();
                //initializes the Database. 
                ApplicationDbInitializer.Initialize(context, environment.IsDevelopment(), um);
            }

            host.Run(); // this line starts the app. 
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}