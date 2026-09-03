using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public interface IEnquiryRepository :IGenericRepository<Enquiry>
    {
        Task<PagedResponse<Enquiry>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);

    }
}
