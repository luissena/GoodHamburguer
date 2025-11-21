using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryCartRepository : ICartRepository
{
    private readonly Dictionary<Guid, Cart> _carts = new();

    public Task CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _carts[cart.Id] = cart;
        return Task.CompletedTask;
    }

    public Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _carts.TryGetValue(id, out var cart);
        return Task.FromResult(cart);
    }

    public Task<IEnumerable<Cart>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Cart>>(_carts.Values.ToList());
    }

    public Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        if (_carts.ContainsKey(cart.Id))
        {
            _carts[cart.Id] = cart;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _carts.Remove(cart.Id);
        return Task.CompletedTask;
    }
}

