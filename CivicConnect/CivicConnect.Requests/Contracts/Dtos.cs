using System;
namespace CivicConnect.Requests.Contracts;

public record SubmitRequestDto(string Category, string Location, string Description, int RequestId);
public record RequestDto(int Id, string Category, string Location, string Description, string Status,int RequesterId, int? AssignedStaffId, DateTime CreatedAt);
