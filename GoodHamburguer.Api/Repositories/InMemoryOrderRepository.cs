using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _orders = new();

    public Task CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Order>>(_orders.Values.ToList());
    }

    public Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        var orders = _orders.Values.Where(o => o.Status == status).ToList();
        return Task.FromResult<IEnumerable<Order>>(orders);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (_orders.ContainsKey(order.Id))
        {
            _orders[order.Id] = order;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders.Remove(order.Id);
        return Task.CompletedTask;
    }
}

