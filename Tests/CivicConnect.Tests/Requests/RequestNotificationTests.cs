using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Tests.Requests;

public class RequestNotificationTests
{
    [Fact]
    public async Task Assigning_a_request_publishes_one_status_changed_event()
    {
        var h = new RequestHarness();
        var request = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.AssignRequestAsync(new AssignRequestDto(request.Id, RequestHarness.StaffId, null));

        Assert.True(result.IsSuccess);
        var e = Assert.IsType<RequestStatusChangedEvent>(Assert.Single(h.Events.Published));
        Assert.Equal(request.Id, e.RequestId);
        Assert.Equal("Submitted", e.OldStatus);
        Assert.Equal("Assigned", e.NewStatus);
        Assert.Equal(RequestHarness.StaffId, e.ChangedBy);
    }

    [Fact]
    public async Task Updating_the_status_publishes_the_old_and_new_status()
    {
        var h = new RequestHarness();
        var request = h.Seed(RequestStatus.Assigned);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.UpdateRequestStatusAsync(
            new UpdateRequestStatusDto(request.Id, RequestStatus.InProgress, null));

        Assert.True(result.IsSuccess);
        var e = Assert.IsType<RequestStatusChangedEvent>(Assert.Single(h.Events.Published));
        Assert.Equal("Assigned", e.OldStatus);
        Assert.Equal("InProgress", e.NewStatus);
    }

    [Fact]
    public async Task Adding_a_note_publishes_nothing()
    {
        var h = new RequestHarness();
        var request = h.Seed(RequestStatus.Assigned);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.AddNoteAsync(new AddNoteDto(request.Id, "Called the requester."));

        Assert.True(result.IsSuccess);
        Assert.Empty(h.Events.Published);
    }

    [Fact]
    public async Task A_rejected_change_publishes_nothing()
    {
        var h = new RequestHarness();
        var request = h.Seed(RequestStatus.Assigned);
        h.SignInAs(RequestHarness.OtherStaffId);   // not the assignee, so the change is refused

        var result = await h.Service.UpdateRequestStatusAsync(
            new UpdateRequestStatusDto(request.Id, RequestStatus.InProgress, null));

        Assert.True(result.IsFailure);
        Assert.Empty(h.Events.Published);
    }

    [Fact]
    public async Task A_failing_listener_does_not_fail_the_saved_change()
    {
        var h = new RequestHarness();
        h.Events.Throw = true;
        var request = h.Seed(RequestStatus.Submitted);
        h.SignInAs(RequestHarness.StaffId);

        var result = await h.Service.AssignRequestAsync(new AssignRequestDto(request.Id, RequestHarness.StaffId, null));

        Assert.True(result.IsSuccess);
        Assert.Equal(RequestStatus.Assigned, h.Repo.Requests[request.Id].Status);
    }
}