using System.Net.Http.Json;
using System.Text.Json;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services.Abstractions;

namespace GoodHamburguer.Client.Services;

public class CartService : ICartService
{
    private readonly HttpClient _httpClient;

    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                using var jsonDoc = JsonDocument.Parse(errorContent);
                if (jsonDoc.RootElement.TryGetProperty("error", out var errorElement))
                {
                    return errorElement.GetString() ?? "Unknown error";
                }
            }
        }
        catch
        {
        }

        return $"Request error: {response.StatusCode} - {response.ReasonPhrase}";
    }

    private async Task ThrowHttpExceptionIfNotSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await GetErrorMessageAsync(response);
            throw new HttpRequestException(errorMessage);
        }
    }

    public async Task<CartDto> CreateCartAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("api/carts", null, cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CartDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize cart response");
    }

    public async Task<CartDto?> GetCartByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/carts/{id}", cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CartDto>(cancellationToken: cancellationToken);
    }

    public async Task<CartDto> AddItemToCartAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var dto = new { ProductId = productId, Quantity = quantity };
        var response = await _httpClient.PostAsJsonAsync($"api/carts/{cartId}/items", dto, cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CartDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize cart response");
    }

    public async Task<CartDto> UpdateItemQuantityAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var dto = new { Quantity = quantity };
        var response = await _httpClient.PutAsJsonAsync($"api/carts/{cartId}/items/{productId}", dto, cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CartDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize cart response");
    }

    public async Task<CartDto> RemoveItemFromCartAsync(Guid cartId, Guid productId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/carts/{cartId}/items/{productId}", cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<CartDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize cart response");
    }
}

