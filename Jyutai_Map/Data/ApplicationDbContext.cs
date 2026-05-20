using Microsoft.EntityFrameworkCore;
using Jyutai_Map.Models;

namespace Jyutai_Map.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrafficReport> TrafficReports { get; set; } = null!;
    }
}
