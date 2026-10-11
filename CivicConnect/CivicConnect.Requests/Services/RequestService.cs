using System;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Services
{
    internal class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
        }

        public async Task<Result<int>> SubmitRequestAsync(SubmitRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Category) || string.IsNullOrWhiteSpace(dto.Description))
            {
                return Result<int>.Failure(new Error("Request.Validate", "Category and Description are required.", ErrorType.Validation));
            }

            var request = new Request(dto.Category, dto.Location, dto.Description, dto.RequestId);

            await _requestRepository.AddAsync(request);
            return Result<int>.Success(request.Id);
        }

        public async Task<Result<IEnumerable<RequestDto>>> GetStaffQueueAsync()
        {
            var requests = await _requestRepository.GetAllActiveAsync();
            var dtos = requests.Select(r => new RequestDto(
                r.Id,
                r.Category,
                r.Location,
                r.Description,
                r.Status.ToString(),
                r.RequesterId,
                r.AssignedStaffId,
                r.CreatedAt
            ));
            return Result<IEnumerable<RequestDto>>.Success(dtos);
        }

        public async Task<Result> UpdateRequestStatusAsync(int requestId, RequestStatus newStatus, int staffId, string notes)
        {
           var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return Result.Failure(new Error("Request.NotFound", "Request not found.", ErrorType.NotFound));
            }
           
            var transitionResults = request.TransitionTo(newStatus, staffId, notes);
            if(transitionResults.IsFailure)
            {
                return Result.Failure(transitionResults.Error);
            }

            await _requestRepository.UpdateAsync(request);
            return Result.Success();
        }
    }
}
