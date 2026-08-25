using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Domain.Enums;
using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Queries.GetStaffUsers;

public class GetStaffUsersQueryHandler : IRequestHandler<GetStaffUsersQuery, List<StaffUserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetStaffUsersQueryHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<List<StaffUserDto>> Handle(GetStaffUsersQuery request, CancellationToken cancellationToken)
        => (await _unitOfWork.Users.GetAllAsync(cancellationToken))
            .Where(user => user.Role is UserRole.Admin or UserRole.KitchenStaff)
            .OrderBy(user => user.FullName)
            .Select(user => new StaffUserDto(user.Id, user.FullName, user.Email, user.Role.ToString(), user.IsActive, user.CreatedAtUtc))
            .ToList();
}
