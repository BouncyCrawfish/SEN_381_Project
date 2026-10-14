using CivicConnect.AuditLog.Contracts;
using CivicConnect.Auth;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// Manager audit trail (NFR-005): every status change ever recorded in request_history, newest first,
/// filterable by request, new status and date range. Strictly read-only. The service caps a query at
/// 500 rows, so the status line says when the list may be cut off.
/// The screen is built in code (no Designer file) and styled by Theme through the Tag values.
internal sealed class AuditTrailForm : Form
{
    private const string AllStatuses = "All statuses";
    private const int ServiceRowCap = 500;   // mirrors AuditLogService.MaxRows

    private readonly IAuditLogService _audit;
    private readonly IUserService _users;
    private readonly IAccessControl _access;

    // Names of the people who made changes, looked up once per session of this screen.
    private readonly Dictionary<long, string> _names = new();

    private readonly TextBox _txtRequestId = new();
    private readonly ComboBox _cboStatus = new();
    private readonly DateTimePicker _dtpFrom = new();
    private readonly DateTimePicker _dtpTo = new();
    private readonly Button _btnApply = new();
    private readonly Button _btnClear = new();
    private readonly Button _btnBack = new();
    private readonly Label _lblStatus = new();
    private readonly DataGridView _grid = new();
    private bool _busy;

    public AuditTrailForm(IAuditLogService audit, IUserService users, IAccessControl access)
    {
        _audit = audit;
        _users = users;
        _access = access;

        Text = "CivicConnect - Audit trail";
        ClientSize = new Size(1200, 720);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;

        BuildLayout();
        Theme.Apply(this);

        _cboStatus.Items.Add(AllStatuses);
        foreach (var s in Enum.GetValues<RequestStatus>()) _cboStatus.Items.Add(s.ToString());
        _cboStatus.SelectedIndex = 0;

        AcceptButton = _btnApply;
        CancelButton = _btnBack;
        Shown += async (_, _) => await LoadAsync();
    }

