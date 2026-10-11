using System;
using CivicConnect.Requests.Domain;
using CivicConnect.Common;

namespace CivicConnect.Requests.Contracts
{
    public interface IRequestService
    {
        Task<Result<int>> SubmitRequestAsync(SubmitRequestDto dto);
        Task<Result<IEnumerable<RequestDto>>> GetStaffQueueAsync();
        Task<Result> UpdateStatusAsync(int requestId, RequestStatus status, int staffId, string notes);
    }

    public interface IRequestRepository
    {
        Task<Requests?> GetRequestByIdAsync(int requestId);
        Task AddAsync(Requests request);
        Task UpdateAsync(Requests request);
        Task<IEnumerable<Requests>> GetAllActiveAsync();
    }

}

