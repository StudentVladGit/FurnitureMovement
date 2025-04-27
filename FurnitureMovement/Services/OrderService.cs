using Blazorise;
using FurnitureMovement.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public interface IOrderService
{
    // Методы для работы с заказами
    Task CreateOrder(Order newOrder, Furniture newOrderFurniture);
    Task UpdateOrder(Order updatedOrder);
    Task DeleteOrder(int id);
    Task<List<Order>> GetAllOrders();

    // Добавляем новые методы для работы с наименованием оснастки
    Task AddFurnitureName(FurnitureName orderName);
    Task UpdateFurnitureName(FurnitureName orderName);
    Task DeleteFurnitureName(int id);
    Task<List<FurnitureName>> GetAllFurnitureNames();

    // Добавляем новые методы для работы с авторами
    Task AddAuthor(OrderAuthor author);
    Task UpdateAuthor(OrderAuthor author);
    Task DeleteAuthor(int id);
    Task<List<OrderAuthor>> GetAllAuthors();

    //Работа со складом
    Task<List<Order>> GetCompletedOrders(); // Новый метод для получения выполненных заказов

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

    // CREATE ORDER
    public async Task CreateOrder(Order newOrder, Furniture newOrderFurniture)
    {
        using var context = _myFactory.CreateDbContext();

        var orderNameExists = await context.FurnitureNames.AnyAsync(x => x.ID == newOrderFurniture.FurnitureNameID);
        var authorExists = await context.OrderAuthors.AnyAsync(x => x.ID == newOrder.OrderAuthorID);

        if (!orderNameExists) throw new ArgumentException("Указанное наименование не существует");
        if (!authorExists) throw new ArgumentException("Указанный автор не существует");

        newOrder.Furnitures = new List<Furniture> { newOrderFurniture };
        newOrderFurniture.Order = newOrder;

        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();

    }
    // CREATE ORDER

    // UPDATE ORDER
    public async Task UpdateOrder(Order updatedOrder)
    {
        using var context = _myFactory.CreateDbContext();

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
		existingOrder.OrderPriority = updatedOrder.OrderPriority;
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
    // UPDATE ORDER

    // DELETE ORDER
    public async Task DeleteOrder(int id)
    {
        using var context = _myFactory.CreateDbContext();

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


            order.DeleteIndicator = 1; //Установили 1, значит удалили

            // Сохраняем изменения
            await context.SaveChangesAsync();
        }

    }
    // DELETE ORDER

    // READ ORDER
    public async Task<List<Order>> GetAllOrders()
    {
        using var context = _myFactory.CreateDbContext();

        return await context.Orders
            .Where(o => o.DeleteIndicator == 0) //Возвращает только не удаленные заказы
            .Include(o => o.OrderAuthor)
            .Include(o => o.Furnitures)
                .ThenInclude(f => f.FurnitureName)
            .ToListAsync();
    }
    // READ ORDER



    // ADD FURNITURE_NAME
    public async Task AddFurnitureName(FurnitureName FurnitureName)
    {
        using var context = _myFactory.CreateDbContext();

        if (string.IsNullOrWhiteSpace(FurnitureName.Name))
            throw new ArgumentException("Наименование не может быть пустым");

        await context.FurnitureNames.AddAsync(FurnitureName);
        await context.SaveChangesAsync();

    }
    // ADD FURNITURE_NAME

    // UPDATE FURNITURE_NAME
    public async Task UpdateFurnitureName(FurnitureName FurnitureName)
    {
        using var context = _myFactory.CreateDbContext();

        var existing = await context.FurnitureNames.FindAsync(FurnitureName.ID);
        if (existing == null)
            throw new ArgumentException("Оснастка не найдена");

        existing.Name = FurnitureName.Name;
        await context.SaveChangesAsync();

    }
    // UPDATE FURNITURE_NAME

    // DELETE FURNITURE_NAME
    public async Task DeleteFurnitureName(int id)
    {
        using var context = _myFactory.CreateDbContext();

        var Furniture_Name = await context.FurnitureNames.FindAsync(id);
        if (Furniture_Name == null)
            throw new ArgumentException("Оснастка не найдена");

        // Проверяем, нет ли заказов с этой оснасткой
        var hasFurniture = await context.Furnitures
            .Include(f => f.FurnitureName)
            .AnyAsync(od => od.FurnitureNameID == id && od.FurnitureName.DeleteIndicator == 0);
        if (hasFurniture)
            throw new InvalidOperationException("Нельзя удалить оснастку, так как существуют связанные заказы");

        Furniture_Name.DeleteIndicator = 1;
        await context.SaveChangesAsync();

    }
    // DELETE FURNITURE_NAME

    // READ FURNITURE_NAME
    public async Task<List<FurnitureName>> GetAllFurnitureNames()
    {
        var context = _myFactory.CreateDbContext();
        return await context.FurnitureNames
            .Where(o => o.DeleteIndicator == 0)
            .ToListAsync();
    }
    // READ FURNITURE_NAME



    // ADD AUTHOR
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
    // ADD AUTHOR

    // UPDATE AUTHOR
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
    // UPDATE AUTHOR

    // DELETE AUTHOR
    public async Task DeleteAuthor(int id)
    {
        using var context = _myFactory.CreateDbContext();

        var author = await context.OrderAuthors.FindAsync(id);
        if (author == null)
            throw new ArgumentException("Автор не найден");

        var hasAuthor = await context.Orders.AnyAsync(od => od.OrderAuthorID == id && od.DeleteIndicator == 0);
        if (hasAuthor)
            throw new InvalidOperationException("Нельзя удалить автора, у которого есть заказы");

        author.DeleteIndicator = 1;
        await context.SaveChangesAsync();

    }
    // DELETE AUTHOR

    // READ AUTHOR
    public async Task<List<OrderAuthor>> GetAllAuthors()
    {
        using var context = _myFactory.CreateDbContext();
        return await context.OrderAuthors
            .Where(o => o.DeleteIndicator == 0)
            .ToListAsync();
    }
    // READ AUTHOR



    // READ ORDERS Where status = Completed
    public async Task<List<Order>> GetCompletedOrders()
    {
        using var context = _myFactory.CreateDbContext();

        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Completed && o.DeleteIndicator == 0)
            .Include(o => o.Furnitures)
                .ThenInclude(f => f.FurnitureName)
            .ToListAsync();
    }
}
