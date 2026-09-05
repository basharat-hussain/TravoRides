using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.DTOs.BookingReport;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<PagedResponse<Booking>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
        Task<BookingReportResponse> GetBookingReportAsync( int pageNumber, int pageSize, string? keyword, DateTime? fromDate,
                  DateTime? toDate, CancellationToken cancellationToken = default);
    }
}
