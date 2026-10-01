using System;
using System.IO;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PBDASHBOARD.Models;
using PBDASHBOARD.Models.AssociationModels;

namespace PBDASHBOARD.Data
{
    public class ApiDbContext<TUser, TRole, TKey> : IdentityDbContext<TUser, TRole, TKey>,
        IPersistedGrantDbContext
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;
        
        public ApiDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();
        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        { base.OnModelCreating(builder);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);
        }
        
    }

    public class ApplicationDbContext : ApiDbContext<CurrentReGroupMember, CustomRole, int>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            
        }
        
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Institute> Institutes { get; set; }
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Contributor> Contributors { get; set; }
        public DbSet<ResearchGroup> ResearchGroups { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Name> Names { get; set; } 
        public DbSet<Affiliation> Affiliations { get; set; }
        public DbSet<Institution> Institutions { get; set; }
        public DbSet<ResearchProject> ResearchProjects { get; set; }
        public DbSet<CurrentReGroupMember> CurrentReGroupMembers { get; set; }
        public DbSet<SynchronisationDate> syncDate { get; set; }
        
        
       

        /** Association tables**/
        
        public DbSet<ContributorReGroup> ContributorReGroups { get; set; }
        /// <summary>
        /// current research group members and research group association table
        /// </summary>
        public DbSet<ResearchGroupCurrentMembers> CurrentGroupMembers { get; set; }
        public DbSet<PublicationContributor> PublicationContributors { get; set; }
        public DbSet<PublicationInstitute> PublicationInstitutes { get; set; }
        public DbSet<InstituteResearchProject> InstituteResearchProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PublicationInstitute>().HasKey(db => new {db.PublicationId, db.InstituteId});
            modelBuilder.Entity<ContributorReGroup>().HasKey(db => new {db.ResearchGroupId, db.ContributorId});
            modelBuilder.Entity<PublicationContributor>().HasKey(db => new {db.ContributorId, db.PublicationId});
            modelBuilder.Entity<InstituteResearchProject>().HasKey(db => new {db.InstituteId, db.ResearchProjectId});
            modelBuilder.Entity<ResearchGroupCurrentMembers>().HasKey(db => new {db.ResearchGroupId, db.CurrentReGroupMemberId});
            modelBuilder.Entity<Affiliation>().HasKey(db => new {db.ContributorId,db.InstitutionId});

        }

       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
            if (!optionsBuilder.IsConfigured)
            {/*
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Development.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString);*/
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString);
            }
            
        }
        
    }
}