using Blazorise;
using FurnitureMovement.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace FurnitureMovement.Services;

public interface IOrderService
{
    Task CreateOrder(Order newOrder, List<Furniture> newOrderFurnitures);
    Task UpdateOrder(Order updatedOrder);
    Task DeleteOrder(int id);
    Task<List<Order>> GetAllOrders();

    Task AddFurnitureName(FurnitureName orderName);
    Task UpdateFurnitureName(FurnitureName orderName);
    Task DeleteFurnitureName(int id);
    Task<List<FurnitureName>> GetAllFurnitureNames();

    Task AddAuthor(OrderAuthor author);
    Task UpdateAuthor(OrderAuthor author);
    Task DeleteAuthor(int id);
    Task<List<OrderAuthor>> GetAllAuthors();

    Task<List<Order>> GetCompletedOrders();
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

    public async Task CreateOrder(Order newOrder, List<Furniture> newOrderFurnitures)
    {
        using var context = _myFactory.CreateDbContext();

        foreach (var furniture in newOrderFurnitures)
        {
            var orderNameExists = await context.FurnitureNames.AnyAsync(x => x.ID == furniture.FurnitureNameID);
            if (!orderNameExists)
                throw new ArgumentException($"Наименование с ID {furniture.FurnitureNameID} не существует");
        }

        var authorExists = await context.OrderAuthors.AnyAsync(x => x.ID == newOrder.OrderAuthorID);
        if (!authorExists)
            throw new ArgumentException("Указанный автор не существует");

        newOrder.Furnitures = newOrderFurnitures;
        foreach (var furniture in newOrderFurnitures)
        {
            furniture.Order = newOrder;
            furniture.FurnitureName = null; // Убедимся, что FurnitureName не добавляется
        }

        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();
    }

    public async Task UpdateOrder(Order updatedOrder)
    {
        using var context = _myFactory.CreateDbContext();

        var existingOrder = await context.Orders
            .Include(o => o.OrderAuthor)
            .Include(o => o.Furnitures)
            .FirstOrDefaultAsync(o => o.ID == updatedOrder.ID);

        if (existingOrder == null) return;

        var oldStatus = existingOrder.OrderStatus;

        existingOrder.OrderNumber = updatedOrder.OrderNumber;
        existingOrder.OrderStatus = updatedOrder.OrderStatus;
        existingOrder.OrderPriority = updatedOrder.OrderPriority;
        existingOrder.AdmissionDate = updatedOrder.AdmissionDate;
        existingOrder.OrderAuthorID = updatedOrder.OrderAuthorID;

        // Удаляем старые оснастки
        if (existingOrder.Furnitures != null && existingOrder.Furnitures.Any())
        {
            context.Furnitures.RemoveRange(existingOrder.Furnitures);
        }

        // Добавляем новые оснастки
        if (updatedOrder.Furnitures != null && updatedOrder.Furnitures.Any())
        {
            foreach (var furniture in updatedOrder.Furnitures)
            {
                // Проверяем, что FurnitureNameID ссылается на существующую запись
                var orderNameExists = await context.FurnitureNames.AnyAsync(x => x.ID == furniture.FurnitureNameID);
                if (!orderNameExists)
                    throw new ArgumentException($"Наименование с ID {furniture.FurnitureNameID} не существует");

                furniture.OrderID = existingOrder.ID;
                furniture.FurnitureName = null; // Убедимся, что FurnitureName не добавляется
                context.Furnitures.Add(furniture);
            }
        }

        if (oldStatus != existingOrder.OrderStatus)
        {
            _notificationService.AddNotification(
                $"{existingOrder.OrderNumber} изменил статус на {existingOrder.OrderStatus} в {DateTime.Now:HH:mm}",
                existingOrder.OrderStatus);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteOrder(int id)
    {
        using var context = _myFactory.CreateDbContext();

        var order = await context.Orders
            .Include(o => o.Furnitures)
            .FirstOrDefaultAsync(o => o.ID == id);

        if (order != null)
        {
            if (order.Furnitures != null && order.Furnitures.Any())
            {
                context.Furnitures.RemoveRange(order.Furnitures);
            }

            order.DeleteIndicator = 1;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Order>> GetAllOrders()
    {
        using var context = _myFactory.CreateDbContext();

        return await context.Orders
            .Where(o => o.DeleteIndicator == 0)
            .Include(o => o.OrderAuthor)
            .Include(o => o.Furnitures)
                .ThenInclude(f => f.FurnitureName)
            .ToListAsync();
    }

    public async Task AddFurnitureName(FurnitureName FurnitureName)
    {
        using var context = _myFactory.CreateDbContext();

        if (string.IsNullOrWhiteSpace(FurnitureName.Name))
            throw new ArgumentException("Наименование не может быть пустым");

        await context.FurnitureNames.AddAsync(FurnitureName);
        await context.SaveChangesAsync();
    }

    public async Task UpdateFurnitureName(FurnitureName FurnitureName)
    {
        using var context = _myFactory.CreateDbContext();

        var existing = await context.FurnitureNames.FindAsync(FurnitureName.ID);
        if (existing == null)
            throw new ArgumentException("Оснастка не найдена");

        existing.Name = FurnitureName.Name;
        existing.Material = FurnitureName.Material;
        existing.ProductionTime = FurnitureName.ProductionTime;
        existing.Drawing = FurnitureName.Drawing;
        existing.Image = FurnitureName.Image;
        existing.DeleteIndicator = FurnitureName.DeleteIndicator;

        await context.SaveChangesAsync();
    }

    public async Task DeleteFurnitureName(int id)
    {
        using var context = _myFactory.CreateDbContext();

        var Furniture_Name = await context.FurnitureNames.FindAsync(id);
        if (Furniture_Name == null)
            throw new ArgumentException("Оснастка не найдена");

        var hasFurniture = await context.Furnitures
            .Include(f => f.FurnitureName)
            .AnyAsync(od => od.FurnitureNameID == id && od.FurnitureName.DeleteIndicator == 0);
        if (hasFurniture)
            throw new InvalidOperationException("Нельзя удалить оснастку, так как существуют связанные заказы");

        Furniture_Name.DeleteIndicator = 1;
        await context.SaveChangesAsync();
    }

    public async Task<List<FurnitureName>> GetAllFurnitureNames()
    {
        var context = _myFactory.CreateDbContext();
        return await context.FurnitureNames
            .Where(o => o.DeleteIndicator == 0)
            .OrderBy(o => o.ID)
            .ToListAsync();
    }

    public async Task AddAuthor(OrderAuthor author)
    {
        using var context = _myFactory.CreateDbContext();
        if (string.IsNullOrWhiteSpace(author.Name))
            throw new ArgumentException("Имя автора не может быть пустым");

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

    public async Task<List<OrderAuthor>> GetAllAuthors()
    {
        using var context = _myFactory.CreateDbContext();
        return await context.OrderAuthors
            .Where(o => o.DeleteIndicator == 0)
            .OrderBy(o => o.ID)
            .ToListAsync();
    }

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