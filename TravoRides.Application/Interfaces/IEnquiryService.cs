
using TravoRiders.Application.DTOs.Enquirer;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Common;

namespace TravoRiders.Application.Interfaces
{
    public interface IEnquiryService
    {
        Task<PagedResponse<EnquiryDTO>> GetAllAsync(SearchEnquiryRequest request, CancellationToken cancellationToken = default);

        Task<EnquiryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateEnquiryRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
