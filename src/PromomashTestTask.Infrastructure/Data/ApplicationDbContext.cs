using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PromomashTestTask.Core.Models;

namespace PromomashTestTask.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationGuardsman, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<Core.Models.System> Systems { get; set; }
        public DbSet<Planet> Planets { get; set; }
    }
}
