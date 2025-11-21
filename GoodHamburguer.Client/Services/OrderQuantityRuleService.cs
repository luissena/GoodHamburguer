using System.Net.Http.Json;
using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services;

public class OrderQuantityRuleService : IOrderQuantityRuleService
{
    private readonly HttpClient _httpClient;

    public OrderQuantityRuleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<OrderQuantityRuleDto>> GetRulesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/orderquantityrules", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<OrderQuantityRuleDto>>(cancellationToken: cancellationToken)
            ?? Enumerable.Empty<OrderQuantityRuleDto>();
    }
}

