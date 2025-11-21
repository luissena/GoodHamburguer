namespace GoodHamburguer.Client.Models;

public record OrderDto(
    Guid Id,
    IEnumerable<OrderItemDto> Items,
    string Status,
    DateTime CreatedAt,
    decimal Subtotal,
    decimal? DiscountAmount,
    decimal Total
);

