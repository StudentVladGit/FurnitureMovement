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

    // Новые методы для работы со складом
    Task AddToWarehouse(WarehouseItem item);
    Task RemoveFromWarehouse(int id, long quantityToRemove);
    Task<List<WarehouseItem>> GetAllWarehouseItems();
}

public class OrderService : IOrderService
{
    private IDbContextFactory<OrderContext> _myFactory;
    private readonly NotificationService _notificationService;

    // Пороговое значение для разового списания
    private const long SingleRemovalThreshold = 50; // Порог для одного списания
    private const long LowStockThreshold = 10;

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
            furniture.FurnitureName = null;
        }

        await context.Orders.AddAsync(newOrder);
        await context.SaveChangesAsync();
    }

    public async Task UpdateOrder(Order updatedOrder)
    {
        using var context = _myFactory.CreateDbContext();
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingOrder = await context.Orders
                .Include(o => o.OrderAuthor)
                .Include(o => o.Furnitures)
                .ThenInclude(f => f.FurnitureName)
                .FirstOrDefaultAsync(o => o.ID == updatedOrder.ID);

            if (existingOrder == null)
            {
                await context.Orders.AddAsync(updatedOrder);
            }
            else
            {
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
                        var orderNameExists = await context.FurnitureNames.AnyAsync(x => x.ID == furniture.FurnitureNameID);
                        if (!orderNameExists)
                            throw new ArgumentException($"Наименование с ID {furniture.FurnitureNameID} не существует");

                        furniture.OrderID = existingOrder.ID;
                        furniture.FurnitureName = null;
                        context.Furnitures.Add(furniture);
                    }
                }

                // Если статус изменился на Completed или ToWarehouse, добавляем данные на склад
                if (oldStatus != OrderStatus.Completed && updatedOrder.OrderStatus == OrderStatus.Completed || 
                    oldStatus != OrderStatus.ToWarehouse && updatedOrder.OrderStatus == OrderStatus.ToWarehouse)
                {
                    foreach (var furniture in updatedOrder.Furnitures)
                    {
                        var furnitureName = await context.FurnitureNames
                            .FirstOrDefaultAsync(fn => fn.ID == furniture.FurnitureNameID);

                        if (furnitureName == null)
                            throw new InvalidOperationException($"Не найдено наименование оснастки с ID {furniture.FurnitureNameID}");

                        var warehouseItem = new WarehouseItem
                        {
                            FurnitureName = furnitureName.Name,
                            FurnitureNameId = furniture.FurnitureNameID,
                            Quantity = furniture.OrderQuantity,
                            AdmissionDate = updatedOrder.AdmissionDate,
                            Material = furnitureName.Material ?? "Не указано",
                            Drawing = furnitureName.Drawing,
                            Image = furnitureName.Image
                        };
                        await AddToWarehouse(warehouseItem);
                    }
                }

                if (oldStatus != existingOrder.OrderStatus)
                {
                    _notificationService.AddNotification(
                        $"{existingOrder.OrderNumber} изменил статус на {existingOrder.OrderStatus.GetDescription()} в {DateTime.Now:HH:mm}",
                        existingOrder.OrderStatus);
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Ошибка при обновлении заказа: {ex.Message}", ex);
        }
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
            .AnyAsync(a => a.Name.ToLower() == author.Name.ToLower() && author.DeleteIndicator == 0);

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
                          a.Name.ToLower() == author.Name.ToLower() && author.DeleteIndicator == 0);

        if (duplicateExists)
            throw new ArgumentException("Автор с таким именем уже существует");

        existing.Name = author.Name;
        existing.Division = author.Division;
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


    public async Task AddToWarehouse(WarehouseItem item)
    {
        using var context = _myFactory.CreateDbContext();

        var existingItem = await context.WarehouseItems
            .FirstOrDefaultAsync(w => w.FurnitureNameId == item.FurnitureNameId);

        if (existingItem != null)
        {
            // Обновляем количество
            existingItem.Quantity += item.Quantity;

            // Обновляем дату поступления, если новая дата позже
            existingItem.AdmissionDate = item.AdmissionDate > existingItem.AdmissionDate ? item.AdmissionDate : existingItem.AdmissionDate;

            // Обновляем чертеж и фото, если они отличаются
            if (existingItem.Drawing != item.Drawing)
            {
                existingItem.Drawing = item.Drawing;
            }

            if (existingItem.Image != item.Image)
            {
                existingItem.Image = item.Image;
            }

            // Обновляем материал, если он изменился
            if (existingItem.Material != item.Material)
            {
                existingItem.Material = item.Material;
            }

            context.WarehouseItems.Update(existingItem);
        }
        else
        {
            await context.WarehouseItems.AddAsync(item);
        }

        await context.SaveChangesAsync();
    }

    public async Task RemoveFromWarehouse(int id, long quantityToRemove)
{
    using var context = _myFactory.CreateDbContext();

    var item = await context.WarehouseItems.FindAsync(id);
    if (item != null)
    {
        if (quantityToRemove > SingleRemovalThreshold)
        {
            _notificationService.AddNotification(
                $"Перерасход материала! Списано {quantityToRemove} единиц оснастки ({item.FurnitureName}, материал: {item.Material}) за раз. Порог: {SingleRemovalThreshold}.",
                OrderStatus.Cancelled);
        }

        if (quantityToRemove >= item.Quantity)
        {
            context.WarehouseItems.Remove(item);
        }
        else if (quantityToRemove > 0)
        {
            item.Quantity -= quantityToRemove;
            if (item.Quantity < LowStockThreshold)
            {
                _notificationService.AddNotification(
                    $"Низкий уровень остатков! Осталось {item.Quantity} единиц оснастки ({item.FurnitureName}, материал: {item.Material}). Порог: {LowStockThreshold}.",
                    OrderStatus.InThePreparationProcess);
            }
            context.WarehouseItems.Update(item);
        }
        else
        {
            throw new ArgumentException("Количество для списания должно быть положительным.");
        }

        await context.SaveChangesAsync();
    }
}

    public async Task<List<WarehouseItem>> GetAllWarehouseItems()
    {
        using var context = _myFactory.CreateDbContext();
        return await context.WarehouseItems.ToListAsync();
    }
}