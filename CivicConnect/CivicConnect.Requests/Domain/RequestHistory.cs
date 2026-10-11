using System;
namespace CivicConnect.Requests.Domain
{
    public class RequestHistory : Request
    {
        public int HistoryId { get; private set; }
        public string Notes { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public RequestHistory() : base() { }
        // For EF Core
        public RequestHistory(int requestId, string category, string location, string description,int requesterId, RequestStatus status, string notes) 
            : base(category, location, description, requesterId)
        {
            Id = requestId;
            Status = status;
            Notes = notes;
            Timestamp = DateTime.UtcNow;
        }
    }
}



