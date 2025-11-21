using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class OrderQuantityRuleEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateEntity()
    {
        var productId = Guid.NewGuid();
        var maxQuantity = 2;
        var ruleName = "test rule";
        
        var entity = OrderQuantityRuleEntity.Create(productId, null, maxQuantity, ruleName);
        
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal(productId, entity.ProductId);
        Assert.Null(entity.CategoryId);
        Assert.Equal(maxQuantity, entity.MaxQuantity);
        Assert.Equal(ruleName, entity.RuleName);
    }

    [Fact]
    public void Create_WithIdAndData_ShouldCreateEntityWithSpecificId()
    {
        var id = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        var entity = OrderQuantityRuleEntity.Create(id, null, categoryId, 1, "test");
        
        Assert.Equal(id, entity.Id);
        Assert.Equal(categoryId, entity.CategoryId);
    }

    [Fact]
    public void Create_WithNeitherProductNorCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(null, null, 1, "test"));
    }

    [Fact]
    public void Create_WithEmptyProductId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(Guid.Empty, null, 1, "test"));
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(null, Guid.Empty, 1, "test"));
    }

    [Fact]
    public void Create_WithZeroMaxQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 0, "test"));
    }

    [Fact]
    public void Create_WithNegativeMaxQuantity_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, -1, "test"));
    }

    [Fact]
    public void Create_WithEmptyRuleName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, string.Empty));
    }

    [Fact]
    public void Create_WithNullRuleName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => 
            OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, null!));
    }

    [Fact]
    public void Update_WithProductId_ShouldUpdateProductId()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "test");
        var newProductId = Guid.NewGuid();
        
        entity.Update(newProductId, null, null, null);
        
        Assert.Equal(newProductId, entity.ProductId);
    }

    [Fact]
    public void Update_WithCategoryId_ShouldUpdateCategoryId()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "test");
        var newCategoryId = Guid.NewGuid();
        
        entity.Update(null, newCategoryId, null, null);
        
        Assert.Equal(newCategoryId, entity.CategoryId);
    }

    [Fact]
    public void Update_WithMaxQuantity_ShouldUpdateMaxQuantity()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "test");
        
        entity.Update(null, null, 5, null);
        
        Assert.Equal(5, entity.MaxQuantity);
    }

    [Fact]
    public void Update_WithRuleName_ShouldUpdateRuleName()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "old name");
        
        entity.Update(null, null, null, "new name");
        
        Assert.Equal("new name", entity.RuleName);
    }

    [Fact]
    public void Update_WithAllFields_ShouldUpdateAllFields()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "test");
        var newProductId = Guid.NewGuid();
        var newCategoryId = Guid.NewGuid();
        
        entity.Update(newProductId, newCategoryId, 3, "updated");
        
        Assert.Equal(newProductId, entity.ProductId);
        Assert.Equal(newCategoryId, entity.CategoryId);
        Assert.Equal(3, entity.MaxQuantity);
        Assert.Equal("updated", entity.RuleName);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldThrowDomainException()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "test");
        
        Assert.Throws<DomainException>(() => 
            entity.Update(Guid.Empty, Guid.Empty, null, null));
    }

    [Fact]
    public void ToValueObject_ShouldConvertToOrderQuantityRule()
    {
        var productId = Guid.NewGuid();
        var entity = OrderQuantityRuleEntity.Create(productId, null, 2, "test");
        
        var valueObject = entity.ToValueObject();
        
        Assert.Equal(productId, valueObject.ProductId);
        Assert.Null(valueObject.CategoryId);
        Assert.Equal(2, valueObject.MaxQuantity);
        Assert.Equal("test", valueObject.RuleName);
    }

    [Fact]
    public void ToValueObject_WithCategoryId_ShouldConvertCorrectly()
    {
        var categoryId = Guid.NewGuid();
        var entity = OrderQuantityRuleEntity.Create(null, categoryId, 1, "category rule");
        
        var valueObject = entity.ToValueObject();
        
        Assert.Null(valueObject.ProductId);
        Assert.Equal(categoryId, valueObject.CategoryId);
        Assert.Equal(1, valueObject.MaxQuantity);
        Assert.Equal("category rule", valueObject.RuleName);
    }

    [Fact]
    public void Create_WithRuleNameWithWhitespace_ShouldTrim()
    {
        var entity = OrderQuantityRuleEntity.Create(Guid.NewGuid(), null, 1, "  test rule  ");
        
        Assert.Equal("test rule", entity.RuleName);
    }
}

