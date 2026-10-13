using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// <summary>
/// Requester form to submit a request (FR-001, FR-002). Security notes:
///  - there is NO requester field: the service takes the requester from the signed-in session, so
///    the form cannot submit on someone else's behalf;
///  - the category is a drop-down filled from the database (no free text, FR-002);
///  - MaxLength mirrors the module's limits, but the REAL validation is in the service, which also
///    trims and rejects bad input; the form only displays the service's field-level messages;
///  - the button is disabled while a submit is in flight (no double submit);
///  - unexpected exceptions are never shown to the user (they can contain connection details).
/// </summary>
internal sealed class SubmitRequestForm : Form
{
    private readonly IRequestService _requests;

    private readonly TableLayoutPanel _layout = new();
    private readonly ComboBox _category = new();
    private readonly TextBox _location = new();
    private readonly TextBox _description = new();
    private readonly Label _counter = new();
    private readonly Label _status = new();
    private readonly Button _submit = new();
    private bool _busy;

    /// <summary>Reference number of the new request, set when submission succeeds.</summary>
    public long SubmittedRequestId { get; private set; }

    public SubmitRequestForm(IRequestService requests)
    {
        _requests = requests;

        Text = "CivicConnect - Submit a request";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(440, 470);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;

        _layout.Dock = DockStyle.Fill;
        _layout.Padding = new Padding(16);
        _layout.ColumnCount = 1;
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _category.DropDownStyle = ComboBoxStyle.DropDownList;   // list only: no free text
        _category.DisplayMember = nameof(CategoryDto.Name);
        _category.ValueMember = nameof(CategoryDto.Id);
        _category.AccessibleName = "Category";
        _category.Enabled = false;                              // until the list has loaded

        _location.MaxLength = RequestRules.MaxLocationLength;
        _location.AccessibleName = "Location";

        _description.Multiline = true;
        _description.ScrollBars = ScrollBars.Vertical;
        _description.Height = 140;
        _description.MaxLength = RequestRules.MaxDescriptionLength;
        _description.AccessibleName = "Description";
        _description.TextChanged += (_, _) => UpdateCounter();

        _counter.AutoSize = true;
        _counter.ForeColor = SystemColors.GrayText;
        UpdateCounter();

        _status.AutoSize = true;
        _status.MaximumSize = new Size(400, 0);
        _status.Margin = new Padding(0, 10, 0, 4);

        _submit.Text = "&Submit request";
        _submit.Dock = DockStyle.Fill;
        _submit.Height = 34;
        _submit.Enabled = false;
        _submit.Click += OnSubmitClick;

        AddField("&Category", _category);
        AddField("&Location (where is the problem?)", _location);
        AddField("&Description", _description);
        _layout.Controls.Add(_counter);
        _layout.Controls.Add(_status);
        _layout.Controls.Add(_submit);
        Controls.Add(_layout);

        AcceptButton = null;               // Enter must not submit from inside the multi-line box
        FormClosing += (_, e) => { if (_busy) e.Cancel = true; };
        Shown += OnShown;
    }

    private void AddField(string label, Control input)
    {
        _layout.Controls.Add(Ui.FieldLabel(label));
        input.Dock = DockStyle.Fill;
        _layout.Controls.Add(input);
    }

    private void UpdateCounter() =>
        _counter.Text = $"{_description.TextLength} / {RequestRules.MaxDescriptionLength} characters " +
                        $"(at least {RequestRules.MinDescriptionLength})";

    private async void OnShown(object? sender, EventArgs e)
    {
        SetBusy(true);
        try
        {
            var result = await _requests.ListCategoriesAsync();
            if (result.IsFailure)
            {
                ShowMessage(result.Error.Message, isError: true);
                return;
            }

            _category.DataSource = result.Value.ToList();
            _category.SelectedIndex = -1;   // force a conscious choice (FR-002)
            _category.Enabled = true;
            _submit.Enabled = true;
        }
        catch (Exception)
        {
            // TODO: log the exception type/message (never user input) to a secure logger.
            ShowMessage(Ui.Unavailable, isError: true);
        }
        finally
        {
            if (!IsDisposed) SetBusy(false);
        }
    }

    private async void OnSubmitClick(object? sender, EventArgs e)
    {
        if (_busy) return;
        SetBusy(true);
        ShowMessage(null, isError: false);
        try
        {
            var categoryId = _category.SelectedValue is long id ? id : 0;   // 0 is rejected by the service
            var result = await _requests.SubmitRequestAsync(
                new SubmitRequestDto(categoryId, _location.Text, _description.Text));

            if (result.IsFailure)
            {
                ShowMessage(result.Error.Message, isError: true);   // field-level messages from the service
                return;
            }

            SubmittedRequestId = result.Value;
            Ui.ShowInfo(this, $"Your request has been submitted.\n\nReference number: #{result.Value}\nStatus: Submitted");
            DialogResult = DialogResult.OK;
        }
        catch (Exception)
        {
            ShowMessage(Ui.Unavailable, isError: true);
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
        _location.Enabled = !busy;
        _description.Enabled = !busy;
        if (busy) { _submit.Enabled = false; _category.Enabled = false; }
        else if (_category.DataSource is not null) { _submit.Enabled = true; _category.Enabled = true; }
    }

    private void ShowMessage(string? text, bool isError)
    {
        _status.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        _status.Text = text ?? string.Empty;
    }
}
