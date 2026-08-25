using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Entities;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.CreateStaffUser;

public class CreateStaffUserCommandHandler : IRequestHandler<CreateStaffUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateStaffUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(CreateStaffUserCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            throw new ConflictException("An account with this email already exists.");

        var user = User.Create(
            request.FullName,
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.Role);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
