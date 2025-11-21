using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class CartItemTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCartItem()
    {
        var productId = Guid.NewGuid();
        var productName = "Hambúrguer";
        var unitPrice = 25.50m;
        var quantity = 2;
        
        var cartItem = CartItem.Create(productId, productName, unitPrice, quantity);
        
        Assert.NotEqual(Guid.Empty, cartItem.Id);
        Assert.Equal(productId, cartItem.ProductId);
        Assert.Equal(productName, cartItem.ProductName);
        Assert.Equal(unitPrice, cartItem.UnitPrice);
        Assert.Equal(quantity, cartItem.Quantity);
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => CartItem.Create(Guid.Empty, "Produto", 25.50m, 2));
    }

    [Fact]
    public void Create_WithEmptyProductName_ShouldThrowDomainException()
    {
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => CartItem.Create(productId, string.Empty, 25.50m, 2));
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrowDomainException()
    {
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => CartItem.Create(productId, "Produto", 0m, 2));
    }

    [Fact]
    public void Create_WithZeroQuantity_ShouldThrowDomainException()
    {
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => CartItem.Create(productId, "Produto", 25.50m, 0));
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrowDomainException()
    {
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => CartItem.Create(productId, "Produto", 25.50m, -1));
    }

    [Fact]
    public void Create_WithQuantityExceeding999_ShouldThrowDomainException()
    {
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => CartItem.Create(productId, "Produto", 25.50m, 1000));
    }

    [Fact]
    public void GetSubtotal_ShouldCalculateCorrectly()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 3);
        
        var subtotal = cartItem.GetSubtotal();
        
        Assert.Equal(76.50m, subtotal);
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateQuantity()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 2);
        
        cartItem.UpdateQuantity(5);
        
        Assert.Equal(5, cartItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_WithValidAmount_ShouldIncreaseQuantity()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 2);
        
        cartItem.IncreaseQuantity(3);
        
        Assert.Equal(5, cartItem.Quantity);
    }

    [Fact]
    public void IncreaseQuantity_WithZeroAmount_ShouldThrowDomainException()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 2);
        
        Assert.Throws<DomainException>(() => cartItem.IncreaseQuantity(0));
    }

    [Fact]
    public void IncreaseQuantity_WithNegativeAmount_ShouldThrowDomainException()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 2);
        
        Assert.Throws<DomainException>(() => cartItem.IncreaseQuantity(-1));
    }
}

