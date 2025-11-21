using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Repositories;

public interface ICategoryRepository
{
    Task CreateAsync(Category category, CancellationToken cancellationToken = default);

    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);

    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
}

