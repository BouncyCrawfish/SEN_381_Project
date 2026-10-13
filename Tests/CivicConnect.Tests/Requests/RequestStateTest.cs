using CivicConnect.Requests.Domain;

namespace CivicConnect.Tests.Requests;

/// <summary>
/// TC-03, TC-04, TC-05 (unit) and TC-08 (black-box state transition), requirement FR-011.
/// Expected results come from the acceptance criteria for FR-011 and ADR-004, not from the code:
///   Submitted > Assigned > InProgress > Resolved > Closed, nothing else, and Closed allows nothing.
/// </summary>
public class RequestStateTest
{
    private static readonly DateTime Now = new(2026, 10, 12, 8, 0, 0, DateTimeKind.Utc);

    private static Request NewRequest() =>
        Request.Submit(categoryId: 1, "123 Main St", "Large pothole in the middle of the road.", requesterId: 42, Now);

    /// <summary>Builds a request that is already in <paramref name="status"/> without walking the path.</summary>
    private static Request InStatus(RequestStatus status) =>
        Request.Restore(10, 1, "456 Elm St", "Streetlight is not working.", 43, status,
            status == RequestStatus.Submitted ? null : 1001, Now);

    [Fact]
    public void TC03_ValidStateTransitions_Succeed()
    {
        var request = NewRequest();
        Assert.Equal(RequestStatus.Submitted, request.Status);

        Assert.True(request.Assign(1001, 1001, "Taking this one.", Now).IsSuccess);
        Assert.Equal(RequestStatus.Assigned, request.Status);
        Assert.Equal(1001, request.AssignedStaffId);

        Assert.True(request.TransitionTo(RequestStatus.InProgress, 1001, "Starting work.", Now).IsSuccess);
        Assert.Equal(RequestStatus.InProgress, request.Status);

        Assert.True(request.TransitionTo(RequestStatus.Resolved, 1001, "Pothole filled.", Now).IsSuccess);
        Assert.Equal(RequestStatus.Resolved, request.Status);

        Assert.True(request.TransitionTo(RequestStatus.Closed, 1001, "Confirmed with requester.", Now).IsSuccess);
        Assert.Equal(RequestStatus.Closed, request.Status);
    }

    [Fact]
    public void TC03_EveryTransition_IsRecordedInHistory_WithActorAndTimestamp()
    {
        var request = NewRequest();
        request.Assign(1001, 1001, null, Now);
        request.TransitionTo(RequestStatus.InProgress, 1001, null, Now.AddHours(1));

        // Initial entry + two transitions (NFR-005, FR-011).
        Assert.Equal(3, request.History.Count);
        var last = request.History[^1];
        Assert.Equal(RequestStatus.Assigned, last.OldStatus);
        Assert.Equal(RequestStatus.InProgress, last.NewStatus);
        Assert.Equal(1001, last.ChangedBy);
        Assert.Equal(Now.AddHours(1), last.Timestamp);
        Assert.Null(request.History[0].OldStatus);   // initial entry has no old status
    }

    [Fact]
    public void TC04_InvalidJump_SubmittedToClosed_IsRejected()
    {
        var request = NewRequest();

        var result = request.TransitionTo(RequestStatus.Closed, 1002, "Trying to close directly.", Now);

        Assert.True(result.IsFailure);
        Assert.Equal(RequestStatus.Submitted, request.Status);                       // unchanged
        Assert.Equal("Invalid state transition from Submitted to Closed.", result.Error.Message);
        Assert.Single(request.History);                                              // nothing was recorded
        Assert.Empty(request.Notes);
    }

    [Fact]
    public void TC04_Assigning_WhenNotSubmitted_IsRejected_AndKeepsOriginalAssignee()
    {
        var request = InStatus(RequestStatus.InProgress);

        var result = request.Assign(2002, 2002, null, Now);

        Assert.True(result.IsFailure);
        Assert.Equal(1001, request.AssignedStaffId);   // not overwritten
    }

