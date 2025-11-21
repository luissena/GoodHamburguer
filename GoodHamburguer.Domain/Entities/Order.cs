using GoodHamburguer.Domain.Exceptions;
using GoodHamburguer.Domain.ValueObjects;

namespace GoodHamburguer.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal? DiscountAmount { get; private set; }
    public decimal Total { get; private set; }

    private Order() { }

    private Order(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Status = OrderStatus.Pending;
        Subtotal = 0;
        DiscountAmount = null;
        Total = 0;
    }

    public static Order Create()
    {
        return new Order(Guid.NewGuid(), DateTime.UtcNow);
    }

    public static Order Create(Guid id, DateTime createdAt)
    {
        return new Order(id, createdAt);
    }

    public static Order CreateFromCart(Cart cart, decimal discountAmount = 0, GoodHamburguer.Domain.OrderQuantityValidator? validator = null, Func<Guid, Product?>? getProductById = null)
    {
        if (validator != null && getProductById != null)
        {
            var result = validator.ValidateOrder(CreateFromCartInternal(cart, discountAmount), getProductById);
            
            if (!result.IsValid)
            {
                throw new DomainException(string.Join(" ", result.Errors));
            }
        }

        return CreateFromCartInternal(cart, discountAmount);
    }

    private static Order CreateFromCartInternal(Cart cart, decimal discountAmount)
    {
        var order = Create();

        foreach (var cartItem in cart.Items)
        {
            var orderItem = OrderItem.Create(
                cartItem.ProductId,
                cartItem.ProductName,
                cartItem.UnitPrice,
                cartItem.Quantity);
            order._items.Add(orderItem);
        }

        order.Subtotal = cart.GetTotal();
        order.DiscountAmount = discountAmount > 0 ? discountAmount : null;
        order.Total = order.Subtotal - (order.DiscountAmount ?? 0);

        return order;
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
        {
            throw new DomainException("Item cannot be null.");
        }

        _items.Add(item);
        RecalculateTotals();
    }

    public void UpdateStatus(OrderStatus status)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
        {
            throw new DomainException("It is not possible to change the status of a delivered or cancelled order.");
        }

        Status = status;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
        {
            throw new DomainException("It is not possible to cancel an already delivered order.");
        }

        Status = OrderStatus.Cancelled;
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(item => item.GetSubtotal());
        Total = Subtotal - (DiscountAmount ?? 0);
    }
}

