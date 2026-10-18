using CivicConnect.Auth;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.App.Forms;

/// One request in full (FR-009). Requesters get a read-only view of their own request, managers a
/// read-only view of any request, and staff also get the response panel: assign (FR-010), move to the next
/// status (FR-011), add a note (FR-012) and resolve/close (FR-013). Security notes:
///  - the response panel is only SHOWN to staff, but the service re-checks the permission, the
///    assignee and the state transition on every call, so a hidden panel is not the protection;
///  - the next-status list offers only what the state machine allows, but the domain re-validates;
///  - the requester's contact details arrive only when the service decides the viewer may see them;
///  - controls are locked while a call is in flight (no double submit), and exceptions are never shown.
internal sealed partial class RequestDetailForm : Form
{
    private readonly IRequestService _requests;
    private readonly IUserService _users;
    private readonly long _requestId;
    private readonly bool _canRespond;
    private readonly bool _canSeeStaffSide;

    private readonly Dictionary<long, string> _names = new();
    private bool _busy;

    public RequestDetailForm(IRequestService requests, IUserService users, IAccessControl access, long requestId)
    {
        _requests = requests;
        _users = users;
        _requestId = requestId;
        _canRespond = access.HasPermission(Permission.UpdateRequestStatus) || access.HasPermission(Permission.AssignRequest);
        _canSeeStaffSide = access.HasPermission(Permission.ViewRequestHistory);   // Staff and Manager

        InitializeComponent();
        Theme.Apply(this);
        StyleBadge();

        lblTitle.Text = $"Request #{requestId}";
        txtNote.MaxLength = RequestRules.MaxNoteLength;
        txtResolutionNote.MaxLength = RequestRules.MaxNoteLength;

        // Requesters never see the staff-only parts (the service also withholds the data).
        lblNotesHeading.Visible = _canSeeStaffSide;
        lstNotes.Visible = _canSeeStaffSide;

        // Managers read only: with no response panel the detail card takes the whole width.
        pnlRespondCard.Visible = _canRespond;
        tlpMain.ColumnStyles[1].SizeType = SizeType.Absolute;
        tlpMain.ColumnStyles[1].Width = _canRespond ? 440F : 0F;
        pnlDetailCard.Margin = _canRespond ? new Padding(0, 0, 16, 0) : new Padding(0);

        FormClosing += (_, e) => { if (_busy) e.Cancel = true; };
        Shown += async (_, _) => await ReloadAsync();
    }

    private void StyleBadge()
    {
        lblStatusBadge.Font = Theme.Bold;
        lblStatusBadge.ForeColor = Color.White;
        lblStatusBadge.BackColor = Theme.Muted;
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
                Theme.ShowMessage(lblMessage, result.Error.Message, isError: true);
                pnlRespondCard.Visible = false;
                return;
            }

            if (_canSeeStaffSide && _names.Count == 0) await LoadStaffAsync();
            Render(result.Value);
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

    private async Task LoadStaffAsync()
    {
        var staff = await _users.ListStaffAsync();
        if (staff.IsFailure) return;
        foreach (var s in staff.Value) _names[s.Id] = s.Name;
        cboStaff.DataSource = staff.Value.ToList();
        cboStaff.DisplayMember = nameof(UserDto.Name);
        cboStaff.ValueMember = nameof(UserDto.Id);
        cboStaff.SelectedIndex = -1;
    }

