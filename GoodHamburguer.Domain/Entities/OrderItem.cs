using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() { }

    private OrderItem(Guid id, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        Id = id;
        SetProductId(productId);
        SetProductName(productName);
        SetUnitPrice(unitPrice);
        SetQuantity(quantity);
    }

    public static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        return new OrderItem(Guid.NewGuid(), productId, productName, unitPrice, quantity);
    }

    public static OrderItem Create(Guid id, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        return new OrderItem(id, productId, productName, unitPrice, quantity);
    }

    public decimal GetSubtotal()
    {
        return UnitPrice * Quantity;
    }

    private void SetProductId(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        ProductId = productId;
    }

    private void SetProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("Product name cannot be empty or null.");
        }

        ProductName = productName.Trim();
    }

    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
        {
            throw new DomainException("Unit price must be greater than zero.");
        }

        UnitPrice = unitPrice;
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }
}

