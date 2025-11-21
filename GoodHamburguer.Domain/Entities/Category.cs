using GoodHamburguer.Domain.Exceptions;

namespace GoodHamburguer.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Category() { } // Construtor privado para EF Core

    private Category(Guid id, string name)
    {
        Id = id;
        SetName(name);
    }

    public static Category Create(string name)
    {
        return new Category(Guid.NewGuid(), name);
    }

    public static Category Create(Guid id, string name)
    {
        return new Category(id, name);
    }

    public void UpdateName(string name)
    {
        SetName(name);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name cannot be empty or null.");
        }

        if (name.Length > 100)
        {
            throw new DomainException("Category name cannot exceed 100 characters.");
        }

        Name = name.Trim();
    }
}

