using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a complete product")]
public record ProductDto(
    Guid Id,
    string Name,
    CategoryDto Category,
    decimal Price
);

[SwaggerSchema(Description = "DTO for creating a product")]
public record CreateProductDto(
    string Name,
    Guid CategoryId,
    decimal Price
);

[SwaggerSchema(Description = "DTO for updating a product. All fields are optional.")]
public record UpdateProductDto(
    string? Name,
    Guid? CategoryId,
    decimal? Price
);

