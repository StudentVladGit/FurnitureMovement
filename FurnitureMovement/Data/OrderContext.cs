using Microsoft.EntityFrameworkCore;
using FurnitureMovement.Data;

namespace FurnitureMovement.Data
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Furniture> Furnitures { get; set; }
        public DbSet<FurnitureName> FurnitureNames { get; set; }
        public DbSet<OrderAuthor> OrderAuthors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Использование serial columns для PostgreSQL
            modelBuilder.UseSerialColumns();

            // Конфигурация связи Order-Furniture (один-ко-многим)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Furnitures)
                .WithOne(f => f.Order)
                .HasForeignKey(f => f.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Конфигурация связи Order-OrderAuthor (многие-к-одному)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderAuthor)
                .WithMany()
                .HasForeignKey(o => o.OrderAuthorID)
                .OnDelete(DeleteBehavior.Restrict);

            // Конфигурация связи Furniture-FurnitureName (многие-к-одному)
            modelBuilder.Entity<Furniture>()
                .HasOne(f => f.FurnitureName)
                .WithMany()
                .HasForeignKey(f => f.FurnitureNameID)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка имен таблиц (соответствует вашим предпочтениям)
            modelBuilder.Entity<Furniture>().ToTable("Furnitures");
            modelBuilder.Entity<FurnitureName>().ToTable("FurnitureNames");
            modelBuilder.Entity<OrderAuthor>().ToTable("OrderAuthors");
        }
    }
}