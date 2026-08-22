namespace CrispyKitchen.Domain.Enums;

// Real-world analogy: think of this like job badges at the restaurant —
// a Customer badge, an Admin badge, a KitchenStaff badge. Each opens
// different doors later (authorization), but that's not this file's job.
public enum UserRole
{
    Customer = 0,
    Admin = 1,
    KitchenStaff = 2
}