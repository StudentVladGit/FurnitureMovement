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
    Task<List<OrderName>> GetAllOrderNames();
    Task Update(Order updatedOrder);
    Task Delete(int id);
}

public class OrderService : IOrderService
{
    private IDbContextFactory<OrderContext> _myFactory;
    private readonly NotificationService _notificationService;

    public OrderService(IDbContextFactory<OrderContext> myFactory, NotificationService notificationService)
    {
        _myFactory = myFactory;
        _notificationService = notificationService;
    }
    
    //Create
    public async Task CreateOrder(Order newOrder, OrderFurniture newOrderFurniture)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            var orderNameExists = await context.OrderNames.AnyAsync(x => x.ID == newOrder.OrderNameID);
            if (!orderNameExists)
            {
                throw new ArgumentException("Указанное наименование не существует");
            }

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
            // Находим заказ вместе с связанными записями OrderFurniture
            var order = await context.Orders
                .Include(o => o.Orders) // Загружаем связанные OrderFurnitures
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order != null)
            {
                // Удаляем все связанные OrderFurnitures
                if (order.Orders != null && order.Orders.Any())
                {
                    context.OrderFurnitures.RemoveRange(order.Orders);
                }

                // Удаляем сам заказ
                context.Orders.Remove(order);

                // Сохраняем изменения
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

    public async Task<List<OrderName>> GetAllOrderNames()
    {
        var context = _myFactory.CreateDbContext();
        return await context.OrderNames.ToListAsync();
    }

    // Update
    public async Task Update(Order updatedOrder)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            // Получаем существующий заказ с OrderFurniture
            var existingOrder = await context.Orders
                .Include(o => o.Orders)
                .FirstOrDefaultAsync(o => o.ID == updatedOrder.ID);

            if (existingOrder == null) return;

            // Обновляем основные свойства заказа
            existingOrder.OrderNumber = updatedOrder.OrderNumber;
            existingOrder.OrderNameID = updatedOrder.OrderNameID;

            // Получаем существующую и новую OrderFurniture
            var existingFurniture = existingOrder.Orders?.FirstOrDefault();
            var updatedFurniture = updatedOrder.Orders?.FirstOrDefault();

            if (existingFurniture != null && updatedFurniture != null)
            {
                // Сохраняем старый статус для проверки изменений
                var oldStatus = existingFurniture.OrderStatus;

                // Обновляем свойства
                existingFurniture.OrderQuantity = updatedFurniture.OrderQuantity;
                existingFurniture.OrderStatus = updatedFurniture.OrderStatus;
                existingFurniture.AdmissionDate = updatedFurniture.AdmissionDate;
                existingFurniture.OrderAuthor = updatedFurniture.OrderAuthor;

                // Проверяем изменение статуса
                if (oldStatus != existingFurniture.OrderStatus)
                {
                    _notificationService.AddNotification(
                        $"{updatedOrder.OrderNumber} изменил статус на {existingFurniture.OrderStatus} в {DateTime.Now:HH:mm}",
                        existingFurniture.OrderStatus);
                }
            }
            else if (updatedFurniture != null)
            {
                // Если не было OrderFurniture, добавляем новую
                updatedFurniture.OrderID = existingOrder.ID;
                context.OrderFurnitures.Add(updatedFurniture);
            }

            await context.SaveChangesAsync();
        }
    }
}
