using System.Threading;
using System.Threading.Tasks;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Repositories
{
    public interface IEnquiryRepository : IGenericRepository<Enquiry>
    {
        Task<PagedResponse<Enquiry>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken);
    }
}

