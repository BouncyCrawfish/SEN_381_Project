using System;
namespace CivicConnect.Requests.Domain;

public class  RequestHistory : Requests
{
    public int HistoryId { get; private set; };
    public string Notes { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    protected RequestHistory() { } : base()
    // For EF Core
    public RequestHistory(int requestId, string category, string location, string description, int requestStatus, string notes)
        : base(category, location, description, requestStatus, requestId)
    {
        requestId = requestId;
        requestStatus = status;
        Notes = notes;
        Timestamp = DateTime.UtcNow;
    }
}

