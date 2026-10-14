using CivicConnect.Auth;

namespace CivicConnect.App.Forms;

/// Role-based home screen. Tiles are shown only for permissions the user holds. That is a convenience:
/// every service method checks the permission again, so hiding a tile is never the only protection.
internal sealed partial class MainForm : Form
{
    private readonly IAuthService _auth;
    private readonly IFormFactory _forms;

    public MainForm(ISessionContext session, IAccessControl access, IAuthService auth, IFormFactory forms)
    {
        _auth = auth;
        _forms = forms;
        var user = session.CurrentUser ?? throw new InvalidOperationException("MainForm requires a signed-in user.");

        InitializeComponent();
        Theme.Apply(this);

        lblUser.Text = $"{user.Name}  |  {user.Role}";
        lblWelcome.Text = $"Welcome, {user.Name}";

        var canSubmit = access.HasPermission(Permission.SubmitRequest);
        var canViewOwn = access.HasPermission(Permission.ViewOwnRequests);
        var canViewQueue = access.HasPermission(Permission.ViewStaffQueue);
        var canViewAll = access.HasPermission(Permission.ViewAllRequests);

        btnSubmit.Visible = canSubmit;
        btnMyRequests.Visible = canViewOwn;
        btnQueue.Visible = canViewQueue;
        btnAllRequests.Visible = canViewAll;

        // Decided from the permissions, not from Control.Visible: that reads false until the form is shown.
        lblHint.Text = canSubmit || canViewOwn || canViewQueue || canViewAll
            ? "Choose what you would like to do."
            : "Nothing is available for your role yet.";

        // Closing the home screen ends the session, so nothing stays signed in on a shared machine.
        FormClosed += (_, _) => _auth.Logout();
    }

    private void btnSubmit_Click(object? sender, EventArgs e) => Open(_forms.CreateSubmitRequestForm());
    private void btnMyRequests_Click(object? sender, EventArgs e) => Open(_forms.CreateMyRequestsForm());
    private void btnQueue_Click(object? sender, EventArgs e) => Open(_forms.CreateStaffQueueForm());
    private void btnAllRequests_Click(object? sender, EventArgs e) => Open(_forms.CreateManagerDashboardForm());
    private void btnSignOut_Click(object? sender, EventArgs e) => Close();

    private void Open(Form form)
    {
        using (form) form.ShowDialog(this);
    }
}
