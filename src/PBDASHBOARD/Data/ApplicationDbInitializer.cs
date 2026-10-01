using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;
using PBDASHBOARD.Models;
using PBDASHBOARD.Models.ModelMapping;
using PBDASHBOARD.User;

namespace PBDASHBOARD.Data
{
    public class ApplicationDbInitializer

    {  
        public static void Initialize(ApplicationDbContext db, bool isDevelopment,UserManager<CurrentReGroupMember> um)
        {
            
          
            if (!isDevelopment)
            { 
                var admin = new CurrentReGroupMember()
                {
                    Role = UserRole.SuperUser, UserName = "SuperUser",
                    Email = Environment.GetEnvironmentVariable("INITIAL_ADMIN_EMAIL") ?? "admin@example.invalid"
                };
                db.Database.Migrate();
                var initialPassword = Environment.GetEnvironmentVariable("INITIAL_ADMIN_PASSWORD");
                if (string.IsNullOrWhiteSpace(initialPassword))
                    throw new InvalidOperationException("Set INITIAL_ADMIN_PASSWORD before administrator initialization.");
                var created = um.CreateAsync(admin, initialPassword).GetAwaiter().GetResult();
                if (!created.Succeeded)
                    throw new InvalidOperationException("Administrator initialization failed.");
                um.AddToRoleAsync(admin, UserRole.SuperUser).GetAwaiter().GetResult();
                
               
                 db.SaveChangesAsync();
                Console.WriteLine(" Migrate ...........");
            } 
            /*db.Database.EnsureDeleted();
            db.Database.EnsureCreated();*/ 
            
          


        }
    }
}
