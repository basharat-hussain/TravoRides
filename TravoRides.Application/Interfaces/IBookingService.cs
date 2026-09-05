using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.BookingReport;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Interfaces
{
    public interface IBookingService
    {
        Task<PagedResponse<BookingDTO>> GetAllAsync(SearchBookingRequest request, CancellationToken cancellationToken = default);
        Task<BookingReportResponse> GetBookingReportAsync(SearchBookingRequest request, CancellationToken cancellationToken);
        Task<BookingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Guid> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateBookingRequest  request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
