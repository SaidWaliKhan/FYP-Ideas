using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public class ResetStaffPasswordCommandHandler : IRequestHandler<ResetStaffPasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetStaffPasswordCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        => (_unitOfWork, _passwordHasher) = (unitOfWork, passwordHasher);

    public async Task Handle(ResetStaffPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Staff account not found.");
        if (user.Role is not (UserRole.Admin or UserRole.KitchenStaff)) throw new ConflictException("Only staff accounts can be reset here.");
        user.ResetPasswordHash(_passwordHasher.Hash(request.NewPassword));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public class DeleteStaffUserCommandHandler : IRequestHandler<DeleteStaffUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteStaffUserCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        => (_unitOfWork, _currentUser) = (unitOfWork, currentUser);

    public async Task Handle(DeleteStaffUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Staff account not found.");
        if (user.Role is not (UserRole.Admin or UserRole.KitchenStaff)) throw new ConflictException("Only staff accounts can be deleted here.");
        if (user.Id == _currentUser.UserId) throw new ConflictException("You cannot delete your own account.");
        if (user.Role == UserRole.Admin && user.IsActive)
        {
            var activeAdmins = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).Count(account => account.Role == UserRole.Admin && account.IsActive);
            if (activeAdmins <= 1) throw new ConflictException("At least one active Admin account is required.");
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
