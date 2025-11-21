using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.Repositories;

namespace GoodHamburguer.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IDiscountRepository discountRepository,
        CancellationToken cancellationToken = default)
    {
        var existingCategories = await categoryRepository.GetAllAsync(cancellationToken);
        if (existingCategories.Any())
        {
            return;
        }

        var extrasCategory = Category.Create("Extras");
        var sandwichesCategory = Category.Create("Sandwiches");

        await categoryRepository.CreateAsync(extrasCategory, cancellationToken);
        await categoryRepository.CreateAsync(sandwichesCategory, cancellationToken);

        var xBurger = Product.Create("X Burger", sandwichesCategory, 5.00m);
        var xEgg = Product.Create("X Egg", sandwichesCategory, 4.50m);
        var xBacon = Product.Create("X Bacon", sandwichesCategory, 7.00m);

        await productRepository.CreateAsync(xBurger, cancellationToken);
        await productRepository.CreateAsync(xEgg, cancellationToken);
        await productRepository.CreateAsync(xBacon, cancellationToken);

        var fries = Product.Create("Fries", extrasCategory, 2.00m);
        var softDrink = Product.Create("Soft Drink", extrasCategory, 2.50m);

        await productRepository.CreateAsync(fries, cancellationToken);
        await productRepository.CreateAsync(softDrink, cancellationToken);

        await SeedDiscountsAsync(discountRepository, sandwichesCategory, fries, softDrink, cancellationToken);
    }

    private static async Task SeedDiscountsAsync(
        IDiscountRepository discountRepository,
        Category sandwichesCategory,
        Product fries,
        Product softDrink,
        CancellationToken cancellationToken)
    {
        var discount1 = Discount.Create("Complete Combo", 20m);
        discount1.AddCondition(DiscountCondition.CreateForCategory(sandwichesCategory.Id, 1));
        discount1.AddCondition(DiscountCondition.CreateForProduct(fries.Id, 1));
        discount1.AddCondition(DiscountCondition.CreateForProduct(softDrink.Id, 1));
        await discountRepository.CreateAsync(discount1, cancellationToken);

        var discount2 = Discount.Create("Drink Combo", 15m);
        discount2.AddCondition(DiscountCondition.CreateForCategory(sandwichesCategory.Id, 1));
        discount2.AddCondition(DiscountCondition.CreateForProduct(softDrink.Id, 1));
        await discountRepository.CreateAsync(discount2, cancellationToken);

        var discount3 = Discount.Create("Fries Combo", 10m);
        discount3.AddCondition(DiscountCondition.CreateForCategory(sandwichesCategory.Id, 1));
        discount3.AddCondition(DiscountCondition.CreateForProduct(fries.Id, 1));
        await discountRepository.CreateAsync(discount3, cancellationToken);
    }
}

