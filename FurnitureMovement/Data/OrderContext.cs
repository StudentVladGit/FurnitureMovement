using Microsoft.EntityFrameworkCore;

namespace FurnitureMovement.Data;

public class OrderContext : DbContext
{
    // DbSet-свойства добавляем после описания наших табличных классов, сейчас не нужно
    public DbSet<Order> Orders { get; set; }

    // Обязательная настройка конструктора. Конфигуратор в Startup’е использует этот конструктор
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns();
    }
}
