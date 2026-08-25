using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.RecoverCustomerPassword;

public class RecoverCustomerPasswordCommandHandler : IRequestHandler<RecoverCustomerPasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RecoverCustomerPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        => (_unitOfWork, _passwordHasher) = (unitOfWork, passwordHasher);

    public async Task Handle(RecoverCustomerPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new NotFoundException("Customer account not found.");

        if (user.Role != UserRole.Customer)
            throw new ConflictException("Password recovery is available only for customer accounts.");

        user.ResetPasswordHash(_passwordHasher.Hash(request.NewPassword));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
