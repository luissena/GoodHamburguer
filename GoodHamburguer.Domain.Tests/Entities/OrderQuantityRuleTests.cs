using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class OrderQuantityRuleTests
{
    [Fact]
    public void CreateForProduct_WithValidData_ShouldCreateRule()
    {
        var productId = Guid.NewGuid();
        var maxQuantity = 2;
        var ruleName = "test rule";
        
        var rule = OrderQuantityRule.CreateForProduct(productId, maxQuantity, ruleName);
        
        Assert.Equal(productId, rule.ProductId);
        Assert.Null(rule.CategoryId);
        Assert.Equal(maxQuantity, rule.MaxQuantity);
        Assert.Equal(ruleName, rule.RuleName);
    }

    [Fact]
    public void CreateForProduct_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.Empty, 2, "test"));
    }

    [Fact]
    public void CreateForCategory_WithValidData_ShouldCreateRule()
    {
        var categoryId = Guid.NewGuid();
        var maxQuantity = 1;
        var ruleName = "category rule";
        
        var rule = OrderQuantityRule.CreateForCategory(categoryId, maxQuantity, ruleName);
        
        Assert.Null(rule.ProductId);
        Assert.Equal(categoryId, rule.CategoryId);
        Assert.Equal(maxQuantity, rule.MaxQuantity);
        Assert.Equal(ruleName, rule.RuleName);
    }

    [Fact]
    public void CreateForCategory_WithEmptyCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForCategory(Guid.Empty, 1, "test"));
    }

    [Fact]
    public void Create_WithProductId_ShouldCreateRule()
    {
        var productId = Guid.NewGuid();
        
        var rule = OrderQuantityRule.Create(productId, null, 2, "test");
        
        Assert.Equal(productId, rule.ProductId);
        Assert.Null(rule.CategoryId);
    }

    [Fact]
    public void Create_WithCategoryId_ShouldCreateRule()
    {
        var categoryId = Guid.NewGuid();
        
        var rule = OrderQuantityRule.Create(null, categoryId, 1, "test");
        
        Assert.Null(rule.ProductId);
        Assert.Equal(categoryId, rule.CategoryId);
    }

    [Fact]
    public void Create_WithBothProductAndCategoryId_ShouldCreateRule()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        var rule = OrderQuantityRule.Create(productId, categoryId, 1, "test");
        
        Assert.Equal(productId, rule.ProductId);
        Assert.Equal(categoryId, rule.CategoryId);
    }

    [Fact]
    public void Create_WithNeitherProductNorCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.Create(null, null, 1, "test"));
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.Create(Guid.Empty, null, 1, "test"));
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.Create(null, Guid.Empty, 1, "test"));
    }

    [Fact]
    public void Create_WithZeroMaxQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 0, "test"));
    }

    [Fact]
    public void Create_WithNegativeMaxQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.NewGuid(), -1, "test"));
    }

    [Fact]
    public void Create_WithEmptyRuleName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 1, string.Empty));
    }

    [Fact]
    public void Create_WithNullRuleName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 1, null!));
    }

    [Fact]
    public void Create_WithWhitespaceRuleName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 1, "   "));
    }

    [Fact]
    public void Matches_WithMatchingProductId_ShouldReturnTrue()
    {
        var productId = Guid.NewGuid();
        var category = Category.Create("Category");
        var product = Product.Create(productId, "Product", category, 25.50m);
        var rule = OrderQuantityRule.CreateForProduct(productId, 1, "test");
        
        var matches = rule.Matches(product);
        
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithNonMatchingProductId_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var otherProductId = Guid.NewGuid();
        var category = Category.Create("Category");
        var product = Product.Create(otherProductId, "Product", category, 25.50m);
        var rule = OrderQuantityRule.CreateForProduct(productId, 1, "test");
        
        var matches = rule.Matches(product);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithMatchingCategoryId_ShouldReturnTrue()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create(categoryId, "Category");
        var product = Product.Create("Product", category, 25.50m);
        var rule = OrderQuantityRule.CreateForCategory(categoryId, 1, "test");
        
        var matches = rule.Matches(product);
        
        Assert.True(matches);
    }

    [Fact]
    public void Matches_WithNonMatchingCategoryId_ShouldReturnFalse()
    {
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();
        var category = Category.Create(otherCategoryId, "Category");
        var product = Product.Create("Product", category, 25.50m);
        var rule = OrderQuantityRule.CreateForCategory(categoryId, 1, "test");
        
        var matches = rule.Matches(product);
        
        Assert.False(matches);
    }

    [Fact]
    public void Matches_WithNullProduct_ShouldReturnFalse()
    {
        var rule = OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 1, "test");
        
        var matches = rule.Matches(null!);
        
        Assert.False(matches);
    }

    [Fact]
    public void Create_WithRuleNameWithWhitespace_ShouldTrim()
    {
        var rule = OrderQuantityRule.CreateForProduct(Guid.NewGuid(), 1, "  test rule  ");
        
        Assert.Equal("test rule", rule.RuleName);
    }
}

