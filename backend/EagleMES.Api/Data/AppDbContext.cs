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
        public DbSet<SysUser> Users => Set<SysUser>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Device> Devices => Set<Device>();
        public DbSet<EventLog> EventLogs => Set<EventLog>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    }
}
