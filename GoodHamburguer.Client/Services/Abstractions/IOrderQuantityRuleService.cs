using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services.Abstractions;

public interface IOrderQuantityRuleService
{
    Task<IEnumerable<OrderQuantityRuleDto>> GetRulesAsync(CancellationToken cancellationToken = default);
}


