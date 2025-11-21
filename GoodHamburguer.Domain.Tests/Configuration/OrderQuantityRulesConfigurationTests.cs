using GoodHamburguer.Domain.Configuration;

namespace GoodHamburguer.Domain.Tests.Configuration;

public class OrderQuantityRulesConfigurationTests
{
    [Fact]
    public void GetDefaultRules_WithValidCategoryAndProducts_ShouldReturnRules()
    {
        var sandwichesCategoryId = Guid.NewGuid();
        var friesProductId = Guid.NewGuid();
        var softDrinkProductId = Guid.NewGuid();
        
        Guid? GetCategoryIdByName(string name) => name == "Sandwiches" ? sandwichesCategoryId : null;
        Guid? GetProductIdByName(string name) => 
            name == "Fries" ? friesProductId : 
            name == "Soft Drink" ? softDrinkProductId : null;
        
        var rules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName).ToList();
        
        Assert.Equal(3, rules.Count);
        
        var sandwichRule = rules.FirstOrDefault(r => r.CategoryId == sandwichesCategoryId);
        Assert.NotNull(sandwichRule);
        Assert.Null(sandwichRule.ProductId);
        Assert.Equal(1, sandwichRule.MaxQuantity);
        Assert.Equal("sandwich", sandwichRule.RuleName);
        
        var friesRule = rules.FirstOrDefault(r => r.ProductId == friesProductId);
        Assert.NotNull(friesRule);
        Assert.Null(friesRule.CategoryId);
        Assert.Equal(1, friesRule.MaxQuantity);
        Assert.Equal("fries", friesRule.RuleName);
        
        var softDrinkRule = rules.FirstOrDefault(r => r.ProductId == softDrinkProductId);
        Assert.NotNull(softDrinkRule);
        Assert.Null(softDrinkRule.CategoryId);
        Assert.Equal(1, softDrinkRule.MaxQuantity);
        Assert.Equal("soft drink", softDrinkRule.RuleName);
    }

    [Fact]
    public void GetDefaultRules_WithMissingCategory_ShouldNotIncludeCategoryRule()
    {
        var friesProductId = Guid.NewGuid();
        var softDrinkProductId = Guid.NewGuid();
        
        Guid? GetCategoryIdByName(string name) => null;
        Guid? GetProductIdByName(string name) => 
            name == "Fries" ? friesProductId : 
            name == "Soft Drink" ? softDrinkProductId : null;
        
        var rules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName).ToList();
        
        Assert.Equal(2, rules.Count);
        Assert.All(rules, r => Assert.NotNull(r.ProductId));
    }

    [Fact]
    public void GetDefaultRules_WithMissingProducts_ShouldNotIncludeProductRules()
    {
        var sandwichesCategoryId = Guid.NewGuid();
        
        Guid? GetCategoryIdByName(string name) => name == "Sandwiches" ? sandwichesCategoryId : null;
        Guid? GetProductIdByName(string name) => null;
        
        var rules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName).ToList();
        
        Assert.Single(rules);
        Assert.Equal(sandwichesCategoryId, rules.First().CategoryId);
    }

    [Fact]
    public void GetDefaultRules_WithAllMissing_ShouldReturnEmpty()
    {
        Guid? GetCategoryIdByName(string name) => null;
        Guid? GetProductIdByName(string name) => null;
        
        var rules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName).ToList();
        
        Assert.Empty(rules);
    }

    [Fact]
    public void GetDefaultRules_WithOnlyFries_ShouldReturnOnlyFriesRule()
    {
        var friesProductId = Guid.NewGuid();
        
        Guid? GetCategoryIdByName(string name) => null;
        Guid? GetProductIdByName(string name) => name == "Fries" ? friesProductId : null;
        
        var rules = OrderQuantityRulesConfiguration.GetDefaultRules(GetCategoryIdByName, GetProductIdByName).ToList();
        
        Assert.Single(rules);
        Assert.Equal(friesProductId, rules.First().ProductId);
        Assert.Equal("fries", rules.First().RuleName);
    }
}

