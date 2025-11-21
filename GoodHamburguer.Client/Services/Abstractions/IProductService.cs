using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services.Abstractions;

public interface IProductService
{
    Task<PagedResultDto<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<PagedResultDto<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default);
}


