using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateCategory()
    {
        var name = "Hambúrgueres";
        
        var category = Category.Create(name);
        
        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Category.Create(string.Empty));
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Category.Create("   "));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => Category.Create(null!));
    }

    [Fact]
    public void Create_WithNameExceeding100Characters_ShouldThrowDomainException()
    {
        var longName = new string('A', 101);
        Assert.Throws<DomainException>(() => Category.Create(longName));
    }

    [Fact]
    public void Create_WithIdAndName_ShouldCreateCategoryWithSpecificId()
    {
        var id = Guid.NewGuid();
        var name = "Bebidas";
        
        var category = Category.Create(id, name);
        
        Assert.Equal(id, category.Id);
        Assert.Equal(name, category.Name);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        var category = Category.Create("Antiga");
        var newName = "Nova";
        
        category.UpdateName(newName);
        
        Assert.Equal(newName, category.Name);
    }

    [Fact]
    public void UpdateName_WithEmptyName_ShouldThrowDomainException()
    {
        var category = Category.Create("Válida");
        
        Assert.Throws<DomainException>(() => category.UpdateName(string.Empty));
    }

    [Fact]
    public void Create_WithNameWithWhitespace_ShouldTrim()
    {
        var category = Category.Create("  Nome  ");
        
        Assert.Equal("Nome", category.Name);
    }
}

