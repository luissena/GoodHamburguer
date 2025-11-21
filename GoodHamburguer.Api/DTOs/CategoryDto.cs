using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a product category")]
public record CategoryDto(
    Guid Id,
    string Name
);

[SwaggerSchema(Description = "DTO for creating a category")]
public record CreateCategoryDto(
    string Name
);

[SwaggerSchema(Description = "DTO for updating a category")]
public record UpdateCategoryDto(
    string Name
);

