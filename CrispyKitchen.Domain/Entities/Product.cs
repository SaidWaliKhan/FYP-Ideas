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
    public int StockQuantity { get; private set; } // NEW


    private Product() { }

    public static Product Create(
        string name, 
        string description, 
        decimal price,
        ProductCategory category, 
        string? imageUrl, 
        bool isFeatured, 
        int stockQuantity)
    
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));
        
        if (price <= 0)
            throw new ArgumentException("Price must be greater than zero.", nameof(price));
        
        if (stockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative.", nameof(stockQuantity));


        return new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            Category = category,
            ImageUrl = imageUrl,
            IsFeatured = isFeatured,
            StockQuantity = stockQuantity,
            IsAvailable = stockQuantity > 0
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

    
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (StockQuantity < quantity)
            throw new Domain.Exceptions.InsufficientStockException(Name, StockQuantity, quantity);

        StockQuantity -= quantity;
        if (StockQuantity == 0)
            IsAvailable = false; // auto sold-out — no separate admin action needed

        MarkUpdated();
    }

        public void Restock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        StockQuantity += quantity;
        IsAvailable = true;
        MarkUpdated();
    }
}