using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Repositories;

public interface IOrderRepository
{
    Task CreateAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);

    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    Task DeleteAsync(Order order, CancellationToken cancellationToken = default);
}

