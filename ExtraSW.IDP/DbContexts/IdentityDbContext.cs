using ExtraSW.IDP.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExtraSW.IDP.DbContexts
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Subject)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                    Password = "password",
                    Subject = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    UserName = "Admin",
                    Active = true
                },
                   new User
                   {
                       Id = new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                       Password = "password",
                       Subject = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                       UserName = "Vendor",
                       Active = true
                   });
            modelBuilder.Entity<UserClaim>().HasData(
                
                    new UserClaim { 
                        Id = Guid.NewGuid(),
                        UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                        Type = "given_name",
                        Value = "Alice"
                    },
                    new UserClaim
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                        Type = "family_name",
                        Value = "Smith"
                    },
                    new UserClaim
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("13229d33-99e0-41b3-b18d-4f72127e3971"),
                        Type = "role",
                        Value = "Admin"
                    },
                    new UserClaim
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                        Type = "given_name",
                        Value = "Will"
                    },
                    new UserClaim
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                        Type = "family_name",
                        Value = "Smith"
                    },
                    new UserClaim
                    {
                        Id = Guid.NewGuid(),
                        UserId = new Guid("96053525-d4a5-47ee-855e-0ea77fa6c55a"),
                        Type = "role",
                        Value = "Vendor"
                    }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var updatedConcurrencyAwareEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified)
                .OfType<IConcurrencyAware>();

            foreach (var entry in updatedConcurrencyAwareEntries)
            {
                entry.ConcurrencyStamp = Guid.NewGuid().ToString();
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
