using CivicConnect.Auth;

namespace CivicConnect.App.Forms;

/// Requester self-registration. Security notes:
///  - there is NO role field: the module always creates a Requester, so the UI cannot even ask
///    for Staff/Manager (those accounts are provisioned by the DB owner role);
///  - the real validation (length, email format, duplicates) happens in the module; the only
///    client-side check is "passwords match", which is a convenience, not a security control;
///  - passwords are cleared after every attempt and when the form closes; the form cannot be
///    closed or double-submitted while the request is in flight;
///  - errors are the module's messages only; unexpected exceptions are never shown.

internal sealed class RegisterForm : Form
{
    private readonly IUserService _users;

    private readonly TableLayoutPanel _layout = new();
    private readonly TextBox _name = new();
    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly TextBox _confirm = new();
    private readonly Label _hint = new();
    private readonly Label _status = new();
    private readonly Button _register = new();
    private bool _busy;

    /// <summary>The normalised email of the new account, set when registration succeeds.</summary>
    public string RegisteredEmail { get; private set; } = string.Empty;

    public RegisterForm(IUserService users)
    {
        _users = users;

        Text = "CivicConnect - Create account";
        AutoScaleMode = AutoScaleMode.Dpi;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(560, 680);
        MaximizeBox = true;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;

        // The card in the middle of the screen. Tag "card" = white panel with a border (Theme).
        _layout.Tag = "card";
        _layout.AutoSize = true;
        _layout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        _layout.Anchor = AnchorStyles.None;
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380));
        _layout.Controls.Add(new Label { Text = "Create a requester account", Tag = "heading", AutoSize = true, Margin = new Padding(0, 0, 0, 8) });

        _name.MaxLength = 100;
        _name.AccessibleName = "Full name";
        _email.MaxLength = 254;
        _email.AccessibleName = "Email address";
        _email.AutoCompleteMode = AutoCompleteMode.None;

        foreach (var box in new[] { _password, _confirm })
        {
            box.MaxLength = 128;
            box.UseSystemPasswordChar = true;
        }
        _password.AccessibleName = "Password";
        _confirm.AccessibleName = "Confirm password";

        _hint.Text = "Use at least 10 characters. A passphrase of several words works well.";
        _hint.AutoSize = true;
        _hint.MaximumSize = new Size(380, 0);
        _hint.Tag = "muted";
        _hint.Margin = new Padding(0, 6, 0, 0);

        _status.AutoSize = true;
        _status.MaximumSize = new Size(380, 0);
        _status.ForeColor = Theme.Danger;
        _status.Margin = new Padding(0, 10, 0, 4);

        _register.Text = "&Create account";
        _register.Dock = DockStyle.Fill;
        _register.Height = 38;
        _register.Tag = "primary";
        _register.Click += OnRegisterClick;

        AddField("&Full name", _name);
        AddField("&Email", _email);
        AddField("&Password", _password);
        AddField("C&onfirm password", _confirm);
        _layout.Controls.Add(_hint);
        _layout.Controls.Add(_status);
        _layout.Controls.Add(_register);
        Controls.Add(CreateCenteringHost(_layout));
        Theme.Apply(this);   // same look as the other screens, and opens maximized
        _status.ForeColor = Theme.Danger;   // Theme.Apply resets plain labels to dark text

        AcceptButton = _register;
        ActiveControl = _name;
        FormClosing += (_, e) => { if (_busy && DialogResult != DialogResult.OK) e.Cancel = true; };
        FormClosed += (_, _) => { _password.Clear(); _confirm.Clear(); };
    }


    // Full screen: the window is maximized (Theme.Apply) and the card is kept in the middle of it
    // at any resolution. The title bar and close button stay, so the app can always be closed.
    private static TableLayoutPanel CreateCenteringHost(Control card)
    {
        var host = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
        host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        host.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        host.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        host.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        host.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        host.Controls.Add(card, 1, 1);
        return host;
    }

    private void AddField(string label, TextBox input)
    {
        _layout.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 8, 0, 2) });
        input.Dock = DockStyle.Fill;
        _layout.Controls.Add(input);
    }

    private async void OnRegisterClick(object? sender, EventArgs e)
    {
        if (_busy) return;
        _status.Text = string.Empty;

        if (_password.Text != _confirm.Text)
        {
            _status.Text = "The passwords do not match.";
            _password.Clear();
            _confirm.Clear();
            _password.Focus();
            return;
        }

        SetBusy(true);
        try
        {
            var result = await _users.RegisterRequesterAsync(
                new RegisterRequesterDto(_name.Text, _email.Text, _password.Text));
            _password.Clear();
            _confirm.Clear();

            if (result.IsSuccess)
            {
                RegisteredEmail = result.Value.Email;
                DialogResult = DialogResult.OK;   // closes the dialog
                return;
            }

            _status.Text = result.Error.Message;
            _password.Focus();
        }
        catch (Exception)
        {
            _password.Clear();
            _confirm.Clear();
            _status.Text = "Registration is unavailable right now. Please try again later.";
        }
        finally
        {
            if (!IsDisposed) SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        UseWaitCursor = busy;
        _layout.Enabled = !busy;
    }
}
