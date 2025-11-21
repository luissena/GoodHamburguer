using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class DiscountTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateDiscount()
    {
        var name = "Desconto 50%";
        var percentage = 50m;
        
        var discount = Discount.Create(name, percentage);
        
        Assert.NotEqual(Guid.Empty, discount.Id);
        Assert.Equal(name, discount.Name);
        Assert.Equal(percentage, discount.Percentage);
        Assert.True(discount.IsActive);
        Assert.Empty(discount.Conditions);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Discount.Create(string.Empty, 50m));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Discount.Create(null!, 50m));
    }

    [Fact]
    public void Create_WithNameExceeding200Characters_ShouldThrowDomainException()
    {
        var longName = new string('A', 201);
        Assert.Throws<DomainException>(() => Discount.Create(longName, 50m));
    }

    [Fact]
    public void Create_WithZeroPercentage_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Discount.Create("Desconto", 0m));
    }

    [Fact]
    public void Create_WithNegativePercentage_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Discount.Create("Desconto", -10m));
    }

    [Fact]
    public void Create_WithPercentageExceeding100_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Discount.Create("Desconto", 101m));
    }

    [Fact]
    public void AddCondition_WithValidCondition_ShouldAddCondition()
    {
        var discount = Discount.Create("Desconto", 50m);
        var condition = DiscountCondition.CreateForProduct(Guid.NewGuid(), 2);
        
        discount.AddCondition(condition);
        
        Assert.Single(discount.Conditions);
    }

    [Fact]
    public void AddCondition_WithNullCondition_ShouldThrowDomainException()
    {
        var discount = Discount.Create("Desconto", 50m);
        
        Assert.Throws<DomainException>(() => discount.AddCondition(null!));
    }

    [Fact]
    public void RemoveCondition_WithValidCondition_ShouldRemoveCondition()
    {
        var discount = Discount.Create("Desconto", 50m);
        var condition = DiscountCondition.CreateForProduct(Guid.NewGuid(), 2);
        
        discount.AddCondition(condition);
        discount.RemoveCondition(condition);
        
        Assert.Empty(discount.Conditions);
    }

    [Fact]
    public void ClearConditions_ShouldRemoveAllConditions()
    {
        var discount = Discount.Create("Desconto", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(Guid.NewGuid(), 2));
        discount.AddCondition(DiscountCondition.CreateForCategory(Guid.NewGuid(), 3));
        
        discount.ClearConditions();
        
        Assert.Empty(discount.Conditions);
    }

    [Fact]
    public void CalculateDiscount_WithValidTotal_ShouldCalculateCorrectly()
    {
        var discount = Discount.Create("Desconto 50%", 50m);
        var total = 100m;
        
        var discountAmount = discount.CalculateDiscount(total);
        
        Assert.Equal(50m, discountAmount);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var discount = Discount.Create("Desconto", 50m);
        discount.Deactivate();
        
        discount.Activate();
        
        Assert.True(discount.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var discount = Discount.Create("Desconto", 50m);
        
        discount.Deactivate();
        
        Assert.False(discount.IsActive);
    }

    [Fact]
    public void IsApplicable_WithActiveDiscountAndMatchingConditions_ShouldReturnTrue()
    {
        var productId = Guid.NewGuid();
        var category = Category.Create("Categoria");
        var product = Product.Create(productId, "Produto", category, 25.50m);
        var cartItem = CartItem.Create(productId, "Produto", 25.50m, 2);
        var items = new[] { cartItem };
        var discount = Discount.Create("Desconto", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(productId, 2));
        
        bool isApplicable = discount.IsApplicable(items, id => id == productId ? product : null);
        
        Assert.True(isApplicable);
    }

    [Fact]
    public void IsApplicable_WithInactiveDiscount_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var category = Category.Create("Categoria");
        var product = Product.Create(productId, "Produto", category, 25.50m);
        var cartItem = CartItem.Create(productId, "Produto", 25.50m, 2);
        var items = new[] { cartItem };
        var discount = Discount.Create("Desconto", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(productId, 2));
        discount.Deactivate();
        
        bool isApplicable = discount.IsApplicable(items, id => id == productId ? product : null);
        
        Assert.False(isApplicable);
    }

    [Fact]
    public void IsApplicable_WithNoConditions_ShouldReturnFalse()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), "Produto", 25.50m, 2);
        var items = new[] { cartItem };
        var discount = Discount.Create("Desconto", 50m);
        
        bool isApplicable = discount.IsApplicable(items, _ => null);
        
        Assert.False(isApplicable);
    }

    [Fact]
    public void UpdatePercentage_WithValidPercentage_ShouldUpdatePercentage()
    {
        var discount = Discount.Create("Desconto", 50m);
        
        discount.UpdatePercentage(75m);
        
        Assert.Equal(75m, discount.Percentage);
    }
}

