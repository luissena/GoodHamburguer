using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a maximum quantity rule for products or categories")]
public record OrderQuantityRuleDto(
    Guid Id,
    Guid? ProductId,
    Guid? CategoryId,
    int MaxQuantity,
    string RuleName
);

[SwaggerSchema(Description = "DTO for creating a quantity rule")]
public record CreateOrderQuantityRuleDto(
    Guid? ProductId,
    Guid? CategoryId,
    int MaxQuantity,
    string RuleName
);

[SwaggerSchema(Description = "DTO for updating a quantity rule. All fields are optional.")]
public record UpdateOrderQuantityRuleDto(
    Guid? ProductId,
    Guid? CategoryId,
    int? MaxQuantity,
    string? RuleName
);

