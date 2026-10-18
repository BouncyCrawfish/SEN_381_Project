using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CivicConnect.Auth;

public static class AuthModuleExtensions
{
    /// <param name="connectionString">
    /// Must use the least-privilege civicconnect_app role. Read it from user-secrets, an environment
    /// variable or the gitignored appsettings.json. Never hard-code it.
    /// </param>
    public static IServiceCollection AddAuthModule(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContextFactory<AuthDbContext>(o => o.UseNpgsql(connectionString));
        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton<IPasswordHasher>(_ => new BCryptPasswordHasher());
        services.AddSingleton<ILoginAttemptTracker, InMemoryLoginAttemptTracker>();

        // One session object for the whole app, exposed read-only to other modules.
        services.AddSingleton<SessionContext>();
        services.AddSingleton<ISessionContext>(sp => sp.GetRequiredService<SessionContext>());
        services.AddSingleton<ISessionWriter>(sp => sp.GetRequiredService<SessionContext>());
        services.AddSingleton<IAccessControl, AccessControl>();

        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>(ServiceLifetime.Singleton);

        services.AddTransient<IUserRepository, EfUserRepository>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IUserService, UserService>();
        return services;
    }
}
