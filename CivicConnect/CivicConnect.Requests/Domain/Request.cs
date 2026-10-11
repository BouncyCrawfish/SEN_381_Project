using System;
using System.Collections.Generic;
using CivicConnect.Requests.Domain;

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
        public int Id { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int RequesterId { get; set; }
        public int? AssignedStaffId { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Submitted;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        private readonly List<RequestHistory> _history = new();
        public IReadOnlyCollection<RequestHistory> History => _history.AsReadOnly();

        public Request() { } // For EF Core

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
            if (!CanTransitionTo(nextStatus))
            {
                return CivicConnect.Common.Result.Failure(
                    new CivicConnect.Common.Error(
                        "InvalidRequestStatusTransition",
                        "Invalid request status transition.",
                        CivicConnect.Common.ErrorType.Validation));
            }

            Status = nextStatus;
            AssignedStaffId = staffId;

            //Add a history entry
            _history.Add(new RequestHistory(Id, Category, Location, Description, RequesterId, Status, notes));

            return CivicConnect.Common.Result.Success();
        }
    }
}

