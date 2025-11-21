using GoodHamburguer.Client.Models;

namespace GoodHamburguer.Client.Services;

public interface IOrderQuantityRuleService
{
    Task<IEnumerable<OrderQuantityRuleDto>> GetRulesAsync(CancellationToken cancellationToken = default);
}

