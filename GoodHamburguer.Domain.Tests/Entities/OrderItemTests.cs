using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Tests.Entities;

public class OrderItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrderItem()
    {
        var productId = Guid.NewGuid();
        var productName = "Hamburger";
        var unitPrice = 25.50m;
        var quantity = 2;
        
        var orderItem = OrderItem.Create(productId, productName, unitPrice, quantity);
        
        Assert.NotEqual(Guid.Empty, orderItem.Id);
        Assert.Equal(productId, orderItem.ProductId);
        Assert.Equal(productName, orderItem.ProductName);
        Assert.Equal(unitPrice, orderItem.UnitPrice);
        Assert.Equal(quantity, orderItem.Quantity);
    }

    [Fact]
    public void Create_WithIdAndData_ShouldCreateOrderItemWithSpecificId()
    {
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        
        var orderItem = OrderItem.Create(id, productId, "Product", 25.50m, 2);
        
        Assert.Equal(id, orderItem.Id);
        Assert.Equal(productId, orderItem.ProductId);
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.Empty, "Product", 25.50m, 2));
    }

    [Fact]
    public void Create_WithEmptyProductName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), string.Empty, 25.50m, 2));
    }

    [Fact]
    public void Create_WithNullProductName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), null!, 25.50m, 2));
    }

    [Fact]
    public void Create_WithWhitespaceProductName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), "   ", 25.50m, 2));
    }

    [Fact]
    public void Create_WithZeroUnitPrice_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), "Product", 0m, 2));
    }

    [Fact]
    public void Create_WithNegativeUnitPrice_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), "Product", -10m, 2));
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), "Product", 25.50m, 0));
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderItem.Create(Guid.NewGuid(), "Product", 25.50m, -1));
    }

    [Fact]
    public void GetSubtotal_ShouldCalculateCorrectly()
    {
        var orderItem = OrderItem.Create(Guid.NewGuid(), "Product", 25.50m, 3);
        
        var subtotal = orderItem.GetSubtotal();
        
        Assert.Equal(76.50m, subtotal);
    }

    [Fact]
    public void Create_WithProductNameWithWhitespace_ShouldTrim()
    {
        var orderItem = OrderItem.Create(Guid.NewGuid(), "  Product  ", 25.50m, 2);
        
        Assert.Equal("Product", orderItem.ProductName);
    }
}