    private void Render(RequestDetailDto d)
    {
        var r = d.Request;
        if (d.RequesterName is not null) _names[r.RequesterId] = d.RequesterName;   // so history shows a name, not "user #4"
        var contact = d.RequesterName is null ? $"Requester #{r.RequesterId}" : $"{d.RequesterName} <{d.RequesterEmail}>";

        lblSubtitle.Text = $"{r.Category}  |  {r.Location}";
        lblStatusBadge.Text = r.Status;
        lblStatusBadge.BackColor = Theme.StatusColor(r.Status);
        lblSummary.Text =
            $"Submitted:   {Ui.Local(r.CreatedAt)}\n" +
            $"Requester:   {contact}\n" +
            $"Assigned to: {(r.AssignedStaffId is { } a ? DisplayName(a) : "nobody yet")}";

        txtDescription.Text = r.Description;

        lstHistory.Items.Clear();
        foreach (var h in d.History)
            lstHistory.Items.Add($"{Ui.Local(h.Timestamp)}   {h.OldStatus ?? "-"}  >  {h.NewStatus}   by {DisplayName(h.ChangedBy)}");

        lstNotes.Items.Clear();
        foreach (var n in d.Notes)
            lstNotes.Items.Add($"{Ui.Local(n.Timestamp)}   {DisplayName(n.AuthorId)}: {n.Note.ReplaceLineEndings(" ")}");

        // Offer only what the state machine allows; Assigned has its own control.
        var status = Enum.Parse<RequestStatus>(r.Status);
        var next = RequestStates.For(status).AllowedNext.Where(s => s != RequestStatus.Assigned).Select(s => s.ToString()).ToList();
        cboNextStatus.DataSource = next;
        UpdateResolutionNoteVisibility();
        var submitted = status == RequestStatus.Submitted;
        cboStaff.Enabled = submitted;
        btnAssign.Enabled = submitted;
        cboNextStatus.Enabled = next.Count > 0;
        btnApplyStatus.Enabled = next.Count > 0;
        btnAddNote.Enabled = status != RequestStatus.Closed;
    }

    private string DisplayName(long userId) => _names.TryGetValue(userId, out var n) ? n : $"user #{userId}";

    // ---- actions ----

    private async void btnAssign_Click(object? sender, EventArgs e) => await RunAsync(() =>
    {
        var staffId = cboStaff.SelectedValue is long id ? id : 0;
        return _requests.AssignRequestAsync(new AssignRequestDto(_requestId, staffId, txtNote.Text));
    }, "Request assigned.");

    private async void btnApplyStatus_Click(object? sender, EventArgs e)
    {
        var status = cboNextStatus.SelectedItem is string s ? Enum.Parse<RequestStatus>(s) : RequestStatus.Submitted;

        // Resolving has its own required note, so the requester always learns what was done.
        // The service enforces this too; this check just gives a friendlier, earlier message.
        if (status == RequestStatus.Resolved && string.IsNullOrWhiteSpace(txtResolutionNote.Text))
        {
            Theme.ShowMessage(lblMessage, "Enter a resolution note first: describe what was done to fix the problem.", isError: true);
            txtResolutionNote.Focus();
            return;
        }

        var note = status == RequestStatus.Resolved ? txtResolutionNote.Text : txtNote.Text;
        await RunAsync(
            () => _requests.UpdateRequestStatusAsync(new UpdateRequestStatusDto(_requestId, status, note)),
            status == RequestStatus.Resolved ? "Request resolved." : "Status updated.");
    }

    private void cboNextStatus_SelectedIndexChanged(object? sender, EventArgs e) => UpdateResolutionNoteVisibility();

    /// <summary>The resolution note box only appears when the chosen next status is Resolved.</summary>
    private void UpdateResolutionNoteVisibility()
    {
        var resolving = cboNextStatus.SelectedItem is string s && s == nameof(RequestStatus.Resolved);
        lblResolutionNote.Visible = resolving;
        txtResolutionNote.Visible = resolving;
        btnApplyStatus.Text = resolving ? "&Resolve" : "Apply &status";
    }

    private async void btnAddNote_Click(object? sender, EventArgs e) => await RunAsync(
        () => _requests.AddNoteAsync(new AddNoteDto(_requestId, txtNote.Text)), "Note added.");

    private async Task RunAsync(Func<Task<Result>> action, string successMessage)
    {
        if (_busy) return;
        SetBusy(true);
        Theme.ShowMessage(lblMessage, null, isError: false);
        try
        {
            var result = await action();
            if (result.IsFailure)
            {
                SetBusy(false);
                if (result.Error.Type == ErrorType.Conflict) await ReloadAsync();   // show what the other person changed
                Theme.ShowMessage(lblMessage, result.Error.Message, isError: true);
                return;
            }

            txtNote.Clear();
            txtResolutionNote.Clear();
            SetBusy(false);
            await ReloadAsync();
            Theme.ShowMessage(lblMessage, successMessage, isError: false);
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
        pnlRespondCard.Enabled = !busy;
    }
}