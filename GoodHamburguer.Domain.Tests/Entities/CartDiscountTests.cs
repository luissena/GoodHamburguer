using GoodHamburguer.Domain.Entities;

namespace GoodHamburguer.Domain.Tests.Entities;

public class CartDiscountTests
{
    private Product CreateProduct(Guid? productId = null, Guid? categoryId = null, decimal price = 25.50m)
    {
        var id = productId ?? Guid.NewGuid();
        var category = Category.Create(categoryId ?? Guid.NewGuid(), "Categoria");
        return Product.Create(id, "Produto", category, price);
    }

    [Fact]
    public void GetBestApplicableDiscount_WithMultipleDiscounts_ShouldReturnHighestPercentage()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 2);

        var discount30 = Discount.Create("Desconto 30%", 30m);
        discount30.AddCondition(DiscountCondition.CreateForProduct(product.Id, 2));

        var discount50 = Discount.Create("Desconto 50%", 50m);
        discount50.AddCondition(DiscountCondition.CreateForProduct(product.Id, 2));

        var availableDiscounts = new[] { discount30, discount50 };

        var bestDiscount = cart.GetBestApplicableDiscount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.NotNull(bestDiscount);
        Assert.Equal(50m, bestDiscount.Percentage);
    }

    [Fact]
    public void GetBestApplicableDiscount_WithNoApplicableDiscounts_ShouldReturnNull()
    {
        var cart = Cart.Create();
        var product = CreateProduct();
        cart.AddItem(product, 1);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(product.Id, 2));

        var availableDiscounts = new[] { discount };

        var bestDiscount = cart.GetBestApplicableDiscount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.Null(bestDiscount);
    }

    [Fact]
    public void GetTotalWithDiscount_WithApplicableDiscount_ShouldApplyDiscount()
    {
        var cart = Cart.Create();
        var product = CreateProduct(price: 100m);
        cart.AddItem(product, 1);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(product.Id, 1));

        var availableDiscounts = new[] { discount };

        var totalWithDiscount = cart.GetTotalWithDiscount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.Equal(50m, totalWithDiscount);
    }

    [Fact]
    public void GetTotalWithDiscount_WithNoApplicableDiscount_ShouldReturnOriginalTotal()
    {
        var cart = Cart.Create();
        var product = CreateProduct(price: 100m);
        cart.AddItem(product, 1);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(Guid.NewGuid(), 1));

        var availableDiscounts = new[] { discount };

        var totalWithDiscount = cart.GetTotalWithDiscount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.Equal(100m, totalWithDiscount);
    }

    [Fact]
    public void GetDiscountAmount_WithApplicableDiscount_ShouldReturnDiscountAmount()
    {
        var cart = Cart.Create();
        var product = CreateProduct(price: 100m);
        cart.AddItem(product, 1);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(product.Id, 1));

        var availableDiscounts = new[] { discount };

        var discountAmount = cart.GetDiscountAmount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.Equal(50m, discountAmount);
    }

    [Fact]
    public void GetDiscountAmount_WithNoApplicableDiscount_ShouldReturnZero()
    {
        var cart = Cart.Create();
        var product = CreateProduct(price: 100m);
        cart.AddItem(product, 1);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProduct(Guid.NewGuid(), 1));

        var availableDiscounts = new[] { discount };

        var discountAmount = cart.GetDiscountAmount(availableDiscounts, id => 
            id == product.Id ? product : null);

        Assert.Equal(0m, discountAmount);
    }

    [Fact]
    public void GetBestApplicableDiscount_WithComplexCondition_ShouldWorkCorrectly()
    {
        var productXId = Guid.NewGuid();
        var categoryZId = Guid.NewGuid();
        
        var productX = CreateProduct(productXId, categoryZId, 50m);
        var productZ = CreateProduct(Guid.NewGuid(), categoryZId, 30m);
        
        var cart = Cart.Create();
        cart.AddItem(productX, 1);
        cart.AddItem(productZ, 2);

        var discount = Discount.Create("Desconto 50%", 50m);
        discount.AddCondition(DiscountCondition.CreateForProductAndCategory(productXId, categoryZId, 2));

        var availableDiscounts = new[] { discount };

        var bestDiscount = cart.GetBestApplicableDiscount(availableDiscounts, id => 
            id == productX.Id ? productX : 
            id == productZ.Id ? productZ : null);

        Assert.NotNull(bestDiscount);
        Assert.Equal(50m, bestDiscount.Percentage);
    }

    [Fact]
    public void GetBestApplicableDiscount_WithEmptyCart_ShouldReturnNull()
    {
        var cart = Cart.Create();
        var discount = Discount.Create("Desconto 50%", 50m);

        var availableDiscounts = new[] { discount };

        var bestDiscount = cart.GetBestApplicableDiscount(availableDiscounts, _ => null);

        Assert.Null(bestDiscount);
    }
}

