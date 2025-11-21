using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Category Category { get; private set; } = null!;
    public decimal Price { get; private set; }

    private Product() { } // Construtor privado para EF Core

    private Product(Guid id, string name, Category category, decimal price)
    {
        Id = id;
        SetName(name);
        SetCategory(category);
        SetPrice(price);
    }

    public static Product Create(string name, Category category, decimal price)
    {
        return new Product(Guid.NewGuid(), name, category, price);
    }

    public static Product Create(Guid id, string name, Category category, decimal price)
    {
        return new Product(id, name, category, price);
    }

    public void UpdateName(string name)
    {
        SetName(name);
    }

    public void UpdateCategory(Category category)
    {
        SetCategory(category);
    }

    public void UpdatePrice(decimal price)
    {
        SetPrice(price);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name cannot be empty or null.");
        }

        if (name.Length > 200)
        {
            throw new DomainException("Product name cannot exceed 200 characters.");
        }

        Name = name.Trim();
    }

    private void SetCategory(Category category)
    {
        if (category == null)
        {
            throw new DomainException("Product category cannot be null.");
        }

        Category = category;
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }

        if (price > 999999.99m)
        {
            throw new DomainException("Product price cannot exceed 999,999.99.");
        }

        Price = price;
    }
}

