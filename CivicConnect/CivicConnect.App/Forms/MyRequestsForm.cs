using CivicConnect.Requests.Contracts;

namespace CivicConnect.App.Forms;

///A requester's own requests with their current status (FR-003, FR-004). Read for the signed-in user only.
internal sealed partial class MyRequestsForm : Form
{
    private readonly IRequestService _requests;
    private readonly IFormFactory _forms;
    private bool _busy;

    public MyRequestsForm(IRequestService requests, IFormFactory forms)
    {
        _requests = requests;
        _forms = forms;

        InitializeComponent();
        Theme.Apply(this);

        Shown += async (_, _) => await LoadAsync();
    }

    private async void btnRefresh_Click(object? sender, EventArgs e) => await LoadAsync();
    private void btnOpen_Click(object? sender, EventArgs e) => OpenSelected();
    private void dgvRequests_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) OpenSelected(); }

    private async Task LoadAsync()
    {
        if (_busy) return;
        _busy = true;
        UseWaitCursor = true;
        try
        {
            var result = await _requests.GetMyRequestsAsync();
            if (result.IsFailure) { Theme.ShowMessage(lblStatus, result.Error.Message, isError: true); return; }

            dgvRequests.DataSource = result.Value.Select(RequestRow.From).ToList();
            Theme.ShowMessage(lblStatus, $"{result.Value.Count} request(s)", isError: false);
            lblStatus.ForeColor = Theme.Muted;
        }
        catch (Exception)
        {
            Theme.ShowMessage(lblStatus, Ui.Unavailable, isError: true);
        }
        finally
        {
            _busy = false;
            if (!IsDisposed) UseWaitCursor = false;
        }
    }

    private void OpenSelected()
    {
        if (dgvRequests.CurrentRow?.DataBoundItem is not RequestRow row) return;
        using var form = _forms.CreateRequestDetailForm(row.Id);
        form.ShowDialog(this);
    }
}
