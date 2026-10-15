using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.App.Forms;

/// Creates forms on demand through the DI container, so forms receive interfaces (IRequestService,
/// IAccessControl, ...) and never construct their own dependencies (Dependency Inversion).
/// Created per use, so the container does not keep closed forms alive.
internal interface IFormFactory
{
    SubmitRequestForm CreateSubmitRequestForm();
    MyRequestsForm CreateMyRequestsForm();
    StaffQueueForm CreateStaffQueueForm();
    ManagerDashboardForm CreateManagerDashboardForm();
    AuditTrailForm CreateAuditTrailForm();
    RequestDetailForm CreateRequestDetailForm(long requestId);
}

internal sealed class FormFactory(IServiceProvider services) : IFormFactory
{
    public SubmitRequestForm CreateSubmitRequestForm() => ActivatorUtilities.CreateInstance<SubmitRequestForm>(services);
    public MyRequestsForm CreateMyRequestsForm() => ActivatorUtilities.CreateInstance<MyRequestsForm>(services);
    public StaffQueueForm CreateStaffQueueForm() => ActivatorUtilities.CreateInstance<StaffQueueForm>(services);
    public ManagerDashboardForm CreateManagerDashboardForm() => ActivatorUtilities.CreateInstance<ManagerDashboardForm>(services);
    public AuditTrailForm CreateAuditTrailForm() => ActivatorUtilities.CreateInstance<AuditTrailForm>(services);
    public RequestDetailForm CreateRequestDetailForm(long requestId) =>
        ActivatorUtilities.CreateInstance<RequestDetailForm>(services, requestId);
}
