using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class DiscountCondition
{
    public Guid? ProductId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public int MinimumQuantity { get; private set; }

    private DiscountCondition() { }

    private DiscountCondition(Guid? productId, Guid? categoryId, int minimumQuantity)
    {
        SetProductId(productId);
        SetCategoryId(categoryId);
        SetMinimumQuantity(minimumQuantity);
    }

    public static DiscountCondition CreateForProduct(Guid productId, int minimumQuantity)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        return new DiscountCondition(productId, null, minimumQuantity);
    }

    public static DiscountCondition CreateForCategory(Guid categoryId, int minimumQuantity)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("Category ID cannot be empty.");
        }

        return new DiscountCondition(null, categoryId, minimumQuantity);
    }

    public static DiscountCondition CreateForProductAndCategory(Guid productId, Guid categoryId, int minimumQuantity)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        if (categoryId == Guid.Empty)
        {
            throw new DomainException("Category ID cannot be empty.");
        }

        return new DiscountCondition(productId, categoryId, minimumQuantity);
    }

    public bool Matches(IEnumerable<CartItem> items, Func<Guid, Product?> getProductById)
    {
        if (ProductId.HasValue && CategoryId.HasValue)
        {
            var hasProduct = items.Any(item => item.ProductId == ProductId.Value);
            
            if (!hasProduct)
            {
                return false;
            }

            var categoryItems = items
                .Where(item =>
                {
                    var product = getProductById(item.ProductId);
                    return product != null && product.Category.Id == CategoryId.Value;
                })
                .Sum(item => item.Quantity);

            return categoryItems >= MinimumQuantity;
        }

        if (ProductId.HasValue)
        {
            var item = items.FirstOrDefault(item => item.ProductId == ProductId.Value);
            return item != null && item.Quantity >= MinimumQuantity;
        }

        if (CategoryId.HasValue)
        {
            var categoryItems = items
                .Where(item =>
                {
                    var product = getProductById(item.ProductId);
                    return product != null && product.Category.Id == CategoryId.Value;
                })
                .Sum(item => item.Quantity);

            return categoryItems >= MinimumQuantity;
        }

        return false;
    }

    private void SetProductId(Guid? productId)
    {
        if (productId.HasValue && productId.Value == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        ProductId = productId;
    }

    private void SetCategoryId(Guid? categoryId)
    {
        if (categoryId.HasValue && categoryId.Value == Guid.Empty)
        {
            throw new DomainException("Category ID cannot be empty.");
        }

        CategoryId = categoryId;
    }

    private void SetMinimumQuantity(int minimumQuantity)
    {
        if (minimumQuantity <= 0)
        {
            throw new DomainException("Minimum quantity must be greater than zero.");
        }

        MinimumQuantity = minimumQuantity;
    }
}

