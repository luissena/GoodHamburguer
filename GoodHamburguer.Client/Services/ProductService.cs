using System.Net.Http.Json;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services.Abstractions;

namespace GoodHamburguer.Client.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/products?page={page}&pageSize={pageSize}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize products response");
    }

    public async Task<PagedResultDto<ProductDto>> GetProductsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/products/category/{categoryId}?page={page}&pageSize={pageSize}", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize products response");
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var allProducts = new List<ProductDto>();
        int page = 1;
        const int pageSize = 100;
        bool hasMore = true;

        while (hasMore)
        {
            var response = await _httpClient.GetAsync($"api/products?page={page}&pageSize={pageSize}", cancellationToken);
            response.EnsureSuccessStatusCode();
            var pagedResult = await response.Content.ReadFromJsonAsync<PagedResultDto<ProductDto>>(cancellationToken: cancellationToken);
            
            if (pagedResult != null && pagedResult.Items.Any())
            {
                allProducts.AddRange(pagedResult.Items);
                hasMore = page < pagedResult.TotalPages;
                page++;
            }
            else
            {
                hasMore = false;
            }
        }

        return allProducts;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}", cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: cancellationToken);
    }
}

