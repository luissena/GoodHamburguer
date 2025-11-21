using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Repositories;

public interface IOrderQuantityRuleRepository
{
    Task CreateAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default);
    Task<OrderQuantityRuleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderQuantityRuleEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default);
    Task DeleteAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default);
    Task ClearAllAsync(CancellationToken cancellationToken = default);
}

