using Swashbuckle.AspNetCore.Annotations;

namespace GoodHamburguer.Api.DTOs;

[SwaggerSchema(Description = "Represents a complete order with items, status and values")]
public record OrderDto(
    Guid Id,
    IEnumerable<OrderItemDto> Items,
    string Status,
    DateTime CreatedAt,
    decimal Subtotal,
    decimal? DiscountAmount,
    decimal Total
);

[SwaggerSchema(Description = "Represents an order item")]
public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal
);

[SwaggerSchema(Description = "DTO for creating an order from a cart")]
public record CreateOrderDto(
    Guid CartId
);

[SwaggerSchema(Description = "DTO for updating order status")]
public record UpdateOrderStatusDto(
    string Status
);

