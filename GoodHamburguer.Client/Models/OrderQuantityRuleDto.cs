namespace GoodHamburguer.Client.Models;

public record OrderQuantityRuleDto(
    Guid Id,
    Guid? ProductId,
    Guid? CategoryId,
    int MaxQuantity,
    string RuleName
);

