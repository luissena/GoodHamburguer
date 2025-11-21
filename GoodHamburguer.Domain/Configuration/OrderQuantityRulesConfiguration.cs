using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Configuration;

public static class OrderQuantityRulesConfiguration
{
    public static IEnumerable<OrderQuantityRuleEntity> GetDefaultRules(
        Func<string, Guid?> getCategoryIdByName,
        Func<string, Guid?> getProductIdByName)
    {
        var rules = new List<OrderQuantityRuleEntity>();

        var sandwichesCategoryId = getCategoryIdByName("Sandwiches");
        if (sandwichesCategoryId.HasValue)
        {
            rules.Add(OrderQuantityRuleEntity.Create(
                null,
                sandwichesCategoryId.Value,
                1,
                "sandwich"));
        }

        var friesProductId = getProductIdByName("Fries");
        if (friesProductId.HasValue)
        {
            rules.Add(OrderQuantityRuleEntity.Create(
                friesProductId.Value,
                null,
                1,
                "fries"));
        }

        var softDrinkProductId = getProductIdByName("Soft Drink");
        if (softDrinkProductId.HasValue)
        {
            rules.Add(OrderQuantityRuleEntity.Create(
                softDrinkProductId.Value,
                null,
                1,
                "soft drink"));
        }

        return rules;
    }
}

