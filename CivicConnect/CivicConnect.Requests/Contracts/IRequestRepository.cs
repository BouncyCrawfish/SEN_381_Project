using System;
using System.Collections.Generic;
using System.Text;
using CivicConnect.Requests.Domain;

namespace CivicConnect.Requests.Contracts
{
    internal interface IRequestRepository
    {
        Task<Request> GetRequestByIdAsync(int Id);
        Task<IEnumerable<Request>> GetAllActiveAsync();
        Task AddAsync(Request request);
        Task UpdateAsync(Request request);
    }
}
