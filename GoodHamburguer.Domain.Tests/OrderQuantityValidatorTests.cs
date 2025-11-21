using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Tests;

public class OrderQuantityValidatorTests
{
    private Product CreateProduct(Guid? productId = null, Guid? categoryId = null)
    {
        var pid = productId ?? Guid.NewGuid();
        var cid = categoryId ?? Guid.NewGuid();
        var category = Category.Create(cid, "Category");
        return Product.Create(pid, "Product", category, 25.50m);
    }

    [Fact]
    public void ValidateCart_WithNullCart_ShouldReturnFailure()
    {
        var validator = new OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        var result = validator.ValidateCart(null!, _ => null);
        
        Assert.False(result.IsValid);
        Assert.Contains("Cart cannot be null", result.Errors.First());
    }

    [Fact]
    public void ValidateCart_WithNoRules_ShouldReturnSuccess()
    {
        var cart = Cart.Create();
        var validator = new OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        var result = validator.ValidateCart(cart, _ => null);
        
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateCart_WithValidCart_ShouldReturnSuccess()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 1);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateCart(cart, id => id == product.Id ? product : null);
        
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateCart_WithExceedingQuantity_ShouldReturnFailure()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 3);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test product");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateCart(cart, id => id == product.Id ? product : null);
        
        Assert.False(result.IsValid);
        Assert.Contains("test product", result.Errors.First());
    }

    [Fact]
    public void ValidateCart_WithCategoryRule_ShouldValidateByCategory()
    {
        var cart = Cart.Create();
        var categoryId = Guid.NewGuid();
        var product1 = CreateProduct(categoryId: categoryId);
        var product2 = CreateProduct(categoryId: categoryId);
        cart.AddItem(product1, 1);
        cart.AddItem(product2, 1);
        var rule = OrderQuantityRule.CreateForCategory(categoryId, 1, "category items");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateCart(cart, id => 
            id == product1.Id ? product1 : 
            id == product2.Id ? product2 : null);
        
        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateCart_WithMultipleRules_ShouldValidateAll()
    {
        var cart = Cart.Create();
        var product1 = CreateProduct();
        var product2 = CreateProduct();
        cart.AddItem(product1, 3);
        cart.AddItem(product2, 1);
        var rule1 = OrderQuantityRule.CreateForProduct(product1.Id, 2, "product1");
        var rule2 = OrderQuantityRule.CreateForProduct(product2.Id, 1, "product2");
        var validator = new OrderQuantityValidator(new[] { rule1, rule2 });
        
        var result = validator.ValidateCart(cart, id => 
            id == product1.Id ? product1 : 
            id == product2.Id ? product2 : null);
        
        Assert.False(result.IsValid);
        Assert.Contains("product1", result.Errors.First());
    }

    [Fact]
    public void ValidateOrder_WithNullOrder_ShouldReturnFailure()
    {
        var validator = new OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        var result = validator.ValidateOrder(null!, _ => null);
        
        Assert.False(result.IsValid);
        Assert.Contains("Order cannot be null", result.Errors.First());
    }

    [Fact]
    public void ValidateOrder_WithNoRules_ShouldReturnSuccess()
    {
        var order = Order.Create();
        var validator = new OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        var result = validator.ValidateOrder(order, _ => null);
        
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateOrder_WithValidOrder_ShouldReturnSuccess()
    {
        var order = Order.Create();
        var product = CreateProduct();
        var orderItem = OrderItem.Create(product.Id, "Product", 25.50m, 1);
        order.AddItem(orderItem);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateOrder(order, id => id == product.Id ? product : null);
        
        Assert.True(result.IsValid);
    }

    [Fact]
    public void ValidateOrder_WithExceedingQuantity_ShouldReturnFailure()
    {
        var order = Order.Create();
        var product = CreateProduct();
        var orderItem = OrderItem.Create(product.Id, "Product", 25.50m, 3);
        order.AddItem(orderItem);
        var rule = OrderQuantityRule.CreateForProduct(product.Id, 2, "test product");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateOrder(order, id => id == product.Id ? product : null);
        
        Assert.False(result.IsValid);
        Assert.Contains("test product", result.Errors.First());
    }

    [Fact]
    public void ValidateOrder_WithCategoryRule_ShouldValidateByCategory()
    {
        var order = Order.Create();
        var categoryId = Guid.NewGuid();
        var product1 = CreateProduct(categoryId: categoryId);
        var product2 = CreateProduct(categoryId: categoryId);
        order.AddItem(OrderItem.Create(product1.Id, "Product1", 25.50m, 1));
        order.AddItem(OrderItem.Create(product2.Id, "Product2", 15.00m, 1));
        var rule = OrderQuantityRule.CreateForCategory(categoryId, 1, "category items");
        var validator = new OrderQuantityValidator(new[] { rule });
        
        var result = validator.ValidateOrder(order, id => 
            id == product1.Id ? product1 : 
            id == product2.Id ? product2 : null);
        
        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateCart_WithNullRules_ShouldUseEmptyRules()
    {
        var cart = Cart.Create();
        var validator = new OrderQuantityValidator(null!);
        
        var result = validator.ValidateCart(cart, _ => null);
        
        Assert.True(result.IsValid);
    }
}

