using CrispyKitchen.Domain.Common;
using CrispyKitchen.Domain.Enums;

namespace CrispyKitchen.Domain.Entities;

/// <summary>
/// Same pattern as User: private setters, controlled entry points.
/// Notice there's no public "IsAvailable = true" anywhere outside this
/// class — availability can ONLY change through SetAvailability(), so
/// there's exactly one code path to audit if "why is this item hidden?"
/// ever comes up in production.
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public ProductCategory Category { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsFeatured { get; private set; }

    private Product() { }

    public static Product Create(
        string name, string description, decimal price,
        ProductCategory category, string? imageUrl, bool isFeatured)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(price));

        return new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Category = category,
            ImageUrl = imageUrl,
            IsFeatured = isFeatured,
            IsAvailable = true
        };
    }

    public void UpdateDetails(
        string name, string description, decimal price,
        ProductCategory category, string? imageUrl, bool isFeatured)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(price));

        Name = name.Trim();
        Description = description.Trim();
        Price = price;
        Category = category;
        ImageUrl = imageUrl;
        IsFeatured = isFeatured;
        MarkUpdated();
    }

    public void SetAvailability(bool isAvailable)
    {
        IsAvailable = isAvailable;
        MarkUpdated();
    }
}