using FurnitureMovement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public class OrderService: IOrderService
{
    public readonly IDbContextFactory<OrderContext> _myFactory; 

    
    public OrderService(IDbContextFactory<OrderContext> myFactory)
    {
        _myFactory = myFactory;
    }

    //Create
    public async Task CreateOrder(Order newOrder)
    {
        using var context = _myFactory.CreateDbContext();
        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();
    }

    //Read
    public async Task<List<Order>> GetAllOrders()
    {
        using var context = _myFactory.CreateDbContext();
        return await context.Orders.ToListAsync();
    }

    public async Task<Order> GetOrderById(int id)
    {
        using var context = _myFactory.CreateDbContext();
        return await context.Orders.FindAsync(id);
    }

    // Update
    public async Task Update(Order updatedOrder)
    {
        using var context = _myFactory.CreateDbContext();
        context.Orders.Update(updatedOrder);
        await context.SaveChangesAsync();
    }

    // Delete
    public async Task Delete(int id)
    {
        using var context = _myFactory.CreateDbContext();
        var order = await context.Orders.FindAsync(id);
        if (order != null)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }
    }
}


public interface IOrderService
{
    Task CreateOrder(Order newOrder);
    Task<List<Order>> GetAllOrders();
    Task<Order> GetOrderById(int id);
    Task Update(Order updatedOrder);
    Task Delete(int id);
}
