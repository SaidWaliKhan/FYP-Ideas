using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Application.Common.Interfaces;
using CrispyKitchen.Application.Common.Models;
using CrispyKitchen.Application.Features.Auth.Commands.Register;
using CrispyKitchen.Domain.Entities;
using CrispyKitchen.Domain.Enums;
using MediatR;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(

        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }


    public async Task<AuthResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            throw new ConflictException($"An account with email'{request.Email}' is already exist. ");


        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(request.FullName, request.Email, passwordHash, UserRole.Customer);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        var token = _jwtTokenGenerator.GenerateToken(user);
        return new AuthResult(token, user.Email, user.FullName, user.Role.ToString());

    }
}