    // TC-05: remaining paths. Each row is (current, target) that must be blocked.
    [Theory]
    [InlineData(RequestStatus.Closed, RequestStatus.Assigned)]
    [InlineData(RequestStatus.Closed, RequestStatus.Submitted)]
    [InlineData(RequestStatus.Resolved, RequestStatus.Submitted)]
    [InlineData(RequestStatus.Resolved, RequestStatus.InProgress)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Closed)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Assigned)]
    [InlineData(RequestStatus.Assigned, RequestStatus.Resolved)]
    [InlineData(RequestStatus.Submitted, RequestStatus.InProgress)]
    [InlineData(RequestStatus.Submitted, RequestStatus.Submitted)]
    public void TC05_BlockedTransitions_AreRejected(RequestStatus current, RequestStatus target)
    {
        var request = InStatus(current);

        Assert.False(request.CanTransitionTo(target), $"{current} to {target} should be blocked.");
        var result = request.TransitionTo(target, 1001, "note", Now);
        Assert.True(result.IsFailure);
        Assert.Equal(current, request.Status);
    }

    [Theory]
    [InlineData(RequestStatus.Assigned, RequestStatus.InProgress)]
    [InlineData(RequestStatus.InProgress, RequestStatus.Resolved)]
    [InlineData(RequestStatus.Resolved, RequestStatus.Closed)]
    public void TC05_AllowedTransitions_FromEachState_Succeed(RequestStatus current, RequestStatus target)
    {
        var request = InStatus(current);

        Assert.True(request.CanTransitionTo(target));
        Assert.True(request.TransitionTo(target, 1001, null, Now).IsSuccess);
        Assert.Equal(target, request.Status);
    }

    [Fact]
    public void TC05_Closed_AllowsNothing()
    {
        var request = InStatus(RequestStatus.Closed);

        foreach (var target in Enum.GetValues<RequestStatus>())
            Assert.False(request.CanTransitionTo(target), $"Closed must not allow {target}.");
        Assert.Empty(request.State.AllowedNext);
    }

    [Fact]
    public void TC05_TransitionToAssigned_WithoutAssignee_IsRefused()
    {
        // Submitted > Assigned is legal, but only through Assign() because an assignee is mandatory (FR-010).
        var request = NewRequest();

        var result = request.TransitionTo(RequestStatus.Assigned, 1001, null, Now);

        Assert.True(result.IsFailure);
        Assert.Equal(RequestStatus.Submitted, request.Status);
        Assert.Null(request.AssignedStaffId);
    }

    [Fact]
    public void LaterTransitions_DoNotChangeTheAssignee()
    {
        var request = InStatus(RequestStatus.InProgress);   // assigned to 1001

        request.TransitionTo(RequestStatus.Resolved, 9999, "Done.", Now);

        Assert.Equal(1001, request.AssignedStaffId);
    }

    [Fact]
    public void ClosedRequest_CannotReceiveNotes()
    {
        var request = InStatus(RequestStatus.Closed);

        Assert.True(request.AddNote(1001, "late note", Now).IsFailure);
        Assert.Empty(request.Notes);
    }

    /// <summary>TC-08 (black-box, state transition): the full 5x5 table of what is allowed from where.</summary>
    [Fact]
    public void TC08_StateTransitionTable_MatchesTheAcceptanceCriteria()
    {
        var allowed = new Dictionary<RequestStatus, RequestStatus[]>
        {
            [RequestStatus.Submitted] = [RequestStatus.Assigned],
            [RequestStatus.Assigned] = [RequestStatus.InProgress],
            [RequestStatus.InProgress] = [RequestStatus.Resolved],
            [RequestStatus.Resolved] = [RequestStatus.Closed],
            [RequestStatus.Closed] = []
        };

        foreach (var from in Enum.GetValues<RequestStatus>())
            foreach (var to in Enum.GetValues<RequestStatus>())
            {
                var expected = allowed[from].Contains(to);
                Assert.True(expected == InStatus(from).CanTransitionTo(to),
                    $"{from} to {to}: expected {(expected ? "allowed" : "blocked")}.");
            }
    }
}
