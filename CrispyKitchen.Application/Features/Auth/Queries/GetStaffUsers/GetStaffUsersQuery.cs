using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Queries.GetStaffUsers;

public record GetStaffUsersQuery : IRequest<List<StaffUserDto>>;
