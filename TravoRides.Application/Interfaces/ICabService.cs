using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface ICabService
    {
        Task<PagedResponse<CabDTO>> GetAllAsync(SearchCabRequest request, CancellationToken cancellationToken = default);

        Task<CabDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateCabRequest request, CancellationToken cancellationToken = default);

        Task UpdateAsync(UpdateCabRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
