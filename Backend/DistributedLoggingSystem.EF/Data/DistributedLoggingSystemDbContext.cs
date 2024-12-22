using DistributedLoggingSystem.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistributedLoggingSystem.EF.Data
{
    public class DistributedLoggingSystemDbContext: IdentityDbContext<ApplicationUser>
    {
        public DistributedLoggingSystemDbContext(DbContextOptions<DistributedLoggingSystemDbContext> options):base(options)
        {
        }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
