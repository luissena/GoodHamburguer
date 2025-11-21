using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Tests.Entities;

public class CartTests
{
    private Product CreateProduct(decimal price = 25.50m)
    {
        var category = Category.Create("Hambúrgueres");
        return Product.Create("Hambúrguer", category, price);
    }

    [Fact]
    public void Create_ShouldCreateEmptyCart()
    {
        var cart = Cart.Create();
        
        Assert.NotEqual(Guid.Empty, cart.Id);
        Assert.Empty(cart.Items);
        Assert.True(cart.IsEmpty());
    }

    [Fact]
    public void AddItem_WithValidProduct_ShouldAddItemToCart()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        
        cart.AddItem(product, 2);
        
        Assert.Single(cart.Items);
        Assert.Equal(2, cart.Items.First().Quantity);
        Assert.Equal(product.Id, cart.Items.First().ProductId);
    }

    [Fact]
    public void AddItem_WithNullProduct_ShouldThrowDomainException()
    {
        var cart = Cart.Create();
        
        Assert.Throws<DomainException>(() => cart.AddItem(null!, 2));
    }

    [Fact]
    public void AddItem_WithSameProductTwice_ShouldIncreaseQuantity()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        
        cart.AddItem(product, 2);
        cart.AddItem(product, 3);
        
        Assert.Single(cart.Items);
        Assert.Equal(5, cart.Items.First().Quantity);
    }

    [Fact]
    public void GetTotal_WithItems_ShouldCalculateCorrectTotal()
    {
        var cart = Cart.Create();
        var product1 = CreateProduct(25.50m);
        var product2 = CreateProduct(15.00m);
        
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);
        
        var total = cart.GetTotal();
        
        Assert.Equal(66.00m, total);
    }

    [Fact]
    public void GetTotal_WithEmptyCart_ShouldReturnZero()
    {
        var cart = Cart.Create();
        
        var total = cart.GetTotal();
        
        Assert.Equal(0m, total);
    }

    [Fact]
    public void GetTotalItems_WithItems_ShouldReturnCorrectCount()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        
        cart.AddItem(product, 3);
        
        var totalItems = cart.GetTotalItems();
        
        Assert.Equal(3, totalItems);
    }

    [Fact]
    public void UpdateItemQuantity_WithValidItem_ShouldUpdateQuantity()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        
        cart.AddItem(product, 2);
        cart.UpdateItemQuantity(product.Id, 5);
        
        Assert.Equal(5, cart.Items.First().Quantity);
    }

    [Fact]
    public void UpdateItemQuantity_WithNonExistentItem_ShouldThrowDomainException()
    {
        var cart = Cart.Create();
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => cart.UpdateItemQuantity(productId, 5));
    }

    [Fact]
    public void RemoveItem_WithValidItem_ShouldRemoveItem()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        
        cart.AddItem(product, 2);
        cart.RemoveItem(product.Id);
        
        Assert.Empty(cart.Items);
    }

    [Fact]
    public void RemoveItem_WithNonExistentItem_ShouldThrowDomainException()
    {
        var cart = Cart.Create();
        var productId = Guid.NewGuid();
        
        Assert.Throws<DomainException>(() => cart.RemoveItem(productId));
    }

    [Fact]
    public void Clear_WithItems_ShouldRemoveAllItems()
    {
        var cart = Cart.Create();
        var product1 = CreateProduct();
        var product2 = CreateProduct(15.00m);
        
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);
        
        cart.Clear();
        
        Assert.Empty(cart.Items);
        Assert.True(cart.IsEmpty());
    }

    [Fact]
    public void ValidateQuantityRules_WithValidCart_ShouldNotThrow()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 1);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test");
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(new[] { rule });
        
        cart.ValidateQuantityRules(validator, id => id == product.Id ? product : null);
        
        Assert.True(true); // If no exception is thrown, test passes
    }

    [Fact]
    public void ValidateQuantityRules_WithInvalidCart_ShouldThrowDomainException()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 3);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test product");
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(new[] { rule });
        
        Assert.Throws<DomainException>(() => 
            cart.ValidateQuantityRules(validator, id => id == product.Id ? product : null));
    }

    [Fact]
    public void ValidateQuantityRules_WithNoRules_ShouldNotThrow()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 10);
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        cart.ValidateQuantityRules(validator, id => id == product.Id ? product : null);
        
        Assert.True(true); // If no exception is thrown, test passes
    }

    [Fact]
    public void ValidateQuantityRules_WithCategoryRule_ShouldValidateByCategory()
    {
        var cart = Cart.Create();
        var categoryId = Guid.NewGuid();
        var category = Category.Create(categoryId, "Category");
        var product1 = Product.Create("Product1", category, 25.50m);
        var product2 = Product.Create("Product2", category, 15.00m);
        cart.AddItem(product1, 1);
        cart.AddItem(product2, 1);
        var rule = OrderQuantityRule.CreateForCategory(categoryId, 1, "category items");
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(new[] { rule });
        
        Assert.Throws<DomainException>(() => 
            cart.ValidateQuantityRules(validator, id => 
                id == product1.Id ? product1 : 
                id == product2.Id ? product2 : null));
    }
}

