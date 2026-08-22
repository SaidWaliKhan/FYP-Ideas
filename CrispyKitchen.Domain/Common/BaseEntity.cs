namespace CrispyKitchen.Domain.Common;


/// Base class every domain entity inherits from.
/// Centralizes the Id and audit fields so we don't repeat them everywhere (DRY),
/// and gives every entity a consistent identity contract.

public abstract class BaseEntity
{
    // We generate Guids in code (not in the database) — this lets domain
    // objects have a real identity the moment they're created in memory,
    // before they're ever saved. Important later for things like raising
    // domain events right after creation.
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }

    public void MarkUpdated() => UpdatedAtUtc = DateTime.UtcNow;
}