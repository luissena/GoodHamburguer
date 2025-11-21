using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly Dictionary<Guid, Category> _categories = new();

    public Task CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _categories[category.Id] = category;
        return Task.CompletedTask;
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _categories.TryGetValue(id, out var category);
        return Task.FromResult(category);
    }

    public Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var category = _categories.Values.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(category);
    }

    public Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Category>>(_categories.Values.ToList());
    }

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (_categories.ContainsKey(category.Id))
        {
            _categories[category.Id] = category;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        _categories.Remove(category.Id);
        return Task.CompletedTask;
    }
}

