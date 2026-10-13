using CivicConnect.Auth;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// <summary>
/// One request in full (FR-009). Requesters get a read-only view of their own request. Staff also get
/// the response panel: assign (FR-010), move to the next status (FR-011), add a note (FR-012) and
/// resolve/close (FR-013). Security notes:
///  - the response panel is only SHOWN to staff, but the service re-checks the permission, the
///    assignee and the state transition on every call, so a hidden panel is not the protection;
///  - the next-status list offers only what the state machine allows, but the domain re-validates;
///  - the requester's contact details arrive only when the service decides the viewer may see them;
///  - controls are locked while a call is in flight (no double submit), and exceptions are never shown.
/// </summary>
internal sealed class RequestDetailForm : Form
{
    private readonly IRequestService _requests;
    private readonly IUserService _users;
    private readonly IAccessControl _access;
    private readonly long _requestId;

    private readonly Label _summary = new();
    private readonly TextBox _description = new();
    private readonly ListBox _history = new();
    private readonly ListBox _notes = new();
    private readonly Label _message = new();

    private readonly GroupBox _respond = new();
    private readonly ComboBox _staff = new();
    private readonly ComboBox _nextStatus = new();
    private readonly TextBox _note = new();
    private readonly Button _assign = new();
    private readonly Button _applyStatus = new();
    private readonly Button _addNote = new();

    private readonly Dictionary<long, string> _names = new();
    private RequestDetailDto? _detail;
    private bool _busy;

    public RequestDetailForm(IRequestService requests, IUserService users, IAccessControl access, long requestId)
    {
        _requests = requests;
        _users = users;
        _access = access;
        _requestId = requestId;

        Text = $"CivicConnect - Request #{requestId}";
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(720, 700);
        StartPosition = FormStartPosition.CenterParent;
        ShowInTaskbar = false;
        MinimizeBox = false;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, AutoScroll = true };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        _summary.AutoSize = true;
        _summary.MaximumSize = new Size(680, 0);

        _description.Multiline = true;
        _description.ReadOnly = true;
        _description.ScrollBars = ScrollBars.Vertical;
        _description.Height = 90;
        _description.Dock = DockStyle.Fill;

        _history.Height = 90; _history.Dock = DockStyle.Fill; _history.HorizontalScrollBar = true;
        _notes.Height = 90; _notes.Dock = DockStyle.Fill; _notes.HorizontalScrollBar = true;

        _message.AutoSize = true;
        _message.MaximumSize = new Size(680, 0);
        _message.Margin = new Padding(0, 6, 0, 6);

        layout.Controls.Add(_summary);
        layout.Controls.Add(Ui.FieldLabel("Description"));
        layout.Controls.Add(_description);
        layout.Controls.Add(Ui.FieldLabel("History (cannot be edited)"));
        layout.Controls.Add(_history);

        var notesLabel = Ui.FieldLabel("Staff notes");
        notesLabel.Name = "notesLabel";
        layout.Controls.Add(notesLabel);
        layout.Controls.Add(_notes);

        BuildRespondPanel();
        layout.Controls.Add(_message);
        layout.Controls.Add(_respond);

        // Requesters never see the staff-only parts (the service also withholds the data).
        var staffSide = _access.HasPermission(Permission.ViewRequestHistory);
        notesLabel.Visible = staffSide;
        _notes.Visible = staffSide;
        _respond.Visible = _access.HasPermission(Permission.UpdateRequestStatus)
                        || _access.HasPermission(Permission.AssignRequest);

