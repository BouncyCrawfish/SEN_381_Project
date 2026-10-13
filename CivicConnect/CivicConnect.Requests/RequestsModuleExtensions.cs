using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Persistence;
using CivicConnect.Requests.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CivicConnect.Requests;

public static class RequestsModuleExtensions
{
    /// <summary>
    /// Call AFTER AddAuthModule: this module needs ISessionContext, IAccessControl and IUserService.
    /// <paramref name="connectionString"/> must use the least-privilege civicconnect_app role.
    /// </summary>
    public static IServiceCollection AddRequestsModule(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContextFactory<RequestsDbContext>(o => o.UseNpgsql(connectionString));
        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton<IValidator<SubmitRequestDto>, SubmitRequestDtoValidator>();
        services.AddSingleton<IValidator<AssignRequestDto>, AssignRequestDtoValidator>();
        services.AddSingleton<IValidator<UpdateRequestStatusDto>, UpdateRequestStatusDtoValidator>();
        services.AddSingleton<IValidator<AddNoteDto>, AddNoteDtoValidator>();

        services.AddTransient<IRequestRepository, EfRequestRepository>();
        services.AddTransient<IRequestService, RequestService>();
        return services;
    }
}
