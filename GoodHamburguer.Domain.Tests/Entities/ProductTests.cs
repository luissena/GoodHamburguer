using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class ProductTests
{
    private Category CreateCategory()
    {
        return Category.Create("Hambúrgueres");
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        var category = CreateCategory();
        var name = "Hambúrguer Clássico";
        var price = 25.50m;
        
        var product = Product.Create(name, category, price);
        
        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(category.Id, product.Category.Id);
        Assert.Equal(price, product.Price);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        
        Assert.Throws<DomainException>(() => Product.Create(string.Empty, category, 25.50m));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        
        Assert.Throws<DomainException>(() => Product.Create(null!, category, 25.50m));
    }

    [Fact]
    public void Create_WithNameExceeding200Characters_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        var longName = new string('A', 201);
        
        Assert.Throws<DomainException>(() => Product.Create(longName, category, 25.50m));
    }

    [Fact]
    public void Create_WithNullCategory_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Product.Create("Produto", null!, 25.50m));
    }

    [Fact]
    public void Create_WithZeroPrice_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        
        Assert.Throws<DomainException>(() => Product.Create("Produto", category, 0m));
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        
        Assert.Throws<DomainException>(() => Product.Create("Produto", category, -10m));
    }

    [Fact]
    public void Create_WithPriceExceeding999999_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        
        Assert.Throws<DomainException>(() => Product.Create("Produto", category, 1000000m));
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        var category = CreateCategory();
        var product = Product.Create("Produto", category, 25.50m);
        var newPrice = 30.00m;
        
        product.UpdatePrice(newPrice);
        
        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var category = CreateCategory();
        var product = Product.Create("Nome Antigo", category, 25.50m);
        
        product.UpdateName("Nome Novo");
        
        Assert.Equal("Nome Novo", product.Name);
    }

    [Fact]
    public void UpdateCategory_WithValidCategory_ShouldUpdateCategory()
    {
        var category1 = CreateCategory();
        var category2 = Category.Create("Bebidas");
        var product = Product.Create("Produto", category1, 25.50m);
        
        product.UpdateCategory(category2);
        
        Assert.Equal(category2.Id, product.Category.Id);
    }

    [Fact]
    public void Create_WithIdAndData_ShouldCreateProductWithSpecificId()
    {
        var id = Guid.NewGuid();
        var category = CreateCategory();
        
        var product = Product.Create(id, "Produto", category, 25.50m);
        
        Assert.Equal(id, product.Id);
    }
}

