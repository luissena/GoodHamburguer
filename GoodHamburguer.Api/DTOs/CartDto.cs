using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a complete shopping cart with items, subtotal and applied discounts")]
public record CartDto(
    Guid Id,
    IEnumerable<CartItemDto> Items,
    decimal Subtotal,
    decimal? DiscountAmount,
    string? AppliedDiscountName,
    decimal Total,
    int TotalItems
);

[SwaggerSchema(Description = "Represents an item in the cart")]
public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

[SwaggerSchema(Description = "Empty DTO for creating a cart")]
public record CreateCartDto();

[SwaggerSchema(Description = "DTO for adding an item to the cart")]
public record AddItemToCartDto(
    Guid ProductId,
    int Quantity
);

[SwaggerSchema(Description = "DTO for updating item quantity in the cart")]
public record UpdateItemQuantityDto(
    int Quantity
);

