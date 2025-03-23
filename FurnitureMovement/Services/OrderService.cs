using FurnitureMovement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public class OrderService : IOrderService
{
    private readonly DbContextOptions<OrderContext> _myFactory;

    public OrderService(DbContextOptions<OrderContext> myFactory)
    {
        _myFactory = myFactory;
    }
    private OrderContext CreateDbContext()
    {
        return new OrderContext(_myFactory);
    }

    //Create
    public async Task CreateOrder(Order newOrder)
    {
        //using var context = _myFactory.CreateDbContext();
        //await context.Orders.AddAsync(newOrder);
        //await context.SaveChangesAsync();

        using (var context = CreateDbContext())
        {
            context.Orders.Add(newOrder);
            await context.SaveChangesAsync();
        }
    }

    // Delete
    public async Task Delete(int id)
    {
        using (var context = CreateDbContext())
        {
            var order = await context.Orders.FindAsync(id);
            if (order != null)
            {
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
            }
        }
    }

    //Read
    public async Task<List<Order>> GetAllOrders()
    {
        using (var context = CreateDbContext())
        {
            return await context.Orders.ToListAsync();
        }
    }

    //public async Task<Order> GetOrderById(int id)
    //{
    //    using var context = _myFactory.CreateDbContext();
    //    return await context.Orders.FindAsync(id);
    //}

    // Update
    public async Task Update(Order updatedOrder)
    {
        using (var context = CreateDbContext())
        {
            context.Orders.Update(updatedOrder);
            await context.SaveChangesAsync();
        }
    }


}


public interface IOrderService
{
    Task CreateOrder(Order newOrder);
    Task<List<Order>> GetAllOrders();
    //Task<Order> GetOrderById(int id);
    Task Update(Order updatedOrder);
    Task Delete(int id);
}
