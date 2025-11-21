using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class OrderQuantityRuleEntity
{
    public Guid Id { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public int MaxQuantity { get; private set; }
    public string RuleName { get; private set; } = string.Empty;

    private OrderQuantityRuleEntity() { }

    private OrderQuantityRuleEntity(Guid id, Guid? productId, Guid? categoryId, int maxQuantity, string ruleName)
    {
        Id = id;
        SetProductId(productId);
        SetCategoryId(categoryId);
        SetMaxQuantity(maxQuantity);
        SetRuleName(ruleName);
        ValidateRule();
    }

    public static OrderQuantityRuleEntity Create(Guid? productId, Guid? categoryId, int maxQuantity, string ruleName)
    {
        return new OrderQuantityRuleEntity(Guid.NewGuid(), productId, categoryId, maxQuantity, ruleName);
    }

    public static OrderQuantityRuleEntity Create(Guid id, Guid? productId, Guid? categoryId, int maxQuantity, string ruleName)
    {
        return new OrderQuantityRuleEntity(id, productId, categoryId, maxQuantity, ruleName);
    }

    public OrderQuantityRule ToValueObject()
    {
        return OrderQuantityRule.Create(ProductId, CategoryId, MaxQuantity, RuleName);
    }

    public void Update(Guid? productId, Guid? categoryId, int? maxQuantity, string? ruleName)
    {
        if (productId.HasValue)
        {
            SetProductId(productId);
        }

        if (categoryId.HasValue)
        {
            SetCategoryId(categoryId);
        }

        if (maxQuantity.HasValue)
        {
            SetMaxQuantity(maxQuantity.Value);
        }

        if (!string.IsNullOrWhiteSpace(ruleName))
        {
            SetRuleName(ruleName);
        }

        ValidateRule();
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

