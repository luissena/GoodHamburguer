using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class Discount
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal Percentage { get; private set; }
    private readonly List<DiscountCondition> _conditions = new();
    public IReadOnlyCollection<DiscountCondition> Conditions => _conditions.AsReadOnly();
    public bool IsActive { get; private set; }

    private Discount() { }

    private Discount(Guid id, string name, decimal percentage)
    {
        Id = id;
        SetName(name);
        SetPercentage(percentage);
        IsActive = true;
    }

    public static Discount Create(string name, decimal percentage)
    {
        return new Discount(Guid.NewGuid(), name, percentage);
    }

    public static Discount Create(Guid id, string name, decimal percentage)
    {
        return new Discount(id, name, percentage);
    }

    public void AddCondition(DiscountCondition condition)
    {
        if (condition == null)
        {
            throw new DomainException("Condition cannot be null.");
        }

        _conditions.Add(condition);
    }

    public void RemoveCondition(DiscountCondition condition)
    {
        if (condition == null)
        {
            throw new DomainException("Condition cannot be null.");
        }

        _conditions.Remove(condition);
    }

    public void ClearConditions()
    {
        _conditions.Clear();
    }

    public bool IsApplicable(IEnumerable<CartItem> items, Func<Guid, Product?> getProductById)
    {
        if (!IsActive || _conditions.Count == 0)
        {
            return false;
        }

        return _conditions.All(condition => condition.Matches(items, getProductById));
    }

    public decimal CalculateDiscount(decimal total)
    {
        return total * (Percentage / 100m);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void UpdateName(string name)
    {
        SetName(name);
    }

    public void UpdatePercentage(decimal percentage)
    {
        SetPercentage(percentage);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Discount name cannot be empty or null.");
        }

        if (name.Length > 200)
        {
            throw new DomainException("Discount name cannot exceed 200 characters.");
        }

        Name = name.Trim();
    }

    private void SetPercentage(decimal percentage)
    {
        if (percentage <= 0)
        {
            throw new DomainException("Discount percentage must be greater than zero.");
        }

        if (percentage > 100)
        {
            throw new DomainException("Discount percentage cannot exceed 100%.");
        }

        Percentage = percentage;
    }
}

