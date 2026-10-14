using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// Staff work queue (FR-007, FR-008). The service refuses anyone without the staff-queue permission,
/// so opening this form as a requester shows an error instead of data. Shows how long the load took,
/// which is the evidence for NFR-002 / PERF-01.
internal sealed partial class StaffQueueForm : Form
{
    private const string AllOpen = "All open";

    private readonly IRequestService _requests;
    private readonly IFormFactory _forms;
    private bool _busy;

    public StaffQueueForm(IRequestService requests, IFormFactory forms)
    {
        _requests = requests;
        _forms = forms;

        InitializeComponent();
        Theme.Apply(this);

        cboStatus.Items.Add(AllOpen);
        foreach (var s in Enum.GetValues<RequestStatus>()) cboStatus.Items.Add(s.ToString());
        cboStatus.SelectedIndex = 0;
        txtSearch.MaxLength = RequestRules.MaxSearchTextLength;
        AcceptButton = btnApply;

        Shown += async (_, _) => await LoadAsync();
    }

    private async void btnApply_Click(object? sender, EventArgs e) => await LoadAsync();
    private async void btnOpen_Click(object? sender, EventArgs e) => await OpenSelectedAsync();
    private async void dgvRequests_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) await OpenSelectedAsync(); }

    private async Task LoadAsync()
    {
        if (_busy) return;
        _busy = true;
        UseWaitCursor = true;
        try
        {
            RequestStatus? status = cboStatus.SelectedItem is string s && s != AllOpen
                ? Enum.Parse<RequestStatus>(s)
                : null;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _requests.GetStaffQueueAsync(new RequestQueueFilter(status, txtSearch.Text, chkMine.Checked));
            watch.Stop();

            if (result.IsFailure)
            {
                dgvRequests.DataSource = null;
                Theme.ShowMessage(lblStatus, result.Error.Message, isError: true);
                return;
            }

            dgvRequests.DataSource = result.Value.Select(RequestRow.From).ToList();
            Theme.ShowMessage(lblStatus, $"{result.Value.Count} request(s) in {watch.ElapsedMilliseconds} ms", isError: false);
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

    private async Task OpenSelectedAsync()
    {
        if (dgvRequests.CurrentRow?.DataBoundItem is not RequestRow row) return;
        using (var form = _forms.CreateRequestDetailForm(row.Id))
            form.ShowDialog(this);
        await LoadAsync();   // pick up whatever the staff member just changed
    }
}
