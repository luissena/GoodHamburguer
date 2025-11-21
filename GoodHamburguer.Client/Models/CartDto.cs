namespace GoodHamburguer.Client.Models;

public record CartDto(
    Guid Id,
    IEnumerable<CartItemDto> Items,
    decimal Subtotal,
    decimal? DiscountAmount,
    string? AppliedDiscountName,
    decimal Total,
    int TotalItems
);