        Controls.Add(layout);
        FormClosing += (_, e) => { if (_busy) e.Cancel = true; };
        Shown += async (_, _) => await ReloadAsync();
    }

    private void BuildRespondPanel()
    {
        _respond.Text = "Respond to this request";
        _respond.Dock = DockStyle.Fill;
        _respond.AutoSize = true;
        _respond.Padding = new Padding(10);

        var panel = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _staff.DropDownStyle = ComboBoxStyle.DropDownList;
        _staff.DisplayMember = nameof(UserDto.Name);
        _staff.ValueMember = nameof(UserDto.Id);
        _staff.Dock = DockStyle.Fill;
        _staff.AccessibleName = "Staff member to assign";

        _nextStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        _nextStatus.Dock = DockStyle.Fill;
        _nextStatus.AccessibleName = "Next status";

        _note.Multiline = true;
        _note.ScrollBars = ScrollBars.Vertical;
        _note.Height = 70;
        _note.MaxLength = RequestRules.MaxNoteLength;
        _note.Dock = DockStyle.Fill;
        _note.AccessibleName = "Note";

        _assign.Text = "&Assign";
        _assign.AutoSize = true;
        _assign.Click += async (_, _) => await AssignAsync();
        _applyStatus.Text = "Apply &status";
        _applyStatus.AutoSize = true;
        _applyStatus.Click += async (_, _) => await ApplyStatusAsync();
        _addNote.Text = "Add &note only";
        _addNote.AutoSize = true;
        _addNote.Click += async (_, _) => await AddNoteAsync();

        panel.Controls.Add(Ui.FieldLabel("Assign to"), 0, 0);
        panel.SetColumnSpan(panel.GetControlFromPosition(0, 0)!, 2);
        panel.Controls.Add(_staff, 0, 1);
        panel.Controls.Add(_assign, 1, 1);

        panel.Controls.Add(Ui.FieldLabel("Move to"), 0, 2);
        panel.SetColumnSpan(panel.GetControlFromPosition(0, 2)!, 2);
        panel.Controls.Add(_nextStatus, 0, 3);
        panel.Controls.Add(_applyStatus, 1, 3);

        panel.Controls.Add(Ui.FieldLabel("Note (required when resolving; shown in the history)"), 0, 4);
        panel.SetColumnSpan(panel.GetControlFromPosition(0, 4)!, 2);
        panel.Controls.Add(_note, 0, 5);
        panel.Controls.Add(_addNote, 1, 5);

        _respond.Controls.Add(panel);
    }

    // ---- loading ----

    private async Task ReloadAsync()
    {
        SetBusy(true);
        try
        {
            var result = await _requests.GetRequestDetailAsync(_requestId);
            if (result.IsFailure)
            {
                ShowMessage(result.Error.Message, isError: true);
                _respond.Visible = false;
                return;
            }

            if (_respond.Visible && _names.Count == 0) await LoadStaffAsync();

            _detail = result.Value;
            Render(_detail);
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

    private async Task LoadStaffAsync()
    {
        var staff = await _users.ListStaffAsync();
        if (staff.IsFailure) return;
        foreach (var s in staff.Value) _names[s.Id] = s.Name;
        _staff.DataSource = staff.Value.ToList();
        _staff.SelectedIndex = -1;
    }

    private void Render(RequestDetailDto d)
    {
        var r = d.Request;
        var contact = d.RequesterName is null ? $"Requester #{r.RequesterId}" : $"{d.RequesterName} <{d.RequesterEmail}>";
        _summary.Text =
            $"Reference #{r.Id}    Status: {r.Status}\n" +
            $"Category: {r.Category}    Location: {r.Location}\n" +
            $"Submitted: {Ui.Local(r.CreatedAt)}    Requester: {contact}\n" +
            $"Assigned to: {(r.AssignedStaffId is { } a ? Name(a) : "nobody yet")}";

        _description.Text = r.Description;

        _history.Items.Clear();
        foreach (var h in d.History)
            _history.Items.Add($"{Ui.Local(h.Timestamp)}  {h.OldStatus ?? "-"} > {h.NewStatus}  by {Name(h.ChangedBy)}");

        _notes.Items.Clear();
        foreach (var n in d.Notes)
            _notes.Items.Add($"{Ui.Local(n.Timestamp)}  {Name(n.AuthorId)}: {n.Note.ReplaceLineEndings(" ")}");

        // Offer only what the state machine allows; Assigned has its own control.
        var status = Enum.Parse<RequestStatus>(r.Status);
        var next = RequestStates.For(status).AllowedNext.Where(s => s != RequestStatus.Assigned).Select(s => s.ToString()).ToList();
        _nextStatus.DataSource = next;
        _assign.Enabled = status == RequestStatus.Submitted;
        _staff.Enabled = status == RequestStatus.Submitted;
        _applyStatus.Enabled = next.Count > 0;
        _nextStatus.Enabled = next.Count > 0;
        _addNote.Enabled = status != RequestStatus.Closed;
    }

    private string Name(long userId) => _names.TryGetValue(userId, out var n) ? n : $"user #{userId}";

    // ---- actions ----

    private Task AssignAsync() => RunAsync(() =>
    {
        var staffId = _staff.SelectedValue is long id ? id : 0;
        return _requests.AssignRequestAsync(new AssignRequestDto(_requestId, staffId, _note.Text));
    }, "Request assigned.");

    private Task ApplyStatusAsync() => RunAsync(() =>
    {
        var status = _nextStatus.SelectedItem is string s ? Enum.Parse<RequestStatus>(s) : RequestStatus.Submitted;
        return _requests.UpdateRequestStatusAsync(new UpdateRequestStatusDto(_requestId, status, _note.Text));
    }, "Status updated.");

    private Task AddNoteAsync() => RunAsync(
        () => _requests.AddNoteAsync(new AddNoteDto(_requestId, _note.Text)), "Note added.");

    private async Task RunAsync(Func<Task<Result>> action, string successMessage)
    {
        if (_busy) return;
        SetBusy(true);
        ShowMessage(null, isError: false);
        try
        {
            var result = await action();
            if (result.IsFailure)
            {
                ShowMessage(result.Error.Message, isError: true);   // includes "changed by someone else" conflicts
                if (result.Error.Type == ErrorType.Conflict) { SetBusy(false); await ReloadAsync(); ShowMessage(result.Error.Message, true); }
                return;
            }

            _note.Clear();
            SetBusy(false);
            await ReloadAsync();
            ShowMessage(successMessage, isError: false);
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
        _respond.Enabled = !busy;
    }

    private void ShowMessage(string? text, bool isError)
    {
        _message.ForeColor = isError ? Color.Firebrick : Color.SeaGreen;
        _message.Text = text ?? string.Empty;
    }
}
