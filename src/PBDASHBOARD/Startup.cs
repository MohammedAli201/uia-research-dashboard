using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PBDASHBOARD.Configuration;
using PBDASHBOARD.Data;
using PBDASHBOARD.Models;
using PBDASHBOARD.User;
using Polly;
using Polly.Extensions.Http;
using BackgroundService = PBDASHBOARD.BackgroundTaks.BackgroundService;

namespace PBDASHBOARD
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;

        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //this line will read the configuration that we have Appsetings.json
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));
            services.AddControllersWithViews();
            //services.AddSingleton<IHostedService, SampleTask>();
            services.AddHttpClient("Cristin")
                .AddPolicyHandler(GetRetryPolicy());
            services.AddHostedService<BackgroundService>();
           
        
            services.AddIdentity<CurrentReGroupMember,CustomRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<CustomRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddIdentityServer()
                .AddApiAuthorization<CurrentReGroupMember, ApplicationDbContext>();


            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                //we will define the key that we will use.
                var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
                // add the setting that we want 
                jwt.SaveToken = true;
                jwt.TokenValidationParameters= new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = false
                };
            });

            
            // // this is database service. 
            // if (_env.IsDevelopment())
            // {
            //    
            //     services.AddDbContext<ApplicationDbContext>(options =>
            //         options.UseSqlite(
            //             Configuration.GetConnectionString("DefaultConnection")));
            //     
            // }else
            // {
            //     services.AddDbContext<ApplicationDbContext>(options =>
            //         options.UseNpgsql(
            //             Configuration.GetConnectionString("DefaultConnection")));
            // }
            //
            services.AddAuthentication();
            
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "wwwroot"; });
        }
        // adding polly policy here
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
           // .OrResult(msg=>msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(_env.IsDevelopment()){
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }
    }
}
