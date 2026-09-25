using AutoMapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Quote;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;


namespace TravoRides.Application.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly EmailSettings _emailSettings;

        public QuoteService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IEmailService emailService, 
            IEmailTemplateService emailTemplate,
            IOptions<EmailSettings> emailSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _emailTemplateService = emailTemplate;
            _emailSettings = emailSettings.Value;
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
                query = query.Where(q => (q.SpecialRequirements != null && q.SpecialRequirements.ToLower().Contains(k)) || (q.Name != null && q.Name.ToLower().Contains(k)) || (q.Email != null && q.Email.ToLower().Contains(k)));
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
            if (string.IsNullOrWhiteSpace(request.Requirements))
                throw new ValidationException("Requirement is required");

            var quote = new Quote
            {
                Name = request.Name?.Trim(),
                Phone = request.Phone?.Trim(),
                Email = request.Email?.Trim(),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Passengers = request.Passengers,
                SpecialRequirements = request.Requirements?.Trim()
            };

            await _unitOfWork.Quotes.AddAsync(quote, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send a confirmation email to the Customer using template
            var subject = "Thank you for your Quote";

            var body = await _emailTemplateService.GetQuoteConfirmationTemplateAsync(
                request.Name,
               
                request.Passengers,
                request.Phone,
                request.StartDate,
                request.EndDate,
                request.Requirements

                );

            var ownerEmail = _emailSettings.OwnerEmail?.Trim();

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var customerEmail = request.Email.Trim();
                var ccEmail = !string.IsNullOrWhiteSpace(ownerEmail) &&
                              !string.Equals(customerEmail, ownerEmail, StringComparison.OrdinalIgnoreCase)
                    ? ownerEmail
                    : null;

                await _emailService.SendEmailAsync(customerEmail, subject, body, true, ccEmail, cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(ownerEmail))
            {
                await _emailService.SendEmailAsync(ownerEmail, subject, body, true, null, cancellationToken);
            }
            return quote.Id;
        }

        public async Task UpdateAsync(UpdateQuoteRequest request, CancellationToken cancellationToken = default)
        {
            var quote = await _unitOfWork.Quotes.GetByIdAsync(request.Id, cancellationToken);
            if (quote == null) throw new ResourceNotFoundException("Quote not found.");

            quote.Name = request.Name?.Trim();
            quote.Phone = request.Phone?.Trim();
            quote.Email = request.Email?.Trim();
            quote.StartDate = request.StartDate;
            quote.EndDate = request.EndDate;
            quote.Passengers = request.Passengers;
            quote.SpecialRequirements = request.Requirements?.Trim();

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
