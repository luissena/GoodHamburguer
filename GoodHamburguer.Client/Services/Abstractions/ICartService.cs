using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services.Abstractions;

public interface ICartService
{
    Task<CartDto> CreateCartAsync(CancellationToken cancellationToken = default);
    Task<CartDto?> GetCartByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CartDto> AddItemToCartAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<CartDto> UpdateItemQuantityAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<CartDto> RemoveItemFromCartAsync(Guid cartId, Guid productId, CancellationToken cancellationToken = default);
}


