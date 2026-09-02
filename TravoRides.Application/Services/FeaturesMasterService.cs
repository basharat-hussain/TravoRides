using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class FeaturesMasterService : IFeaturesMasterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FeaturesMasterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<QuoteDTO>> GetAllAsync(SearchQuoteRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var all = await _unitOfWork.Quotes.GetAllAsync(cancellationToken);
            var query = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var k = request.Keyword.Trim().ToLower();
                query = query.Where(q => (q.Purpose != null && q.Purpose.ToLower().Contains(k)) || (q.Name != null && q.Name.ToLower().Contains(k)) || (q.Email != null && q.Email.ToLower().Contains(k)));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<QuoteDTO>
            {
                Items = _mapper.Map<List<QuoteDTO>>(items),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        public async Task<QuoteDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(id, cancellationToken);
            if (quote == null) return null;
            return _mapper.Map<QuoteDTO>(quote);
        }

        public async Task<Guid> CreateAsync(CreateQuoteRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Purpose))
                throw new ValidationException("Purpose is required");

            var quote = new Quote
            {
                Purpose = request.Purpose?.Trim(),
                Name = request.Name?.Trim(),
                Phone = request.Phone?.Trim(),
                Email = request.Email?.Trim(),
                Requirements = request.Requirements?.Trim()
            };

            await _unitOfWork.Quotes.AddAsync(quote, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return quote.Id;
        }

        public async Task UpdateAsync(UpdateQuoteRequest request, CancellationToken cancellationToken = default)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(request.Id, cancellationToken);
            if (quote == null) throw new ResourceNotFoundException("Quote not found.");

            quote.Purpose = request.Purpose?.Trim();
            quote.Name = request.Name?.Trim();
            quote.Phone = request.Phone?.Trim();
            quote.Email = request.Email?.Trim();
            quote.Requirements = request.Requirements?.Trim();

            _unitOfWork.Quotes.Update(quote);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(id, cancellationToken);
            if (quote == null) throw new ResourceNotFoundException("Quote not found.");

            quote.IsDeleted = true;
            quote.ModifiedAt = DateTime.UtcNow;
            quote.ModifiedBy = "System";

            _unitOfWork.Quotes.Update(quote);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
