namespace GoodHamburguer.Client.Models;

public record ProductDto(
    Guid Id,
    string Name,
    CategoryDto Category,
    decimal Price
);

