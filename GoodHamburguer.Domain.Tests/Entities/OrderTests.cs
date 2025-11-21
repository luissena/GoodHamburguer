using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain;
using Xunit;

namespace GoodHamburguer.Domain.Tests.Entities;

public class OrderTests
{
    private Product CreateProduct(decimal price = 25.50m)
    {
        var category = Category.Create("Sandwiches");
        return Product.Create("Hamburger", category, price);
    }

    private Cart CreateCartWithItems()
    {
        var cart = Cart.Create();
        var product1 = CreateProduct(25.50m);
        var product2 = CreateProduct(15.00m);
        cart.AddItem(product1, 2);
        cart.AddItem(product2, 1);
        return cart;
    }

    [Fact]
    public void Create_ShouldCreateOrderWithPendingStatus()
    {
        var order = Order.Create();
        
        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.Subtotal);
        Assert.Null(order.DiscountAmount);
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void Create_WithIdAndDate_ShouldCreateOrderWithSpecificValues()
    {
        var id = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-1);
        
        var order = Order.Create(id, createdAt);
        
        Assert.Equal(id, order.Id);
        Assert.Equal(createdAt, order.CreatedAt);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void CreateFromCart_WithValidCart_ShouldCreateOrderWithItems()
    {
        var cart = CreateCartWithItems();
        
        var order = Order.CreateFromCart(cart);
        
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(66.00m, order.Subtotal);
        Assert.Equal(66.00m, order.Total);
    }

    [Fact]
    public void CreateFromCart_WithDiscount_ShouldApplyDiscount()
    {
        var cart = CreateCartWithItems();
        var discountAmount = 10.00m;
        
        var order = Order.CreateFromCart(cart, discountAmount);
        
        Assert.Equal(66.00m, order.Subtotal);
        Assert.Equal(10.00m, order.DiscountAmount);
        Assert.Equal(56.00m, order.Total);
    }

    [Fact]
    public void CreateFromCart_WithValidatorAndValidCart_ShouldCreateOrder()
    {
        var cart = CreateCartWithItems();
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(Enumerable.Empty<OrderQuantityRule>());
        
        var order = Order.CreateFromCart(cart, 0, validator, _ => null);
        
        Assert.NotNull(order);
        Assert.Equal(2, order.Items.Count);
    }

    [Fact]
    public void CreateFromCart_WithValidatorAndInvalidCart_ShouldThrowDomainException()
    {
        var cart = Cart.Create();
        var category = Category.Create("Sandwiches");
        var product1 = Product.Create("Product1", category, 25.50m);
        var product2 = Product.Create("Product2", category, 15.00m);
        cart.AddItem(product1, 1);
        cart.AddItem(product2, 1); // Total of 2 items from same category, but rule allows only 1
        var rule = OrderQuantityRule.CreateForCategory(category.Id, 1, "test");
        var validator = new GoodHamburguer.Domain.OrderQuantityValidator(new[] { rule });
        
        Assert.Throws<DomainException>(() => 
            Order.CreateFromCart(cart, 0, validator, id => 
                id == product1.Id ? product1 : 
                id == product2.Id ? product2 : null));
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddItem()
    {
        var order = Order.Create();
        var orderItem = OrderItem.Create(Guid.NewGuid(), "Product", 25.50m, 2);
        
        order.AddItem(orderItem);
        
        Assert.Single(order.Items);
        Assert.Equal(51.00m, order.Subtotal);
        Assert.Equal(51.00m, order.Total);
    }

    [Fact]
    public void AddItem_WithNullItem_ShouldThrowDomainException()
    {
        var order = Order.Create();
        
        Assert.Throws<DomainException>(() => order.AddItem(null!));
    }

    [Fact]
    public void AddItem_WithMultipleItems_ShouldRecalculateTotals()
    {
        var order = Order.Create();
        var item1 = OrderItem.Create(Guid.NewGuid(), "Product1", 25.50m, 2);
        var item2 = OrderItem.Create(Guid.NewGuid(), "Product2", 15.00m, 1);
        
        order.AddItem(item1);
        order.AddItem(item2);
        
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(66.00m, order.Subtotal);
        Assert.Equal(66.00m, order.Total);
    }

    [Fact]
    public void UpdateStatus_WithValidStatus_ShouldUpdateStatus()
    {
        var order = Order.Create();
        
        order.UpdateStatus(OrderStatus.Confirmed);
        
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void UpdateStatus_FromPendingToReady_ShouldUpdateStatus()
    {
        var order = Order.Create();
        
        order.UpdateStatus(OrderStatus.Confirmed);
        order.UpdateStatus(OrderStatus.Preparing);
        order.UpdateStatus(OrderStatus.Ready);
        
        Assert.Equal(OrderStatus.Ready, order.Status);
    }

    [Fact]
    public void UpdateStatus_FromDelivered_ShouldThrowDomainException()
    {
        var order = Order.Create();
        order.UpdateStatus(OrderStatus.Confirmed);
        order.UpdateStatus(OrderStatus.Preparing);
        order.UpdateStatus(OrderStatus.Ready);
        order.UpdateStatus(OrderStatus.Delivered);
        
        Assert.Throws<DomainException>(() => order.UpdateStatus(OrderStatus.Preparing));
    }

    [Fact]
    public void UpdateStatus_FromCancelled_ShouldThrowDomainException()
    {
        var order = Order.Create();
        order.Cancel();
        
        Assert.Throws<DomainException>(() => order.UpdateStatus(OrderStatus.Confirmed));
    }

    [Fact]
    public void Cancel_WithPendingOrder_ShouldSetStatusToCancelled()
    {
        var order = Order.Create();
        
        order.Cancel();
        
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WithDeliveredOrder_ShouldThrowDomainException()
    {
        var order = Order.Create();
        order.UpdateStatus(OrderStatus.Confirmed);
        order.UpdateStatus(OrderStatus.Preparing);
        order.UpdateStatus(OrderStatus.Ready);
        order.UpdateStatus(OrderStatus.Delivered);
        
        Assert.Throws<DomainException>(() => order.Cancel());
    }

    [Fact]
    public void CreateFromCart_WithDiscount_ShouldRecalculateTotalCorrectly()
    {
        var cart = CreateCartWithItems();
        var discountAmount = 5.50m;
        
        var order = Order.CreateFromCart(cart, discountAmount);
        
        Assert.Equal(66.00m, order.Subtotal);
        Assert.Equal(5.50m, order.DiscountAmount);
        Assert.Equal(60.50m, order.Total);
    }

    [Fact]
    public void AddItem_WithDiscount_ShouldRecalculateTotalCorrectly()
    {
        var cart = Cart.Create();
        var product = CreateProduct(25.50m);
        cart.AddItem(product, 2);
        var discountAmount = 10.00m;
        var order = Order.CreateFromCart(cart, discountAmount);
        var item = OrderItem.Create(Guid.NewGuid(), "Product2", 15.00m, 1);
        
        order.AddItem(item);
        
        Assert.Equal(66.00m, order.Subtotal);
        Assert.Equal(10.00m, order.DiscountAmount);
        Assert.Equal(56.00m, order.Total);
    }
}

