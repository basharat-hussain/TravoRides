using TravoRides.Domain.Entities;
using TravoRides.Infrastructure.Context;

namespace TravoRides.Infrastructure.Repository
{
    public class QuoteRepository : GenericRepository<Quote>, Application.Repositories.IQuoteRepository
    {
        private readonly ApplicationDbContext _context;
        public QuoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
