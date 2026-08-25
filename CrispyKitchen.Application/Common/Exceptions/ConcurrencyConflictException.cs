namespace CrispyKitchen.Application.Common.Exceptions;

// The Application-layer, EF-Core-agnostic version of "someone else
// changed this at the exact same moment you did." Infrastructure
// translates EF Core's DbUpdateConcurrencyException into this — handlers
// never need to know EF Core exists.
public class ConcurrencyConflictException : Exception
{
    public ConcurrencyConflictException(string message) : base(message) { }
}