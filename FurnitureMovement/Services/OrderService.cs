using FurnitureMovement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public interface IOrderService
{
    Task CreateOrder(Order newOrder, OrderFurniture newOrderFurniture);
    Task<List<Order>> GetAllOrders();
    Task Update(Order updatedOrder);
    Task Delete(int id);
}

public class OrderService : IOrderService
{
    private IDbContextFactory<OrderContext> _myFactory;

    public OrderService(IDbContextFactory<OrderContext> myFactory)
    {
        _myFactory = myFactory;
    }
    
    //Create
    public async Task CreateOrder(Order newOrder, OrderFurniture newOrderFurniture)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            newOrder.Orders = new List<OrderFurniture> { newOrderFurniture };
            newOrderFurniture.Order = newOrder;

            await context.Orders.AddAsync(newOrder);
            await context.SaveChangesAsync();
        }
    }

    // Delete
    public async Task Delete(int id)
    {
        using (var context = _myFactory.CreateDbContext())
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
        using (var context = _myFactory.CreateDbContext())
        {
            return await context.Orders.Include(o => o.Orders).ToListAsync();
        }
    }

    // Update
    public async Task Update(Order updatedOrder)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            context.Orders.Update(updatedOrder);
            await context.SaveChangesAsync();
        }
    }
}
