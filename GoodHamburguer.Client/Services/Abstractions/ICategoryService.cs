using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services.Abstractions;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken = default);
}


