namespace CivicConnect.App.Forms;

//Small shared helpers: message boxes, time formatting and the generic error text.
internal static class Ui
{
    //Shown instead of an exception message: exceptions can contain server names or SQL.
    public const string Unavailable = "Something went wrong. Please try again later.";

    //Database times are UTC; people read local time.
    public static string Local(DateTime utc) =>
        DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm");

    public static void ShowInfo(IWin32Window owner, string message) =>
        MessageBox.Show(owner, message, "CivicConnect", MessageBoxButtons.OK, MessageBoxIcon.Information);
}

//Flat row for the request grids (DataGridView binds to public properties).
internal sealed record RequestRow(long Id, string Category, string Location, string Status, string Assigned, string Submitted)
{
    public static RequestRow From(CivicConnect.Requests.Contracts.RequestDto r) => new(
        r.Id, r.Category, r.Location, r.Status,
        r.AssignedStaffId is { } id ? $"Staff #{id}" : "-",
        Ui.Local(r.CreatedAt));
}
