using CivicConnect.Auth;

namespace CivicConnect.App.Forms;

/// <summary>
/// Sign-in screen. Security notes:
///  - password box masks input and is capped at the module's max length;
///  - the password is cleared after EVERY attempt and when the form closes;
///  - the form cannot be closed or double-submitted while a sign-in is in flight;
///  - only the module's generic messages are shown; unexpected exceptions are never displayed
///    (they can contain server names or connection details);
///  - nothing here logs, stores or remembers credentials.
/// LIMITATION: .NET strings are immutable, so a password cannot be wiped from memory on demand.
/// </summary>
internal sealed class LoginForm : Form
{
    private readonly IAuthService _auth;
    private readonly Func<RegisterForm> _createRegisterForm;

    private readonly TableLayoutPanel _layout = new();
    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly Label _status = new();
    private readonly Button _signIn = new();
    private readonly LinkLabel _createAccount = new();
    private bool _busy;

    public LoginForm(IAuthService auth, Func<RegisterForm> createRegisterForm)
    {
        _auth = auth;
        _createRegisterForm = createRegisterForm;

        Text = "CivicConnect - Sign in";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(360, 330);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;

        _layout.Dock = DockStyle.Fill;
        _layout.Padding = new Padding(16);
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _email.MaxLength = 254;
        _email.AccessibleName = "Email address";
        _email.AutoCompleteMode = AutoCompleteMode.None;

        _password.MaxLength = 128;
        _password.UseSystemPasswordChar = true;
        _password.AccessibleName = "Password";

        _status.AutoSize = true;
        _status.MaximumSize = new Size(320, 0);
        _status.Margin = new Padding(0, 10, 0, 4);

        _signIn.Text = "&Sign in";
        _signIn.Dock = DockStyle.Fill;
        _signIn.Height = 34;
        _signIn.Click += OnSignInClick;

        _createAccount.Text = "Create a requester account";
        _createAccount.AutoSize = true;
        _createAccount.Margin = new Padding(0, 12, 0, 0);
        _createAccount.LinkClicked += OnCreateAccountClicked;

        // Controls are added in tab order; label mnemonics jump to the next control.
        AddField("&Email", _email);
        AddField("&Password", _password);
        _layout.Controls.Add(_status);
        _layout.Controls.Add(_signIn);
        _layout.Controls.Add(_createAccount);
        Controls.Add(_layout);

        AcceptButton = _signIn;     // Enter submits
        ActiveControl = _email;
        FormClosing += OnFormClosing;
        FormClosed += (_, _) => _password.Clear();
    }

    private void AddField(string label, TextBox input)
    {
        _layout.Controls.Add(new Label { Text = label, AutoSize = true, Margin = new Padding(0, 8, 0, 2) });
        input.Dock = DockStyle.Fill;
        _layout.Controls.Add(input);
    }

    private async void OnSignInClick(object? sender, EventArgs e)
    {
        if (_busy) return;
        SetBusy(true);
        ShowMessage(null, isError: false);
        try
        {
            var result = await _auth.LoginAsync(new LoginDto(_email.Text, _password.Text));
            _password.Clear();

            if (result.IsSuccess)
            {
                DialogResult = DialogResult.OK;   // closes the dialog
                return;
            }

            ShowMessage(result.Error.Message, isError: true);
            _password.Focus();
        }
        catch (Exception)
        {
            // Database unreachable, etc. Deliberately generic. TODO: log the exception type/message
            // (never the DTO or credentials) to a secure logger once logging exists.
            _password.Clear();
            ShowMessage("Sign-in is unavailable right now. Please try again later.", isError: true);
        }
        finally
        {
            if (!IsDisposed) SetBusy(false);
        }
    }

    private void OnCreateAccountClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (_busy) return;
        using var form = _createRegisterForm();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            _email.Text = form.RegisteredEmail;   // email only; the password is never carried over
            _password.Clear();
            ShowMessage("Account created. Please sign in.", isError: false);
            _password.Focus();
        }
    }

    private void OnFormClosing(object? sender, FormClosingEventArgs e)
    {
        // Closing mid-login could leave a session open after the user "cancelled".
        if (_busy && DialogResult != DialogResult.OK) e.Cancel = true;
    }

    private void SetBusy(bool busy)
    {
        _busy = busy;
        UseWaitCursor = busy;
        _layout.Enabled = !busy;
    }

    private void ShowMessage(string? text, bool isError)
    {
        _status.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        _status.Text = text ?? string.Empty;
    }
}
