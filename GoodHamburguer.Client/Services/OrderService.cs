using System.Net.Http.Json;
using System.Text.Json;
using GoodHamburguer.Client.Models;
using GoodHamburguer.Client.Services.Abstractions;

namespace GoodHamburguer.Client.Services;

public class OrderService : IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderService(HttpClient httpClient)
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

    public async Task<OrderDto> CreateOrderAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        var dto = new { CartId = cartId };
        var response = await _httpClient.PostAsJsonAsync("api/orders", dto, cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize order response");
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/orders", cancellationToken);
        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrderDto>>(cancellationToken: cancellationToken)
            ?? Enumerable.Empty<OrderDto>();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/orders/{id}", cancellationToken);
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        await ThrowHttpExceptionIfNotSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken: cancellationToken);
    }
}

