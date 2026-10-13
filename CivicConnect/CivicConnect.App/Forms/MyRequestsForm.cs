using CivicConnect.Requests.Contracts;

namespace CivicConnect.App.Forms;

/// <summary>A requester's own requests with their current status (FR-003, FR-004). The list is read for the signed-in user only.</summary>
internal sealed class MyRequestsForm : Form
{
    private readonly IRequestService _requests;
    private readonly IFormFactory _forms;

    private readonly DataGridView _grid = Ui.CreateGrid();
    private readonly Label _status = new();
    private readonly Button _refresh = new();
    private readonly Button _open = new();
    private bool _busy;

    public MyRequestsForm(IRequestService requests, IFormFactory forms)
    {
        _requests = requests;
        _forms = forms;

        Text = "CivicConnect - My requests";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(760, 420);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;

        _grid.Columns.Add(Ui.Column("Ref", nameof(RequestRow.Id), 40));
        _grid.Columns.Add(Ui.Column("Category", nameof(RequestRow.Category), 90));
        _grid.Columns.Add(Ui.Column("Location", nameof(RequestRow.Location), 120));
        _grid.Columns.Add(Ui.Column("Status", nameof(RequestRow.Status), 70));
        _grid.Columns.Add(Ui.Column("Submitted", nameof(RequestRow.Submitted), 90));
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenSelected(); };

        _refresh.Text = "&Refresh";
        _refresh.Click += async (_, _) => await LoadAsync();
        _open.Text = "&Open";
        _open.Click += (_, _) => OpenSelected();

        var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 0) };
        bar.Controls.AddRange([_refresh, _open, _status]);
        _status.AutoSize = true;
        _status.Margin = new Padding(12, 8, 0, 0);

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
            var result = await _requests.GetMyRequestsAsync();
            if (result.IsFailure) { _status.Text = result.Error.Message; return; }
            _grid.DataSource = result.Value.Select(RequestRow.From).ToList();
            _status.Text = $"{result.Value.Count} request(s)";
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

    private void OpenSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not RequestRow row) return;
        using var form = _forms.CreateRequestDetailForm(row.Id);
        form.ShowDialog(this);
    }
}

/// <summary>Flat row for grids (DataGridView binds to public properties).</summary>
internal sealed record RequestRow(long Id, string Category, string Location, string Status, string Assigned, string Submitted)
{
    public static RequestRow From(RequestDto r) => new(
        r.Id, r.Category, r.Location, r.Status,
        r.AssignedStaffId is { } id ? $"#{id}" : "-",
        Ui.Local(r.CreatedAt));
}
