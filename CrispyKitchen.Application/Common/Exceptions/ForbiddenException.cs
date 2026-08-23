namespace CrispyKitchen.Application.Common.Exceptions;

// Different from Unauthorized: Unauthorized = "we don't know who you are."
// Forbidden = "we know exactly who you are, and the answer is still no."
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}