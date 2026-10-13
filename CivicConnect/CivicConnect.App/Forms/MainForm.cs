using CivicConnect.Auth;

namespace CivicConnect.App.Forms;

/// <summary>
/// Role-based shell. Buttons are shown only for permissions the user holds, which is a convenience:
/// every service method checks the permission again, so hiding a button is never the only protection.
/// </summary>
internal sealed class MainForm : Form
{
    private readonly IAuthService _auth;
    private readonly IFormFactory _forms;

    public MainForm(ISessionContext session, IAccessControl access, IAuthService auth, IFormFactory forms)
    {
        _auth = auth;
        _forms = forms;

        var user = session.CurrentUser ?? throw new InvalidOperationException("MainForm requires a signed-in user.");

        Text = "CivicConnect";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(360, 320);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        var layout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(16)
        };
        layout.Controls.Add(new Label
        {
            Text = $"Signed in as {user.Name} ({user.Role})",
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 12)
        });

        if (access.HasPermission(Permission.SubmitRequest))
            layout.Controls.Add(MakeButton("&Submit a new request", () => _forms.CreateSubmitRequestForm()));
        if (access.HasPermission(Permission.ViewOwnRequests))
            layout.Controls.Add(MakeButton("&My requests", () => _forms.CreateMyRequestsForm()));
        if (access.HasPermission(Permission.ViewStaffQueue))
            layout.Controls.Add(MakeButton("Staff &queue", () => _forms.CreateStaffQueueForm()));

        if (layout.Controls.Count == 1)
            layout.Controls.Add(new Label { Text = "Nothing is available for your role yet.", AutoSize = true });

        var signOut = new Button { Text = "Sign &out", Width = 300, Height = 34, Margin = new Padding(0, 24, 0, 0) };
        signOut.Click += (_, _) => Close();
        layout.Controls.Add(signOut);

        Controls.Add(layout);
        // Closing the shell ends the session, so nothing stays signed in on a shared machine.
        FormClosed += (_, _) => _auth.Logout();
    }

    private Button MakeButton(string text, Func<Form> create)
    {
        var button = new Button { Text = text, Width = 300, Height = 34, Margin = new Padding(0, 0, 0, 8) };
        button.Click += (_, _) =>
        {
            using var form = create();
            form.ShowDialog(this);
        };
        return button;
    }
}
