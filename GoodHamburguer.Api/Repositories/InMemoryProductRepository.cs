using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _products = new();

    public Task CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products[product.Id] = product;
        return Task.CompletedTask;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _products.TryGetValue(id, out var product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var products = _products.Values.Where(p => p.Category.Id == categoryId).ToList();
        return Task.FromResult<IEnumerable<Product>>(products);
    }

    public Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Product>>(_products.Values.ToList());
    }

    public Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        if (_products.ContainsKey(product.Id))
        {
            _products[product.Id] = product;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        _products.Remove(product.Id);
        return Task.CompletedTask;
    }
}

