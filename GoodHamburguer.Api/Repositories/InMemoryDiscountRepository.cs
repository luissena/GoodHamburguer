using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryDiscountRepository : IDiscountRepository
{
    private readonly Dictionary<Guid, Discount> _discounts = new();

    public Task CreateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        _discounts[discount.Id] = discount;
        return Task.CompletedTask;
    }

    public Task<Discount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _discounts.TryGetValue(id, out var discount);
        return Task.FromResult(discount);
    }

    public Task<IEnumerable<Discount>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Discount>>(_discounts.Values.ToList());
    }

    public Task<IEnumerable<Discount>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var activeDiscounts = _discounts.Values.Where(d => d.IsActive).ToList();
        return Task.FromResult<IEnumerable<Discount>>(activeDiscounts);
    }

    public Task UpdateAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        if (_discounts.ContainsKey(discount.Id))
        {
            _discounts[discount.Id] = discount;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        _discounts.Remove(discount.Id);
        return Task.CompletedTask;
    }
}

