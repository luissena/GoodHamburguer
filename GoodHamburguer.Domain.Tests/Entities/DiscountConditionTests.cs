using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Tests.Entities;

public class DiscountConditionTests
{
    private Product CreateProduct(Guid? productId = null, Guid? categoryId = null)
    {
        var pid = productId ?? Guid.NewGuid();
        var cid = categoryId ?? Guid.NewGuid();
        var category = Category.Create(cid, "Category");
        return Product.Create(pid, "Product", category, 25.50m);
    }

    [Fact]
    public void CreateForProduct_WithValidData_ShouldCreateCondition()
    {
        var productId = Guid.NewGuid();
        var minimumQuantity = 2;
        
        var condition = DiscountCondition.CreateForProduct(productId, minimumQuantity);
        
        Assert.Equal(productId, condition.ProductId);
        Assert.Null(condition.CategoryId);
        Assert.Equal(minimumQuantity, condition.MinimumQuantity);
    }

    [Fact]
    public void CreateForProduct_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForProduct(Guid.Empty, 2));
    }

    [Fact]
    public void CreateForCategory_WithValidData_ShouldCreateCondition()
    {
        var categoryId = Guid.NewGuid();
        var minimumQuantity = 3;
        
        var condition = DiscountCondition.CreateForCategory(categoryId, minimumQuantity);
        
        Assert.Null(condition.ProductId);
        Assert.Equal(categoryId, condition.CategoryId);
        Assert.Equal(minimumQuantity, condition.MinimumQuantity);
    }

    [Fact]
    public void CreateForCategory_WithEmptyCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForCategory(Guid.Empty, 2));
    }

    [Fact]
    public void CreateForProductAndCategory_WithValidData_ShouldCreateCondition()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var minimumQuantity = 1;
        
        var condition = DiscountCondition.CreateForProductAndCategory(productId, categoryId, minimumQuantity);
        
        Assert.Equal(productId, condition.ProductId);
        Assert.Equal(categoryId, condition.CategoryId);
        Assert.Equal(minimumQuantity, condition.MinimumQuantity);
    }

    [Fact]
    public void CreateForProductAndCategory_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForProductAndCategory(Guid.Empty, Guid.NewGuid(), 1));
    }

    [Fact]
    public void CreateForProductAndCategory_WithEmptyCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForProductAndCategory(Guid.NewGuid(), Guid.Empty, 1));
    }

    [Fact]
    public void CreateForProduct_WithZeroMinimumQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForProduct(Guid.NewGuid(), 0));
    }

    [Fact]
    public void CreateForProduct_WithNegativeMinimumQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            DiscountCondition.CreateForProduct(Guid.NewGuid(), -1));
    }

    [Fact]
    public void Matches_WithMatchingProductIdAndSufficientQuantity_ShouldReturnTrue()
    {
        var productId = Guid.NewGuid();
        var condition = DiscountCondition.CreateForProduct(productId, 2);
        var cartItem = CartItem.Create(productId, "Product", 25.50m, 2);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, _ => null);
        
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithMatchingProductIdButInsufficientQuantity_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var condition = DiscountCondition.CreateForProduct(productId, 3);
        var cartItem = CartItem.Create(productId, "Product", 25.50m, 2);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, _ => null);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithNonMatchingProductId_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        var condition = DiscountCondition.CreateForProduct(productId, 1);
        var cartItem = CartItem.Create(otherProductId, "Product", 25.50m, 1);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, _ => null);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithMatchingCategoryIdAndSufficientQuantity_ShouldReturnTrue()
    {
        var categoryId = Guid.NewGuid();
        var product = CreateProduct(categoryId: categoryId);
        var condition = DiscountCondition.CreateForCategory(categoryId, 2);
        var cartItem = CartItem.Create(product.Id, "Product", 25.50m, 2);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, id => id == product.Id ? product : null);
        
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithMatchingCategoryIdButInsufficientQuantity_ShouldReturnFalse()
    {
        var categoryId = Guid.NewGuid();
        var product = CreateProduct(categoryId: categoryId);
        var condition = DiscountCondition.CreateForCategory(categoryId, 3);
        var cartItem = CartItem.Create(product.Id, "Product", 25.50m, 2);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, id => id == product.Id ? product : null);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithProductAndCategoryBothMatching_ShouldReturnTrue()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = CreateProduct(productId, categoryId);
        var condition = DiscountCondition.CreateForProductAndCategory(productId, categoryId, 1);
        var cartItem = CartItem.Create(productId, "Product", 25.50m, 1);
        var otherProduct = CreateProduct(categoryId: categoryId);
        var otherCartItem = CartItem.Create(otherProduct.Id, "Other", 15.00m, 1);
        var items = new[] { cartItem, otherCartItem };
        
        var matches = condition.Matches(items, id => 
            id == product.Id ? product : 
            id == otherProduct.Id ? otherProduct : null);
        
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithProductAndCategoryButProductNotMatching_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        var product = CreateProduct(otherProductId, categoryId);
        var condition = DiscountCondition.CreateForProductAndCategory(productId, categoryId, 1);
        var cartItem = CartItem.Create(otherProductId, "Product", 25.50m, 1);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, id => id == product.Id ? product : null);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithNoMatchingProductOrCategory_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        var condition = DiscountCondition.CreateForProduct(productId, 1);
        var cartItem = CartItem.Create(otherProductId, "Product", 25.50m, 1);
        var items = new[] { cartItem };
        
        var matches = condition.Matches(items, _ => null);
        
        Assert.False(matches);
    }
}

