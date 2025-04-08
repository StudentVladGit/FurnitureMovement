using Microsoft.EntityFrameworkCore;

namespace FurnitureMovement.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {

    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderFurniture> OrderFurnitures { get; set; }
    public DbSet<OrderName> OrderNames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }
}
