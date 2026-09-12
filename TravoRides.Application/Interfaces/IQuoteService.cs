using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Quote;

namespace TravoRides.Application.Interfaces
{
    public interface IQuoteService
    {
        Task<PagedResponse<QuoteDTO>> GetAllAsync(SearchQuoteRequest request, CancellationToken cancellationToken = default);

        Task<QuoteDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default);

        Task UpdateAsync(UpdateQuoteRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
