using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// Manager overview (ViewAllRequests + ViewReports): counts by status and category, plus every request
/// including closed ones. Strictly read-only: managers can open a request to read its history, but the
/// detail screen shows no response panel to them and the service refuses any change from a manager.
internal sealed partial class ManagerDashboardForm : Form
{
    private const string AllStatuses = "All statuses";

    private readonly IRequestService _requests;
    private readonly IFormFactory _forms;
    private bool _busy;

    public ManagerDashboardForm(IRequestService requests, IFormFactory forms)
    {
        _requests = requests;
        _forms = forms;

        InitializeComponent();
        Theme.Apply(this);

        cboStatus.Items.Add(AllStatuses);
        foreach (var s in Enum.GetValues<RequestStatus>()) cboStatus.Items.Add(s.ToString());
        cboStatus.SelectedIndex = 0;
        txtSearch.MaxLength = RequestRules.MaxSearchTextLength;
        AcceptButton = btnApply;

        Shown += async (_, _) => await LoadAsync();
    }

    private async void btnApply_Click(object? sender, EventArgs e) => await LoadAsync();
    private async void btnOpen_Click(object? sender, EventArgs e) => await OpenSelectedAsync();
    private void btnAudit_Click(object? sender, EventArgs e)
    {
        using var form = _forms.CreateAuditTrailForm();
        form.ShowDialog(this);
    }
    private async void dgvRequests_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) { if (e.RowIndex >= 0) await OpenSelectedAsync(); }

    private async Task LoadAsync()
    {
        if (_busy) return;
        _busy = true;
        UseWaitCursor = true;
        try
        {
            RequestStatus? status = cboStatus.SelectedItem is string s && s != AllStatuses
                ? Enum.Parse<RequestStatus>(s)
                : null;

            var summary = await _requests.GetSummaryAsync();
            var list = await _requests.GetAllRequestsAsync(new RequestQueueFilter(status, txtSearch.Text));

            if (summary.IsFailure) { Theme.ShowMessage(lblStatus, summary.Error.Message, isError: true); return; }
            if (list.IsFailure) { Theme.ShowMessage(lblStatus, list.Error.Message, isError: true); return; }

            ShowSummary(summary.Value);
            dgvRequests.DataSource = list.Value.Select(RequestRow.From).ToList();
            Theme.ShowMessage(lblStatus, $"{list.Value.Count} request(s)", isError: false);
            lblStatus.ForeColor = Theme.Muted;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);   // Debug builds only: full error goes to the Output window
            Theme.ShowMessage(lblStatus, Ui.Unavailable, isError: true);
        }
        finally
        {
            _busy = false;
            if (!IsDisposed) UseWaitCursor = false;
        }
    }

    private void ShowSummary(RequestSummaryDto summary)
    {
        lblKpiTotalValue.Text = summary.Total.ToString();
        lblKpiSubmittedValue.Text = Count(summary, nameof(RequestStatus.Submitted));
        lblKpiAssignedValue.Text = Count(summary, nameof(RequestStatus.Assigned));
        lblKpiInProgressValue.Text = Count(summary, nameof(RequestStatus.InProgress));
        lblKpiResolvedValue.Text = Count(summary, nameof(RequestStatus.Resolved));
        lblKpiClosedValue.Text = Count(summary, nameof(RequestStatus.Closed));

        lstCategories.Items.Clear();
        foreach (var (name, count) in summary.ByCategory)
            lstCategories.Items.Add($"{name}: {count}");
    }

    private static string Count(RequestSummaryDto summary, string status) =>
        (summary.ByStatus.TryGetValue(status, out var n) ? n : 0).ToString();

    private async Task OpenSelectedAsync()
    {
        if (dgvRequests.CurrentRow?.DataBoundItem is not RequestRow row) return;
        using (var form = _forms.CreateRequestDetailForm(row.Id))
            form.ShowDialog(this);
        await LoadAsync();
    }
}
