using EagleMES.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EagleMES.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    }
}