    private void BuildLayout()
    {
        // Header bar: title and subtitle on the left, Back on the right.
        var header = new Panel { Dock = DockStyle.Top, Height = 84, Tag = "header" };
        var title = new Label { AutoSize = true, Location = new Point(28, 10), Tag = "title", Text = "Audit trail" };
        var subtitle = new Label
        {
            AutoSize = true,
            Location = new Point(30, 52),
            Tag = "subtitle",
            Text = "Every status change ever recorded. Read-only: entries can never be edited or deleted."
        };
        var headerRight = new Panel { Dock = DockStyle.Right, Width = 150, BackColor = Color.Transparent };
        _btnBack.Text = "&Back";
        _btnBack.Tag = "ghost";
        _btnBack.Size = new Size(120, 40);
        _btnBack.Location = new Point(0, 22);
        _btnBack.DialogResult = DialogResult.Cancel;
        headerRight.Controls.Add(_btnBack);
        header.Controls.Add(headerRight);
        header.Controls.Add(subtitle);
        header.Controls.Add(title);

        // Filters.
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 12),
            WrapContents = true
        };
        _txtRequestId.Width = 110;
        _txtRequestId.MaxLength = 18;
        _txtRequestId.PlaceholderText = "Request #";
        _txtRequestId.Margin = new Padding(0, 4, 16, 0);

        _cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboStatus.Width = 170;
        _cboStatus.Margin = new Padding(0, 4, 16, 0);

        _dtpFrom.Format = DateTimePickerFormat.Short;
        _dtpFrom.ShowCheckBox = true;
        _dtpFrom.Checked = false;
        _dtpFrom.Width = 130;
        _dtpFrom.Margin = new Padding(0, 4, 12, 0);

        _dtpTo.Format = DateTimePickerFormat.Short;
        _dtpTo.ShowCheckBox = true;
        _dtpTo.Checked = false;
        _dtpTo.Width = 130;
        _dtpTo.Margin = new Padding(0, 4, 16, 0);

        _btnApply.Text = "&Apply";
        _btnApply.Tag = "primary";
        _btnApply.Size = new Size(110, 36);
        _btnApply.Margin = new Padding(0, 0, 10, 0);
        _btnApply.Click += async (_, _) => await LoadAsync();

        _btnClear.Text = "&Clear";
        _btnClear.Tag = "secondary";
        _btnClear.Size = new Size(110, 36);
        _btnClear.Margin = new Padding(0, 0, 16, 0);
        _btnClear.Click += async (_, _) => { ClearFilters(); await LoadAsync(); };

        _lblStatus.AutoSize = true;
        _lblStatus.Tag = "muted";
        _lblStatus.Margin = new Padding(0, 8, 0, 0);

        toolbar.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0, 8, 8, 0), Text = "Request" });
        toolbar.Controls.Add(_txtRequestId);
        toolbar.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0, 8, 8, 0), Text = "Status" });
        toolbar.Controls.Add(_cboStatus);
        toolbar.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0, 8, 8, 0), Text = "From" });
        toolbar.Controls.Add(_dtpFrom);
        toolbar.Controls.Add(new Label { AutoSize = true, Margin = new Padding(0, 8, 8, 0), Text = "To" });
        toolbar.Controls.Add(_dtpTo);
        toolbar.Controls.Add(_btnApply);
        toolbar.Controls.Add(_btnClear);
        toolbar.Controls.Add(_lblStatus);

        // Results grid.
        _grid.Dock = DockStyle.Fill;
        _grid.AutoGenerateColumns = false;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.ReadOnly = true;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.Columns.Add(Column("When", nameof(AuditRow.When), 18));
        _grid.Columns.Add(Column("Request", nameof(AuditRow.Request), 10));
        _grid.Columns.Add(Column("From", nameof(AuditRow.From), 16));
        _grid.Columns.Add(Column("To", nameof(AuditRow.Status), 16));      // "Status" property: Theme colours it
        _grid.Columns.Add(Column("Changed by", nameof(AuditRow.ChangedBy), 40));

        // Card and page body. Fill control first, then the Top control, so the toolbar sits above the grid.
        var card = new Panel { Dock = DockStyle.Fill, Tag = "card" };
        card.Controls.Add(_grid);
        card.Controls.Add(toolbar);

        var body = new Panel { Dock = DockStyle.Fill, Padding = new Padding(28) };
        body.Controls.Add(card);

        Controls.Add(body);
        Controls.Add(header);
    }

    private static DataGridViewTextBoxColumn Column(string header, string property, float fillWeight) => new()
    {
        HeaderText = header,
        DataPropertyName = property,
        FillWeight = fillWeight,
        SortMode = DataGridViewColumnSortMode.NotSortable
    };

    private void ClearFilters()
    {
        _txtRequestId.Clear();
        _cboStatus.SelectedIndex = 0;
        _dtpFrom.Checked = false;
        _dtpTo.Checked = false;
    }

    private async Task LoadAsync()
    {
        if (_busy) return;
        _busy = true;
        UseWaitCursor = true;
        try
        {
            // Fail closed, like every service method: only a user who may view reports gets this data.
            var allowed = _access.Require(Permission.ViewReports);
            if (allowed.IsFailure) { Theme.ShowMessage(_lblStatus, allowed.Error.Message, isError: true); return; }

            if (!TryReadQuery(out var query, out var problem))
            {
                Theme.ShowMessage(_lblStatus, problem, isError: true);
                return;
            }

            var result = await _audit.QueryAsync(query);
            if (result.IsFailure) { Theme.ShowMessage(_lblStatus, result.Error.Message, isError: true); return; }

            // The service returns oldest first; an audit trail reads best newest first.
            var entries = result.Value.OrderByDescending(e => e.Timestamp).ThenByDescending(e => e.Id).ToList();
            await ResolveNamesAsync(entries);

            _grid.DataSource = entries.Select(e => AuditRow.Create(e, NameOf(e.ChangedBy))).ToList();

            var text = entries.Count >= ServiceRowCap
                ? $"{entries.Count} entries shown (limit reached, narrow the filters to see the rest)"
                : $"{entries.Count} entr{(entries.Count == 1 ? "y" : "ies")}";
            Theme.ShowMessage(_lblStatus, text, isError: false);
            _lblStatus.ForeColor = Theme.Muted;
        }
        catch (Exception)
        {
            Theme.ShowMessage(_lblStatus, Ui.Unavailable, isError: true);
        }
        finally
        {
            _busy = false;
            if (!IsDisposed) UseWaitCursor = false;
        }
    }

    // Turns the filter controls into a query. Returns false with a message the user can act on.
    private bool TryReadQuery(out AuditQueryDto query, out string problem)
    {
        query = new AuditQueryDto();
        problem = string.Empty;

        long? requestId = null;
        var idText = _txtRequestId.Text.Trim().TrimStart('#');
        if (idText.Length > 0)
        {
            if (!long.TryParse(idText, out var id) || id <= 0)
            {
                problem = "Request must be a positive number.";
                return false;
            }
            requestId = id;
        }

        string? status = _cboStatus.SelectedItem is string s && s != AllStatuses ? s : null;

        // Whole days in the user's local time: From = start of that day, To = last tick of that day.
        DateTimeOffset? fromDate = _dtpFrom.Checked ? new DateTimeOffset(_dtpFrom.Value.Date) : null;
        DateTimeOffset? toDate = _dtpTo.Checked ? new DateTimeOffset(_dtpTo.Value.Date.AddDays(1).AddTicks(-1)) : null;
        if (fromDate > toDate)
        {
            problem = "'From' must not be after 'To'.";
            return false;
        }

        query = new AuditQueryDto(requestId, null, status, fromDate, toDate);
        return true;
    }

    // One lookup per distinct person, not per row. A failed lookup falls back to the user id.
    private async Task ResolveNamesAsync(IEnumerable<AuditEntryDto> entries)
    {
        var missing = entries.Select(e => e.ChangedBy).Distinct().Where(uid => !_names.ContainsKey(uid)).ToList();
        foreach (var id in missing)
        {
            var user = await _users.GetByIdAsync(id);
            _names[id] = user.IsSuccess ? $"{user.Value.Name} ({user.Value.Role})" : $"User #{id}";
        }
    }

    private string NameOf(long userId) => _names.TryGetValue(userId, out var n) ? n : $"User #{userId}";

    /// Flat row for the grid (DataGridView binds to public properties). "Status" is the new status.
    private sealed record AuditRow(string When, string Request, string From, string Status, string ChangedBy)
    {
        public static AuditRow Create(AuditEntryDto e, string changedBy) => new(
            Ui.Local(e.Timestamp.UtcDateTime),
            $"#{e.RequestId}",
            e.OldStatus ?? "(new request)",
            e.NewStatus,
            changedBy);
    }
}