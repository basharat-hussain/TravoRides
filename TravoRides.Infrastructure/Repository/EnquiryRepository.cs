using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravoRides.Infrastructure.Context;
using TravoRides.Application.DTOs.Common;
using TravoRides.Domain.Entities;

namespace TravoRides.Infrastructure.Repository
{
    public class EnquiryRepository :GenericRepository<Enquiry>, IEnquiryRepository
    {
        private readonly ApplicationDbContext _context;

        public EnquiryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedResponse<Enquiry>> GetAllSearchAsync(int pageNumber, int pageSize, string? keyword, CancellationToken cancellationToken)
        {
            var query = _context.Enquiries
                .Where(e => !e.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var clean = keyword.Trim();
                query = query.Where(c => c.Name.Contains(clean) || (c.Email != null && c.Email.Contains(clean)));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResponse<Enquiry>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = totalPages
            };
        }
    }
}
