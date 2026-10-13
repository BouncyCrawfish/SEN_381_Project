using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Tests.Requests;

/// <summary>FR-006, FR-007, FR-010 to FR-013, NFR-005, NFR-008 at the service level (role + ownership + workflow).</summary>
public class RequestSecurityTests
{
    // ---- authentication / role checks ----

    [Fact]
    public async Task NotSignedIn_EverythingIsRefused()
    {
        var h = new RequestHarness();
        var seeded = h.Seed(RequestStatus.Submitted);

        Assert.Equal(ErrorType.Unauthorized, (await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "x", "long enough text"))).Error.Type);
        Assert.True((await h.Service.GetMyRequestsAsync()).IsFailure);
        Assert.True((await h.Service.GetStaffQueueAsync()).IsFailure);
        Assert.True((await h.Service.GetRequestDetailAsync(seeded.Id)).IsFailure);
        Assert.True((await h.Service.AssignRequestAsync(new AssignRequestDto(seeded.Id, 10, null))).IsFailure);
    }

    [Fact]
    public async Task Requester_CannotUseStaffActions()
    {
        var h = new RequestHarness();
        var seeded = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.RequesterId);

        var queue = await h.Service.GetStaffQueueAsync();
        var assign = await h.Service.AssignRequestAsync(new AssignRequestDto(seeded.Id, RequestHarness.StaffId, null));
        var note = await h.Service.AddNoteAsync(new AddNoteDto(seeded.Id, "I am staff now"));

        Assert.Equal(ErrorType.Forbidden, queue.Error.Type);
        Assert.Equal(ErrorType.Forbidden, assign.Error.Type);
        Assert.Equal(ErrorType.Forbidden, note.Error.Type);
        Assert.Equal(RequestStatus.Submitted, h.Repo.Requests[seeded.Id].Status);
    }

    [Fact]
    public async Task Manager_CannotChangeRequests()
    {
        var h = new RequestHarness();
        var seeded = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.ManagerId);

        Assert.Equal(ErrorType.Forbidden, (await h.Service.AssignRequestAsync(new AssignRequestDto(seeded.Id, RequestHarness.StaffId, null))).Error.Type);
        Assert.Equal(ErrorType.Forbidden, (await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "x", "long enough text"))).Error.Type);
    }

    [Fact]
    public async Task Staff_CannotSubmitRequests()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "12 Long Street", "A perfectly valid description."));

        Assert.Equal(ErrorType.Forbidden, result.Error.Type);
    }

    // ---- ownership (NFR-008, FR-004) ----

    [Fact]
    public async Task SubmittedRequest_AlwaysBelongsToTheSignedInUser()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.RequesterId);

        var id = (await h.Service.SubmitRequestAsync(new SubmitRequestDto(1, "12 Long Street", "A perfectly valid description."))).Value;

        Assert.Equal(RequestHarness.RequesterId, h.Repo.Requests[id].RequesterId);
        Assert.Equal(RequestHarness.RequesterId, h.Repo.Requests[id].History[0].ChangedBy);
    }

    [Fact]
    public async Task Requester_SeesOnlyOwnRequests()
    {
        var h = new RequestHarness();
        var mine = h.Seed(RequestStatus.Submitted, RequestHarness.RequesterId);
        h.Seed(RequestStatus.Submitted, RequestHarness.OtherRequesterId);
        h.SignInAs(RequestHarness.RequesterId);

        var list = (await h.Service.GetMyRequestsAsync()).Value;

        Assert.Single(list);
        Assert.Equal(mine.Id, list[0].Id);
    }

    [Fact]
    public async Task Requester_CannotOpenSomeoneElsesRequest_AndGetsSameAnswerAsMissing()
    {
        var h = new RequestHarness();
        var theirs = h.Seed(RequestStatus.Submitted, RequestHarness.OtherRequesterId);
        h.SignInAs(RequestHarness.RequesterId);

        var other = await h.Service.GetRequestDetailAsync(theirs.Id);
        var missing = await h.Service.GetRequestDetailAsync(99999);

        Assert.True(other.IsFailure);
        Assert.Equal(missing.Error.Code, other.Error.Code);   // ids cannot be probed
    }

    [Fact]
    public async Task Requester_DetailView_HidesContactDetailsAndStaffNotes()
    {
        var h = new RequestHarness();
        var mine = h.Seed(RequestStatus.Assigned);
        mine.AddNote(RequestHarness.StaffId, "internal note", h.Clock.Now.UtcDateTime);
        h.SignInAs(RequestHarness.RequesterId);

        var detail = (await h.Service.GetRequestDetailAsync(mine.Id)).Value;

        Assert.Null(detail.RequesterEmail);
        Assert.Null(detail.RequesterName);
        Assert.Empty(detail.Notes);
    }

    [Fact]
    public async Task Staff_DetailView_ShowsContactDetailsHistoryAndNotes()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Assigned);
        req.AddNote(RequestHarness.StaffId, "called the requester", h.Clock.Now.UtcDateTime);
        h.SignInAs(RequestHarness.StaffId);

        var detail = (await h.Service.GetRequestDetailAsync(req.Id)).Value;

        Assert.Equal("rita@example.com", detail.RequesterEmail);
        Assert.Single(detail.Notes);
    }

    // ---- workflow: assign, progress, resolve, close (FR-010 to FR-013) ----

    [Fact]
    public async Task FullJourney_Submit_Assign_Progress_Resolve_Close_RecordsEveryStep()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.RequesterId);
        var id = (await h.Service.SubmitRequestAsync(new SubmitRequestDto(2, "Park Road", "Streetlight has been out for a week."))).Value;

        h.SignInAs(RequestHarness.StaffId);
        Assert.True((await h.Service.AssignRequestAsync(new AssignRequestDto(id, RequestHarness.StaffId, "Mine"))).IsSuccess);
        Assert.True((await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(id, RequestStatus.InProgress, null))).IsSuccess);
        Assert.True((await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(id, RequestStatus.Resolved, "Bulb replaced."))).IsSuccess);
        Assert.True((await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(id, RequestStatus.Closed, null))).IsSuccess);

        var saved = h.Repo.Requests[id];
        Assert.Equal(RequestStatus.Closed, saved.Status);
        Assert.Equal(5, saved.History.Count);                       // Submitted + 4 transitions
        Assert.All(saved.History.Skip(1), e => Assert.Equal(RequestHarness.StaffId, e.ChangedBy));
        Assert.Equal(2, saved.Notes.Count);

        h.SignInAs(RequestHarness.RequesterId);                     // FR-013: requester sees the new status
        Assert.Equal("Closed", (await h.Service.GetMyRequestsAsync()).Value.Single().Status);
    }

    [Fact]
    public async Task Assign_ToSomeoneWhoIsNotStaff_IsRejected()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        var toRequester = await h.Service.AssignRequestAsync(new AssignRequestDto(req.Id, RequestHarness.OtherRequesterId, null));
        var toManager = await h.Service.AssignRequestAsync(new AssignRequestDto(req.Id, RequestHarness.ManagerId, null));
        var toGhost = await h.Service.AssignRequestAsync(new AssignRequestDto(req.Id, 4242, null));

        Assert.True(toRequester.IsFailure && toManager.IsFailure && toGhost.IsFailure);
        Assert.Null(h.Repo.Requests[req.Id].AssignedStaffId);
    }

    [Fact]
    public async Task Staff_CanAssignToAnotherStaffMember()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        Assert.True((await h.Service.AssignRequestAsync(new AssignRequestDto(req.Id, RequestHarness.OtherStaffId, null))).IsSuccess);
        Assert.Equal(RequestHarness.OtherStaffId, h.Repo.Requests[req.Id].AssignedStaffId);
    }

    [Fact]
    public async Task Staff_CannotMoveSubmittedStraightToClosed()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(req.Id, RequestStatus.Closed, "skip"));

        Assert.True(result.IsFailure);
        Assert.Equal(RequestStatus.Submitted, h.Repo.Requests[req.Id].Status);
        Assert.Equal(0, h.Repo.UpdateCalls);   // nothing was saved
    }

    [Fact]
    public async Task OnlyTheAssignee_CanChangeStatus()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Assigned, assignee: RequestHarness.StaffId);
        h.SignInAs(RequestHarness.OtherStaffId);

        var result = await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(req.Id, RequestStatus.InProgress, null));

        Assert.Equal(ErrorType.Forbidden, result.Error.Type);
        Assert.Equal(RequestStatus.Assigned, h.Repo.Requests[req.Id].Status);
    }

    [Fact]
    public async Task Assigned_CannotBeSetThroughTheStatusUpdate()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(req.Id, RequestStatus.Assigned, null));

        Assert.True(result.IsFailure);
        Assert.Null(h.Repo.Requests[req.Id].AssignedStaffId);
    }

    [Fact]
    public async Task Resolving_WithoutANote_IsRejected()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.InProgress);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(req.Id, RequestStatus.Resolved, "  "));

        Assert.True(result.IsFailure);
        Assert.Equal(RequestStatus.InProgress, h.Repo.Requests[req.Id].Status);
    }

    [Fact]
    public async Task NoteTooLong_IsRejected()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Assigned);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.AddNoteAsync(new AddNoteDto(req.Id, new string('n', RequestRules.MaxNoteLength + 1)));

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UnknownRequest_ReturnsNotFound()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.AddNoteAsync(new AddNoteDto(31337, "hello"));

        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task LostUpdate_IsReportedAsConflict_NotAsACrash()
    {
        var h = new RequestHarness();
        var req = h.Seed(RequestStatus.Assigned);
        h.Repo.ThrowConcurrencyOnUpdate = true;
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.UpdateRequestStatusAsync(new UpdateRequestStatusDto(req.Id, RequestStatus.InProgress, null));

        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    // ---- staff queue (FR-007, FR-008) ----

    [Fact]
    public async Task StaffQueue_HidesClosed_AndFiltersByStatusAndSearch()
    {
        var h = new RequestHarness();
        h.Seed(RequestStatus.Submitted);
        h.Seed(RequestStatus.InProgress);
        h.Seed(RequestStatus.Closed);
        h.SignInAs(RequestHarness.StaffId);

        Assert.Equal(2, (await h.Service.GetStaffQueueAsync()).Value.Count);
        Assert.Single((await h.Service.GetStaffQueueAsync(new RequestQueueFilter(RequestStatus.Closed))).Value);
        Assert.Equal(2, (await h.Service.GetStaffQueueAsync(new RequestQueueFilter(SearchText: "pothole"))).Value.Count);
        Assert.Empty((await h.Service.GetStaffQueueAsync(new RequestQueueFilter(SearchText: "zebra"))).Value);
    }

    [Fact]
    public async Task StaffQueue_SearchTextTooLong_IsRejected()
    {
        var h = new RequestHarness();
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.GetStaffQueueAsync(new RequestQueueFilter(SearchText: new string('x', RequestRules.MaxSearchTextLength + 1)));

        Assert.True(result.IsFailure);
    }
}
