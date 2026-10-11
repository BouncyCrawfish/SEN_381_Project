using System;
using CivicConnect.Common;
using CivicConnect.Requests.Contracts;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public RequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
        }

        public async Task<Result<int>> SubmitRequestAsync(SubmitRequestDto dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Category) || string.IsNullOrWhiteSpaces(dto.Description))
            {
                return Result<int>.Failure("Category and Description are required.");
            }

            var request = new Request
            {
                Category = dto.Category,
                Location = dto.Location,
                Description = dto.Description,
                RequesterId = dto.RequesterId,
                CreatedAt = DateTime.UtcNow
            };

            await _requestRepository.AddAsync(request);
            return Result<int>.Success(request.Id);
        }

        public async Task<Result<IEnumerable<RequestDto>>> GetStaffQueueAsync()
        {
            var requests = await _requestRepository.GetAllActiveAsync();
            var dtos = requests.Select(r => new RequestDto
            {
                Id = r.Id,
                Category = r.Category,
                Location = r.Location,
                Description = r.Description,
                RequesterId = r.RequesterId,
                CreatedAt = r.CreatedAt
            });
            return Result<IEnumerable<RequestDto>>.Success(dtos);
        }

        public async Task<Result> UpdateRequestStatusAsync(int requestId, RequestStatus newStatus, int staffId, string notes)
        {
           var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
            {
                return Result.Failure("Request not found.");
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
