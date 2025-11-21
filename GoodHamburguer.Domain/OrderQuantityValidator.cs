using GoodHamburguer.Domain.Entities;
using GoodHamburguer.Domain.ValueObjects;

namespace GoodHamburguer.Domain;

public class OrderQuantityValidator
{
    private readonly IEnumerable<OrderQuantityRule> _rules;

    public OrderQuantityValidator(IEnumerable<OrderQuantityRule> rules)
    {
        _rules = rules ?? Enumerable.Empty<OrderQuantityRule>();
    }

    public ValidationResult ValidateCart(Cart cart, Func<Guid, Product?> getProductById)
    {
        if (cart == null)
        {
            return ValidationResult.Failure("Cart cannot be null.");
        }

        if (!_rules.Any())
        {
            return ValidationResult.Success();
        }

        var errors = new List<string>();

        foreach (var rule in _rules)
        {
            var matchingItems = cart.Items
                .Where(item =>
                {
                    var product = getProductById(item.ProductId);
                    return product != null && rule.Matches(product);
                })
                .ToList();

            var totalQuantity = matchingItems.Sum(item => item.Quantity);

            if (totalQuantity > rule.MaxQuantity)
            {
                errors.Add($"You already have {rule.RuleName} in your cart. Only {rule.MaxQuantity} per order is allowed.");
            }
        }

        return errors.Any() 
            ? ValidationResult.Failure(errors) 
            : ValidationResult.Success();
    }

    public ValidationResult ValidateOrder(Order order, Func<Guid, Product?> getProductById)
    {
        if (order == null)
        {
            return ValidationResult.Failure("Order cannot be null.");
        }

        if (!_rules.Any())
        {
            return ValidationResult.Success();
        }

        var errors = new List<string>();

        foreach (var rule in _rules)
        {
            var matchingItems = order.Items
                .Where(item =>
                {
                    var product = getProductById(item.ProductId);
                    return product != null && rule.Matches(product);
                })
                .ToList();

            var totalQuantity = matchingItems.Sum(item => item.Quantity);

            if (totalQuantity > rule.MaxQuantity)
            {
                errors.Add($"The order contains {rule.RuleName} in excess. Only {rule.MaxQuantity} per order is allowed.");
            }
        }

        return errors.Any() 
            ? ValidationResult.Failure(errors) 
            : ValidationResult.Success();
    }
}

