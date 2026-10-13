using Microsoft.Extensions.DependencyInjection;

namespace CivicConnect.App.Forms;

/// <summary>
/// Creates forms on demand through the DI container, so forms receive interfaces (IRequestService,
/// IAccessControl, ...) and never construct their own dependencies (Dependency Inversion).
/// Created per use, so the container does not keep closed forms alive.
/// </summary>
internal interface IFormFactory
{
    SubmitRequestForm CreateSubmitRequestForm();
    MyRequestsForm CreateMyRequestsForm();
    StaffQueueForm CreateStaffQueueForm();
    RequestDetailForm CreateRequestDetailForm(long requestId);
}

internal sealed class FormFactory(IServiceProvider services) : IFormFactory
{
    public SubmitRequestForm CreateSubmitRequestForm() => ActivatorUtilities.CreateInstance<SubmitRequestForm>(services);
    public MyRequestsForm CreateMyRequestsForm() => ActivatorUtilities.CreateInstance<MyRequestsForm>(services);
    public StaffQueueForm CreateStaffQueueForm() => ActivatorUtilities.CreateInstance<StaffQueueForm>(services);
    public RequestDetailForm CreateRequestDetailForm(long requestId) =>
        ActivatorUtilities.CreateInstance<RequestDetailForm>(services, requestId);
}
