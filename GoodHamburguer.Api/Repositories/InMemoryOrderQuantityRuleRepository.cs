using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryOrderQuantityRuleRepository : IOrderQuantityRuleRepository
{
    private readonly Dictionary<Guid, OrderQuantityRuleEntity> _rules = new();
    private readonly object _lock = new();

    public Task CreateAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_rules.ContainsKey(rule.Id))
            {
                throw new InvalidOperationException($"Regra com ID {rule.Id} já existe.");
            }

            _rules[rule.Id] = rule;
        }

        return Task.CompletedTask;
    }

    public Task<OrderQuantityRuleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _rules.TryGetValue(id, out var rule);
            return Task.FromResult<OrderQuantityRuleEntity?>(rule);
        }
    }

    public Task<IEnumerable<OrderQuantityRuleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult<IEnumerable<OrderQuantityRuleEntity>>(_rules.Values.ToList());
        }
    }

    public Task UpdateAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_rules.ContainsKey(rule.Id))
            {
                throw new InvalidOperationException($"Regra com ID {rule.Id} não encontrada.");
            }

            _rules[rule.Id] = rule;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(OrderQuantityRuleEntity rule, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_rules.Remove(rule.Id));
        }
    }

    public Task ClearAllAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _rules.Clear();
        }

        return Task.CompletedTask;
    }
}

