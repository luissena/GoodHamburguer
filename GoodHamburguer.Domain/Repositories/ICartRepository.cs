using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Repositories;

public interface ICartRepository
{
    Task CreateAsync(Cart cart, CancellationToken cancellationToken = default);

    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Cart>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    Task DeleteAsync(Cart cart, CancellationToken cancellationToken = default);
}

