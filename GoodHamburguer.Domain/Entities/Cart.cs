using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class Cart
{
    public Guid Id { get; private set; }
    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private Cart() { }

    private Cart(Guid id)
    {
        Id = id;
    }

    public static Cart Create()
    {
        return new Cart(Guid.NewGuid());
    }

    public static Cart Create(Guid id)
    {
        return new Cart(id);
    }

    public void AddItem(Product product, int quantity)
    {
        if (product == null)
        {
            throw new DomainException("Product cannot be null.");
        }

        var existingItem = _items.FirstOrDefault(item => item.ProductId == product.Id);

        if (existingItem != null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var cartItem = CartItem.Create(product.Id, product.Name, product.Price, quantity);
            _items.Add(cartItem);
        }
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = _items.FirstOrDefault(item => item.ProductId == productId);

        if (item == null)
        {
            throw new DomainException("Item was not found in the cart.");
        }

        item.UpdateQuantity(quantity);
    }

    public void RemoveItem(Guid productId)
    {
        var item = _items.FirstOrDefault(item => item.ProductId == productId);

        if (item == null)
        {
            throw new DomainException("Item was not found in the cart.");
        }

        _items.Remove(item);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public decimal GetTotal()
    {
        return _items.Sum(item => item.GetSubtotal());
    }

    public int GetTotalItems()
    {
        return _items.Sum(item => item.Quantity);
    }

    public bool IsEmpty()
    {
        return _items.Count == 0;
    }

    public Discount? GetBestApplicableDiscount(
        IEnumerable<Discount> availableDiscounts,
        Func<Guid, Product?> getProductById)
    {
        if (IsEmpty() || availableDiscounts == null || !availableDiscounts.Any())
        {
            return null;
        }

        var applicableDiscounts = availableDiscounts
            .Where(discount => discount.IsApplicable(_items, getProductById))
            .OrderByDescending(discount => discount.Percentage)
            .ToList();

        return applicableDiscounts.FirstOrDefault();
    }

    public decimal GetTotalWithDiscount(
        IEnumerable<Discount> availableDiscounts,
        Func<Guid, Product?> getProductById)
    {
        var total = GetTotal();

        var bestDiscount = GetBestApplicableDiscount(availableDiscounts, getProductById);

        if (bestDiscount == null)
        {
            return total;
        }

        var discountAmount = bestDiscount.CalculateDiscount(total);
        return total - discountAmount;
    }

    public decimal GetDiscountAmount(
        IEnumerable<Discount> availableDiscounts,
        Func<Guid, Product?> getProductById)
    {
        var total = GetTotal();
        var bestDiscount = GetBestApplicableDiscount(availableDiscounts, getProductById);

        if (bestDiscount == null)
        {
            return 0;
        }

        return bestDiscount.CalculateDiscount(total);
    }

    public void ValidateQuantityRules(GoodHamburguer.Domain.OrderQuantityValidator validator, Func<Guid, Product?> getProductById)
    {
        var result = validator.ValidateCart(this, getProductById);
        
        if (!result.IsValid)
        {
            throw new DomainException(string.Join(" ", result.Errors));
        }
    }
}

