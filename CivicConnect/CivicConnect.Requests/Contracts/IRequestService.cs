using System;
using System.Collections.Generic;
using System.Text;
using CivicConnect.Requests.Domain;
using CivicConnect.Common;

namespace CivicConnect.Requests.Contracts
{
    internal interface IRequestService
    {
        Task<Result<int>> SubmitRequestAsync(SubmitRequestDto dto);
        Task<Result<IEnumerable<RequestDto>>> GetStaffQueueAsync();
        Task<Result> UpdateRequestStatusAsync(int requestId, RequestStatus newStatus, int staffId, string notes);
    }
}
