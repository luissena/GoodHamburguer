using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a discount with its conditions and percentage")]
public record DiscountDto(
    Guid Id,
    string Name,
    decimal Percentage,
    bool IsActive,
    IEnumerable<DiscountConditionDto> Conditions
);

[SwaggerSchema(Description = "Represents a discount condition")]
public record DiscountConditionDto(
    Guid? ProductId,
    Guid? CategoryId,
    int MinimumQuantity
);

[SwaggerSchema(Description = "DTO for creating a discount")]
public record CreateDiscountDto(
    string Name,
    decimal Percentage,
    IEnumerable<CreateDiscountConditionDto> Conditions
);

[SwaggerSchema(Description = "DTO for creating a discount condition")]
public record CreateDiscountConditionDto(
    Guid? ProductId,
    Guid? CategoryId,
    int MinimumQuantity
);

[SwaggerSchema(Description = "DTO for updating a discount. All fields are optional.")]
public record UpdateDiscountDto(
    string? Name,
    decimal? Percentage,
    bool? IsActive
);

