using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class OrderQuantityRule
{
    public Guid? ProductId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public int MaxQuantity { get; private set; }
    public string RuleName { get; private set; } = string.Empty;

    private OrderQuantityRule() { }

    private OrderQuantityRule(Guid? productId, Guid? categoryId, int maxQuantity, string ruleName)
    {
        SetProductId(productId);
        SetCategoryId(categoryId);
        SetMaxQuantity(maxQuantity);
        SetRuleName(ruleName);
        ValidateRule();
    }

    public static OrderQuantityRule CreateForProduct(Guid productId, int maxQuantity, string ruleName)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        return new OrderQuantityRule(productId, null, maxQuantity, ruleName);
    }

    public static OrderQuantityRule CreateForCategory(Guid categoryId, int maxQuantity, string ruleName)
    {
        if (categoryId == Guid.Empty)
        {
            throw new DomainException("Category ID cannot be empty.");
        }

        return new OrderQuantityRule(null, categoryId, maxQuantity, ruleName);
    }

    public static OrderQuantityRule Create(Guid? productId, Guid? categoryId, int maxQuantity, string ruleName)
    {
        return new OrderQuantityRule(productId, categoryId, maxQuantity, ruleName);
    }

    public bool Matches(Product product)
    {
        if (product == null)
        {
            return false;
        }

        if (ProductId.HasValue && product.Id == ProductId.Value)
        {
            return true;
        }

        if (CategoryId.HasValue && product.Category.Id == CategoryId.Value)
        {
            return true;
        }

        return false;
    }

    private void ValidateRule()
    {
        if (!ProductId.HasValue && !CategoryId.HasValue)
        {
            throw new DomainException("The rule must have at least one ProductId or CategoryId.");
        }

        if (ProductId.HasValue && ProductId.Value == Guid.Empty)
        {
            throw new DomainException("Product ID cannot be empty.");
        }

        if (CategoryId.HasValue && CategoryId.Value == Guid.Empty)
        {
            throw new DomainException("Category ID cannot be empty.");
        }
    }

    private void SetProductId(Guid? productId)
    {
        ProductId = productId;
    }

    private void SetCategoryId(Guid? categoryId)
    {
        CategoryId = categoryId;
    }

    private void SetMaxQuantity(int maxQuantity)
    {
        if (maxQuantity < 1)
        {
            throw new DomainException("Maximum quantity must be greater than zero.");
        }

        MaxQuantity = maxQuantity;
    }

    private void SetRuleName(string ruleName)
    {
        if (string.IsNullOrWhiteSpace(ruleName))
        {
            throw new DomainException("Rule name cannot be empty or null.");
        }

        RuleName = ruleName.Trim();
    }
}

