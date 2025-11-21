using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services.Abstractions;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
}


