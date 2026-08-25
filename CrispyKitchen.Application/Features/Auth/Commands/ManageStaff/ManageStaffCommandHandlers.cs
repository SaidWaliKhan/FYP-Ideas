using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.ManageStaff;

public class UpdateStaffRoleCommandHandler : IRequestHandler<UpdateStaffRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    public UpdateStaffRoleCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) => (_unitOfWork, _currentUser) = (unitOfWork, currentUser);

    public async Task Handle(UpdateStaffRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await GetStaffUser(request.Id, cancellationToken);
        if (user.Id == _currentUser.UserId) throw new ConflictException("You cannot change your own role.");
        await EnsureNotRemovingLastAdmin(user, request.Role == UserRole.Admin, cancellationToken);
        user.SetRole(request.Role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<CrispyKitchen.Domain.Entities.User> GetStaffUser(Guid id, CancellationToken cancellationToken)
        => await _unitOfWork.Users.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException("Staff account not found.");

    private async Task EnsureNotRemovingLastAdmin(CrispyKitchen.Domain.Entities.User user, bool remainsAdmin, CancellationToken cancellationToken)
    {
        if (user.Role != UserRole.Admin || remainsAdmin || !user.IsActive) return;
        var activeAdmins = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).Count(account => account.Role == UserRole.Admin && account.IsActive);
        if (activeAdmins <= 1) throw new ConflictException("At least one active Admin account is required.");
    }
}

public class SetStaffActiveCommandHandler : IRequestHandler<SetStaffActiveCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    public SetStaffActiveCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) => (_unitOfWork, _currentUser) = (unitOfWork, currentUser);

    public async Task Handle(SetStaffActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken) ?? throw new NotFoundException("Staff account not found.");
        if (user.Id == _currentUser.UserId) throw new ConflictException("You cannot deactivate your own account.");
        if (!request.IsActive && user.Role == UserRole.Admin && user.IsActive)
        {
            var activeAdmins = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).Count(account => account.Role == UserRole.Admin && account.IsActive);
            if (activeAdmins <= 1) throw new ConflictException("At least one active Admin account is required.");
        }
        user.SetActive(request.IsActive);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
