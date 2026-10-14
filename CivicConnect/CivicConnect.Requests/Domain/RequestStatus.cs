namespace CivicConnect.Requests.Domain;


/// Lifecycle of a request. Names MUST match the request_status_valid CHECK constraint in
/// db/03_schema.sql (stored as strings with HasConversion&lt;string&gt;()).

public enum RequestStatus
{
    Submitted,
    Assigned,
    InProgress,
    Resolved,
    Closed
}
