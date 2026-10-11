using System;
namespace CivicConnect.Requests.Contracts;

public record SubmitRequestDto(string Category, string Location, string Description, int RequestId);
public record RequestDto(int HistoryId, string Category, string Location, string Description, string RequestStatus,int RequestId, int? AssignedStaffId, DateTime CreatedAt );
