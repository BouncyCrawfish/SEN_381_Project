using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// Requester form to submit a request (FR-001, FR-002). Security notes:
///  - there is NO requester field: the service takes the requester from the signed-in session, so
///    the form cannot submit on someone else's behalf;
///  - the category is a drop-down filled from the database (no free text, FR-002);
///  - MaxLength mirrors the module's limits, but the REAL validation is in the service, which also
///    trims and rejects bad input; the form only displays the service's field-level messages;
///  - the button is disabled while a submit is in flight (no double submit);
///  - unexpected exceptions are never shown to the user (they can contain connection details).
internal sealed partial class SubmitRequestForm : Form
{
    private readonly IRequestService _requests;
    private bool _busy;
    private bool _categoriesLoaded;

    //Reference number of the new request, set when submission succeeds.
    public long SubmittedRequestId { get; private set; }

    public SubmitRequestForm(IRequestService requests)
    {
        _requests = requests;

        InitializeComponent();
        Theme.Apply(this);

        // Limits come from the same constants the service validates with.
        txtLocation.MaxLength = RequestRules.MaxLocationLength;
        txtDescription.MaxLength = RequestRules.MaxDescriptionLength;
        UpdateCounter();

        FormClosing += (_, e) => { if (_busy) e.Cancel = true; };
        Shown += OnShown;
    }

    private void txtDescription_TextChanged(object? sender, EventArgs e) => UpdateCounter();

    private void UpdateCounter() =>
        lblCounter.Text = $"{txtDescription.TextLength} / {RequestRules.MaxDescriptionLength} characters " +
                          $"(at least {RequestRules.MinDescriptionLength})";

    private async void OnShown(object? sender, EventArgs e)
    {
        SetBusy(true);
        try
        {
            var result = await _requests.ListCategoriesAsync();
            if (result.IsFailure)
            {
                Theme.ShowMessage(lblMessage, result.Error.Message, isError: true);
                return;
            }

            cboCategory.DataSource = result.Value.ToList();
            cboCategory.DisplayMember = nameof(CategoryDto.Name);
            cboCategory.ValueMember = nameof(CategoryDto.Id);
            cboCategory.SelectedIndex = -1;   // force a conscious choice (FR-002)
            _categoriesLoaded = true;
        }
        catch (Exception)
        {
            // TODO: log the exception type/message (never user input) to a secure logger.
            Theme.ShowMessage(lblMessage, Ui.Unavailable, isError: true);
        }
        finally
        {
            if (!IsDisposed) SetBusy(false);
        }
    }

    private async void btnSubmit_Click(object? sender, EventArgs e)
    {
        if (_busy) return;
        SetBusy(true);
        Theme.ShowMessage(lblMessage, null, isError: false);
        try
        {
            var categoryId = cboCategory.SelectedValue is long id ? id : 0;   // 0 is rejected by the service
            var result = await _requests.SubmitRequestAsync(
                new SubmitRequestDto(categoryId, txtLocation.Text, txtDescription.Text));

            if (result.IsFailure)
            {
                Theme.ShowMessage(lblMessage, result.Error.Message, isError: true);   // field-level messages from the service
                return;
            }

            SubmittedRequestId = result.Value;
            Ui.ShowInfo(this, $"Your request has been submitted.\n\nReference number: #{result.Value}\nStatus: Submitted");
            DialogResult = DialogResult.OK;
        }
        catch (Exception)
        {
            Theme.ShowMessage(lblMessage, Ui.Unavailable, isError: true);
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
        txtLocation.Enabled = !busy;
        txtDescription.Enabled = !busy;
        cboCategory.Enabled = !busy && _categoriesLoaded;
        btnSubmit.Enabled = !busy && _categoriesLoaded;
    }
}
