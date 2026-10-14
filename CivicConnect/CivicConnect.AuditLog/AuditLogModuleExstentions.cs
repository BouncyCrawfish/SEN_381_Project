using CivicConnect.AuditLog.Contracts;
using CivicConnect.AuditLog.Events;
using CivicConnect.AuditLog.Persistence;
using CivicConnect.AuditLog.Services;
using CivicConnect.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.AuditLog;

public static class AuditLogModuleExtensions
{
    /// Use the least-privilege civicconnect_app role (SELECT + INSERT on request_history only).
    public static IServiceCollection AddAuditLogModule(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContextFactory<AuditDbContext>(o => o.UseNpgsql(connectionString));
        services.AddTransient<IAuditLogRepository, EfAuditLogRepository>();
        services.AddTransient<IAuditLogService, AuditLogService>();

        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }
}