using Domain;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure;

public interface IOrderService
{
    Task<Order?> GetOrderAsync(Guid orderId, CancellationToken ct);
    Task<Order> CreateOrderAsync(string customerName, CancellationToken ct);
}

internal class OrderService : IOrderService
{
    private readonly IFusionCache _cache;

    public OrderService(IFusionCache cache)
    {
        _cache = cache;
    }

    public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken ct)
    {
        return await _cache.GetOrDefaultAsync<Order>(orderId.ToString(), token: ct);
    }

    public async Task<Order> CreateOrderAsync(string customerName, CancellationToken ct)
    {
        Order order = new()
        {
            OrderId = Guid.NewGuid(),
            CustomerName = customerName,
            DateOrdered = DateTimeOffset.UtcNow,
        };
           
        await _cache.SetAsync(order.OrderId.ToString(), order, token: ct);
        
        return order;
    }
}
