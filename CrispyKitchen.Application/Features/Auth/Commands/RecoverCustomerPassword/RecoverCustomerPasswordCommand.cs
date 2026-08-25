using MediatR;

namespace CrispyKitchen.Application.Features.Auth.Commands.RecoverCustomerPassword;

public record RecoverCustomerPasswordCommand(string Email, string NewPassword) : IRequest;
