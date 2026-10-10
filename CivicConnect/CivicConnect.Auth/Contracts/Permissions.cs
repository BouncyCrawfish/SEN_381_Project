using CivicConnect.Common;

namespace CivicConnect.Auth;

/// <summary>Business capabilities from the Master Project Brief section 3.</summary>
public enum Permission
{
    SubmitRequest,
    ViewOwnRequests,
    ViewStaffQueue,
    SearchRequests,
    AssignRequest,
    UpdateRequestStatus,
    AddNote,
    ResolveRequest,
    CloseRequest,
    ViewRequestHistory,
    ViewAllRequests,
    ViewReports,
    ViewStaffDirectory
}

/// <summary>
/// The ONE place that decides what each role may do. ASSUMPTION: derived from the Master Brief
/// capability lists; confirm against the PED requirements and adjust here (and in the tests).
/// This is coarse-grained. Row-level rules ("a requester sees only their own requests") belong to
/// the Requests module, which should compare ISessionContext.CurrentUser.Id to the record owner.
/// </summary>
public static class RolePermissions
{
    private static readonly IReadOnlyDictionary<UserRole, IReadOnlySet<Permission>> Map =
        new Dictionary<UserRole, IReadOnlySet<Permission>>
        {
            [UserRole.Requester] = new HashSet<Permission>
            {
                Permission.SubmitRequest,
                Permission.ViewOwnRequests
            },
            [UserRole.Staff] = new HashSet<Permission>
            {
                Permission.ViewStaffQueue,
                Permission.SearchRequests,
                Permission.AssignRequest,
                Permission.UpdateRequestStatus,
                Permission.AddNote,
                Permission.ResolveRequest,
                Permission.CloseRequest,
                Permission.ViewRequestHistory,
                Permission.ViewStaffDirectory
            },
            [UserRole.Manager] = new HashSet<Permission>
            {
                Permission.ViewAllRequests,
                Permission.SearchRequests,
                Permission.ViewRequestHistory,
                Permission.ViewReports,
                Permission.ViewStaffDirectory
            }
        };

    public static bool Has(UserRole role, Permission permission) =>
        Map.TryGetValue(role, out var set) && set.Contains(permission);
}
