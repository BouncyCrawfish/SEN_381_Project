using System;
namespace CivicConnect.Requests.Domain
{
    /// <summary>
    /// Represents a request in the CivicConnect system.
    /// </summary>

    public enum RequestStatus
    {
        Submitted,
        Assigned,
        InProgress,
        Resolved,
        Closed
    }

    public class Request
    {
        public int Id { get; protected set; };
        public string Category { get; protected set; };
        public string Location { get; protected set; };
        public string Description { get; protected set; };
        public int RequesterId { get; protected set; };
        public int? AssignedStaffId { get; protected set; };
        public RequestStatus Status { get; protected set; } = RequestStatus.Submitted;
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        private readonly List<RequestHistory> _history = new();
        public IReadOnlyCollection<RequestHistory> History => _history.AsReadOnly();

        protected Request() { } // For EF Core

        public Request(string category, string location, string description, int requesterId)
        {
            Category = category;
            Location = location;
            Description = description;
            RequesterId = requesterId;
            Status = RequestStatus.Submitted;
        }

        public virtual bool CanTransitionTo(RequestStatus newStatus)
        {
            return (Status, newStatus) switch
            {
                (RequestStatus.Submitted, RequestStatus.Assigned) => true,
                (RequestStatus.Assigned, RequestStatus.InProgress) => true,
                (RequestStatus.InProgress, RequestStatus.Resolved) => true,
                (RequestStatus.Resolved, RequestStatus.Closed) => true,
                _ => false //prevents invalid jumps like Submitted to Resolved or InProgress to Closed
            };
        }

        public virtual CivicConnect.Common.Result TransitionTo(RequestStatus nextStatus, int staffId, string notes)
        {
            if (!CanTransitionTo(newStatus))
            {
                return CivicConnect.Common.Result.Failure($"Invalid status transition from {Status} to {nextStatus}.");
            }

            Status = nextStatus;
            AssignedStaffId = staffId;

            //Add a history entry
            _history.Add(new RequestHistory
            {
                RequestId = Id,
                PreviousStatus = Status,
                NewStatus = nextStatus,
                ChangedByStaffId = staffId,
                Notes = notes,
                ChangedAt = DateTime.UtcNow
            });

            return CivicConnect.Common.Result.Success();
        }
    }
}

