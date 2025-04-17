using FurnitureMovement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public interface IOrderService
{
    Task CreateOrder(Order newOrder, Furniture newOrderFurniture);
    Task<List<Order>> GetAllOrders();
    Task<List<FurnitureName>> GetAllOrderNames();
    Task Update(Order updatedOrder);
    Task Delete(int id);

    // Добавляем новые методы для работы с наименованием оснастки
    Task AddOrderName(FurnitureName orderName);
    Task UpdateOrderName(FurnitureName orderName);
    Task DeleteOrderName(int id);

    // Добавляем новые методы для работы с авторами
    Task<List<OrderAuthor>> GetAllAuthors();
    Task AddAuthor(OrderAuthor author);
    Task UpdateAuthor(OrderAuthor author);
    Task DeleteAuthor(int id);

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
    public async Task CreateOrder(Order newOrder, Furniture newOrderFurniture)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            var orderNameExists = await context.FurnitureNames.AnyAsync(x => x.ID == newOrderFurniture.FurnitureNameID);
            var authorExists = await context.OrderAuthors.AnyAsync(x => x.ID == newOrder.OrderAuthorID);

            if (!orderNameExists) throw new ArgumentException("Указанное наименование не существует");
            if (!authorExists) throw new ArgumentException("Указанный автор не существует");

            newOrder.Furnitures = new List<Furniture> { newOrderFurniture };
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
                .Include(o => o.Furnitures) // Загружаем связанные OrderFurnitures
                .FirstOrDefaultAsync(o => o.ID == id);

            if (order != null)
            {
                // Удаляем все связанные OrderFurnitures
                if (order.Furnitures != null && order.Furnitures.Any())
                {
                    context.Furnitures.RemoveRange(order.Furnitures);
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
        using var context = _myFactory.CreateDbContext();
        return await context.Orders
            .Include(o => o.OrderAuthor)  // Добавлена загрузка автора
            .Include(o => o.Furnitures)
                .ThenInclude(f => f.FurnitureName)
            .ToListAsync();
    }

    public async Task<List<FurnitureName>> GetAllOrderNames()
    {
        var context = _myFactory.CreateDbContext();
        return await context.FurnitureNames.ToListAsync();
    }

    public async Task<List<OrderAuthor>> GetAllAuthors()
    {
        using var context = _myFactory.CreateDbContext();
        return await context.OrderAuthors.ToListAsync();
    }

    // Update
    public async Task Update(Order updatedOrder)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            // Получаем существующий заказ с OrderFurniture
            var existingOrder = await context.Orders
                .Include(o => o.OrderAuthor)
                .Include(o => o.Furnitures)
                .FirstOrDefaultAsync(o => o.ID == updatedOrder.ID);

            if (existingOrder == null) return;

            // Сохраняем старый статус ДО обновления
            var oldStatus = existingOrder.OrderStatus;

            // Обновляем основные свойства заказа
            existingOrder.OrderNumber = updatedOrder.OrderNumber;
            existingOrder.OrderStatus = updatedOrder.OrderStatus;
            existingOrder.AdmissionDate = updatedOrder.AdmissionDate;
            existingOrder.OrderAuthorID = updatedOrder.OrderAuthorID;

            // Получаем существующую и новую OrderFurniture
            var existingFurniture = existingOrder.Furnitures?.FirstOrDefault();
            var updatedFurniture = updatedOrder.Furnitures?.FirstOrDefault();

            if (existingFurniture != null && updatedFurniture != null)
            {
                // Обновляем свойства
                existingFurniture.OrderQuantity = updatedFurniture.OrderQuantity;
                existingFurniture.FurnitureNameID = updatedFurniture.FurnitureNameID;
            }
            else if (updatedFurniture != null)
            {
                // Если не было OrderFurniture, добавляем новую
                updatedFurniture.OrderID = existingOrder.ID;
                context.Furnitures.Add(updatedFurniture);
            }

            // Проверяем изменение статуса ПОСЛЕ обновления
            if (oldStatus != existingOrder.OrderStatus)
            {
                _notificationService.AddNotification(
                    $"{existingOrder.OrderNumber} изменил статус на {existingOrder.OrderStatus} в {DateTime.Now:HH:mm}",
                    existingOrder.OrderStatus);
            }

            await context.SaveChangesAsync();
        }
    }

    // Работа с именованием оснастки
    public async Task AddOrderName(FurnitureName orderName)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            if (string.IsNullOrWhiteSpace(orderName.Name))
                throw new ArgumentException("Наименование не может быть пустым");

            await context.FurnitureNames.AddAsync(orderName);
            await context.SaveChangesAsync();
        }
    }

    // Обновляем оснастку
    public async Task UpdateOrderName(FurnitureName orderName)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            var existing = await context.FurnitureNames.FindAsync(orderName.ID);
            if (existing == null)
                throw new ArgumentException("Оснастка не найдена");

            existing.Name = orderName.Name;
            await context.SaveChangesAsync();
        }
    }

    // Удаляем оснастку
    public async Task DeleteOrderName(int id)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            var Furniture_Name = await context.FurnitureNames.FindAsync(id);
            if (Furniture_Name == null)
                throw new ArgumentException("Оснастка не найдена");

            // Проверяем, нет ли заказов с этой оснасткой
            var hasFurniture = await context.Furnitures.AnyAsync(od => od.FurnitureNameID == id);
            if (hasFurniture)
                throw new InvalidOperationException("Нельзя удалить оснастку, так как существуют связанные заказы");

            context.FurnitureNames.Remove(Furniture_Name);
            await context.SaveChangesAsync();
        }
    }

    public async Task AddAuthor(OrderAuthor author)
    {
        using var context = _myFactory.CreateDbContext();
        if (string.IsNullOrWhiteSpace(author.Name))
            throw new ArgumentException("Имя автора не может быть пустым");

        // Проверка на уникальность имени автора
        var authorExists = await context.OrderAuthors
            .AnyAsync(a => a.Name.ToLower() == author.Name.ToLower());

        if (authorExists)
            throw new ArgumentException("Автор с таким именем уже существует");

        await context.OrderAuthors.AddAsync(author);
        await context.SaveChangesAsync();
    }


    public async Task UpdateAuthor(OrderAuthor author)
    {
        using var context = _myFactory.CreateDbContext();
        var existing = await context.OrderAuthors.FindAsync(author.ID);
        if (existing == null)
            throw new ArgumentException("Автор не найден");

        // Проверка на уникальность имени при обновлении
        var duplicateExists = await context.OrderAuthors
            .AnyAsync(a => a.ID != author.ID &&
                          a.Name.ToLower() == author.Name.ToLower());

        if (duplicateExists)
            throw new ArgumentException("Автор с таким именем уже существует");

        existing.Name = author.Name;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAuthor(int id)
    {
        using (var context = _myFactory.CreateDbContext())
        {
            var author = await context.OrderAuthors.FindAsync(id);
            if (author == null)
                throw new ArgumentException("Автор не найден");

            var hasAuthor = await context.Orders.AnyAsync(od => od.OrderAuthorID == id);
            if (hasAuthor)
                throw new InvalidOperationException("Нельзя удалить автора, у которого есть заказы");

            context.OrderAuthors.Remove(author);
            await context.SaveChangesAsync();
        }
    }
}
