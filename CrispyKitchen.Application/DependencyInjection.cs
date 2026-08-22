using System.Reflection;
using CrispyKitchen.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CrispyKitchen.Application;

/// Registers everything the Application layer needs into the DI container.
/// Keeping this here (not in WebApi's Program.cs) means Application owns
/// its own setup — WebApi just calls one line and doesn't need to know
/// MediatR or FluentValidation are even involved.
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Scans this assembly for every class that implements
        // IRequestHandler<> and registers it automatically —
        // so when we add CreateProductCommandHandler later, we
        // don't touch this file again.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Same idea for FluentValidation — any AbstractValidator<T>
        // we write gets picked up automatically.
        services.AddValidatorsFromAssembly(assembly);
        // add the validation behaviours
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        return services;
    }
}