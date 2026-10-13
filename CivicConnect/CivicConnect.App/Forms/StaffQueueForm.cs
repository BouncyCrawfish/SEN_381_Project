using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// <summary>
/// Staff work queue (FR-007, FR-008). The service refuses anyone without the staff-queue permission,
/// so opening this form as a requester shows an error instead of data. Shows how long the load took,
/// which is the evidence for NFR-002 / PERF-01.
/// </summary>
internal sealed class StaffQueueForm : Form
{
    private const string AllOpen = "All open";

    private readonly IRequestService _requests;
    private readonly IFormFactory _forms;

    private readonly DataGridView _grid = Ui.CreateGrid();
    private readonly ComboBox _statusFilter = new();
    private readonly TextBox _search = new();
    private readonly CheckBox _mine = new();
    private readonly Label _status = new();
    private bool _busy;

    public StaffQueueForm(IRequestService requests, IFormFactory forms)
    {
        _requests = requests;
        _forms = forms;

        Text = "CivicConnect - Staff queue";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(900, 480);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;

        _statusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _statusFilter.Items.Add(AllOpen);
        foreach (var s in Enum.GetValues<RequestStatus>()) _statusFilter.Items.Add(s.ToString());
        _statusFilter.SelectedIndex = 0;
        _statusFilter.AccessibleName = "Status filter";

        _search.MaxLength = RequestRules.MaxSearchTextLength;
        _search.Width = 180;
        _search.PlaceholderText = "Search description or location";
        _search.AccessibleName = "Search";

        _mine.Text = "Assigned to me";
        _mine.AutoSize = true;

        var apply = new Button { Text = "&Apply", AutoSize = true };
        apply.Click += async (_, _) => await LoadAsync();
        var open = new Button { Text = "&Open / respond", AutoSize = true };
        open.Click += async (_, _) => await OpenSelectedAsync();
        AcceptButton = apply;

        _status.AutoSize = true;
        _status.Margin = new Padding(12, 8, 0, 0);

        var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8, 6, 8, 0) };
        bar.Controls.AddRange([new Label { Text = "Status:", AutoSize = true, Margin = new Padding(0, 8, 0, 0) },
            _statusFilter, _search, _mine, apply, open, _status]);

        _grid.Columns.Add(Ui.Column("Ref", nameof(RequestRow.Id), 35));
        _grid.Columns.Add(Ui.Column("Category", nameof(RequestRow.Category), 80));
        _grid.Columns.Add(Ui.Column("Location", nameof(RequestRow.Location), 120));
        _grid.Columns.Add(Ui.Column("Status", nameof(RequestRow.Status), 65));
        _grid.Columns.Add(Ui.Column("Assigned to", nameof(RequestRow.Assigned), 60));
        _grid.Columns.Add(Ui.Column("Submitted", nameof(RequestRow.Submitted), 85));
        _grid.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await OpenSelectedAsync(); };

        Controls.Add(_grid);
        Controls.Add(bar);
        Shown += async (_, _) => await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (_busy) return;
        _busy = true;
        UseWaitCursor = true;
        try
        {
            RequestStatus? status = _statusFilter.SelectedItem is string s && s != AllOpen
                ? Enum.Parse<RequestStatus>(s)
                : null;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = await _requests.GetStaffQueueAsync(new RequestQueueFilter(status, _search.Text, _mine.Checked));
            watch.Stop();

            if (result.IsFailure) { _grid.DataSource = null; _status.Text = result.Error.Message; return; }

            _grid.DataSource = result.Value.Select(RequestRow.From).ToList();
            _status.Text = $"{result.Value.Count} request(s) in {watch.ElapsedMilliseconds} ms";
        }
        catch (Exception)
        {
            _status.Text = Ui.Unavailable;
        }
        finally
        {
            _busy = false;
            if (!IsDisposed) UseWaitCursor = false;
        }
    }

    private async Task OpenSelectedAsync()
    {
        if (_grid.CurrentRow?.DataBoundItem is not RequestRow row) return;
        using (var form = _forms.CreateRequestDetailForm(row.Id))
            form.ShowDialog(this);
        await LoadAsync();   // pick up whatever the staff member just changed
    }
}
