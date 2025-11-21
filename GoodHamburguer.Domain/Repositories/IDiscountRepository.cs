using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Repositories;

public interface IDiscountRepository
{
    Task CreateAsync(Discount discount, CancellationToken cancellationToken = default);

    Task<Discount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Discount>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<Discount>> GetActiveAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Discount discount, CancellationToken cancellationToken = default);

    Task DeleteAsync(Discount discount, CancellationToken cancellationToken = default);
}

