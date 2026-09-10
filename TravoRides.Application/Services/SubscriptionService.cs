using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Review;
using TravoRides.Application.DTOs.Subscription;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper,IEmailService email)
        {
            _unitOfWork = unitOfWork;
            _emailService = email;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SubscriptionDTO>> GetAllAsync(TravoRides.Application.DTOs.Subscription.SearchSubscriptionRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var all = await _unitOfWork.Subscriptions.GetAllAsync(cancellationToken);

            var query = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var k = request.Keyword.Trim().ToLower();
                query = query.Where(s => s.Email != null && s.Email.ToLower().Contains(k));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new PagedResponse<SubscriptionDTO>
            {
                Items = _mapper.Map<List<SubscriptionDTO>>(items),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
        public async Task<SubscriptionDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var subscription = await _unitOfWork.Subscriptions
                .GetByIdAsync(id, cancellationToken);

            if (subscription == null)
                return null;

            return _mapper.Map<SubscriptionDTO>(subscription);
        }

        public async Task<Guid> CreateAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException("Subscription is required.");

            var existingsubscription = await _unitOfWork.Subscriptions
                .FindAsync(x => x.Email == request.Email.Trim(), cancellationToken);

            if (existingsubscription.Any())
                throw new ValidationException("Subscription with the same Email already exists.");

            var subscribe = new Subscription
            {
                Email = request.Email.Trim()
            };

            await _unitOfWork.Subscriptions.AddAsync(subscribe, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);



            return subscribe.Id;
        }
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var subscribe = await _unitOfWork.Subscriptions
                .GetByIdAsync(id, cancellationToken);

            if (subscribe == null)
                throw new ResourceNotFoundException("Enquiry not found.");

            subscribe.IsDeleted = true;
            subscribe.ModifiedAt = DateTime.UtcNow;
            subscribe.ModifiedBy = "System"; // You

            _unitOfWork.Subscriptions.Update(subscribe);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    } 
   

}
