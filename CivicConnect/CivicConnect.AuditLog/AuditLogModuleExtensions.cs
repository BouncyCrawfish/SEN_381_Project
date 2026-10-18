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

        // DEF-01 fix: AuditLogEventHandler is NOT registered. RequestService already writes the
        // request_history row in the same transaction as the status change (ADR-002), so a second
        // writer here produced duplicate history rows. The handler class and its tests are kept as a
        // ready listener in case history writing ever moves out of the Requests module (ADR-005).
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }
}