namespace CrispyKitchen.Application.Features.Menu;

/// What we send back to the client — deliberately NOT the same shape
/// as the Product entity. This is called the "DTO wall": it stops
/// internal domain details from leaking into the API contract, so you
/// can change how Product works internally without breaking every
/// client that consumes the API.

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Category,
    string? ImageUrl,
    bool IsAvailable,
    bool IsFeatured
    );