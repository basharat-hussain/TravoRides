using TravoRides.Application.Common.Exceptions;

using TravoRides.Application.DTOs.Review;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Repositories;
using TravoRides.Application.DTOs.Common;

namespace TravoRides.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileUrlService _fileUrlService;

        public ReviewService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            IFileUrlService fileUrlService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUrlService = fileUrlService;
        }

        public async Task<PagedResponse<ReviewDTO>> GetAllAsync(SearchReviewRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var all = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);

            var query = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var k = request.Keyword.Trim().ToLower();
                query = query.Where(r => (r.Name != null && r.Name.ToLower().Contains(k)) || (r.Address != null && r.Address.ToLower().Contains(k)));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var reviewDtos = _mapper.Map<List<ReviewDTO>>(items);
            reviewDtos = EnrichReviewDtosWithAbsoluteUrls(reviewDtos).ToList();

            return new PagedResponse<ReviewDTO>
            {
                Items = reviewDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / request.PageSize)
            };
        }

        // Method 2: For the Public Frontend (See Only Approved/Active)
        public async Task<PagedResponse<ReviewDTO>> GetAllApprovedAsync(SearchReviewRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var all = await _unitOfWork.Reviews.GetAllApprovedAsync(cancellationToken);

            var query = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var k = request.Keyword.Trim().ToLower();
                query = query.Where(r => (r.Name != null && r.Name.ToLower().Contains(k)) || (r.Address != null && r.Address.ToLower().Contains(k)));
            }

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var reviewDtos = _mapper.Map<List<ReviewDTO>>(items);
            reviewDtos = EnrichReviewDtosWithAbsoluteUrls(reviewDtos).ToList();

            return new PagedResponse<ReviewDTO>
            {
                Items = reviewDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)System.Math.Ceiling((double)totalCount / request.PageSize)
            };
        }
        public async Task<ReviewDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var Review = await _unitOfWork.Reviews
                .GetByIdAsync(id, cancellationToken);

            if (Review == null)
                return null;

            var reviewDto = _mapper.Map<ReviewDTO>(Review);
            reviewDto = EnrichReviewDtoWithAbsoluteUrls(reviewDto);
            return reviewDto;
        }

        public async Task<Guid> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException(" name is required.");

            var existingClient = await _unitOfWork.Reviews
                .FindAsync(x => x.Name == request.Name.Trim(), cancellationToken);

            var review = new Review
            {
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                Feedback = request.Feedback.Trim(),
                Rating = request.Rating,
              
                ImageUrl = request.ImageUrl
            };

            await _unitOfWork.Reviews.AddAsync(review, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return review.Id;
        }

        public async Task UpdateAsync(UpdateReviewRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Name is required.");

            var review = await _unitOfWork.Reviews
                .GetByIdAsync(request.Id, cancellationToken);

            if (review == null)
                throw new ResourceNotFoundException("Name not found.");

            review.Name = request.Name.Trim();
            review.Address = request.Address.Trim();
          
            review.Feedback = request.Feedback.Trim();
            review.ImageUrl = request.ImageUrl;


            _unitOfWork.Reviews.Update(review);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Reviews
                .GetByIdAsync(id, cancellationToken);

            if (review == null)
                throw new ResourceNotFoundException("Review not found.");

            review.IsDeleted = true;
            review.ModifiedAt = DateTime.UtcNow;
            review.ModifiedBy = "System"; // You

            _unitOfWork.Reviews.Update(review);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ToggleStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
            if (review == null)
                throw new ResourceNotFoundException("Review not found.");

            review.IsActive = isActive;
            _unitOfWork.Reviews.Update(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Converts relative file paths in a ReviewDTO to absolute URLs
        /// </summary>
        private ReviewDTO EnrichReviewDtoWithAbsoluteUrls(ReviewDTO reviewDto)
        {
            if (reviewDto == null)
                return reviewDto;

            if (!string.IsNullOrWhiteSpace(reviewDto.ImageUrl))
            {
                reviewDto.ImageUrl = _fileUrlService.GetAbsoluteUrl(reviewDto.ImageUrl);
            }

            return reviewDto;
        }

        /// <summary>
        /// Converts relative file paths in a collection of ReviewDTOs to absolute URLs
        /// </summary>
        private IEnumerable<ReviewDTO> EnrichReviewDtosWithAbsoluteUrls(IEnumerable<ReviewDTO> reviewDtos)
        {
            if (reviewDtos == null)
                return reviewDtos;

            foreach (var reviewDto in reviewDtos)
            {
                EnrichReviewDtoWithAbsoluteUrls(reviewDto);
            }

            return reviewDtos;
        }
    }
}
