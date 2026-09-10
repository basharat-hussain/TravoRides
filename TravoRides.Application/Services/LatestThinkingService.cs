using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Application.Common.Models;
using TravoRides.Application.Common.Options;
using TravoRides.Application.Interfaces.Services;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace TravoRides.Application.Services
{
    public class LatestThinkingService : ILatestThinkingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFileUrlService _fileUrlService;

        public LatestThinkingService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IFileStorageService fileStorageService,
            IFileUrlService fileUrlService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _fileUrlService = fileUrlService;
        }

        public async Task<PagedResponse<LatestThinkingDTO>> GetAllAsync(SearchLatestThinkingRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var paged = await _unitOfWork.LatestThinkings.GetAllSearchAsync(request.PageNumber, request.PageSize, request.Keyword, request.Author, cancellationToken);

            var latestThinkingDtos = _mapper.Map<IEnumerable<LatestThinkingDTO>>(paged.Items);
            latestThinkingDtos = EnrichLatestThinkingDtosWithAbsoluteUrls(latestThinkingDtos);

            return new PagedResponse<LatestThinkingDTO>
            {
                Items = latestThinkingDtos,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                TotalPages = paged.TotalPages
            };
        }

        public async Task<LatestThinkingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.LatestThinkings.GetByIdAsync(id, cancellationToken);
            if (item == null) return null;

            var latestThinkingDto = _mapper.Map<LatestThinkingDTO>(item);
            latestThinkingDto = EnrichLatestThinkingDtoWithAbsoluteUrls(latestThinkingDto);
            return latestThinkingDto;
        }

        public async Task<Guid> CreateAsync(CreateLatestThinkingRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ImageUrl == null)
                throw new ValidationException("image is required");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.ImageUrl.ContentType,
                FolderName = "latestthinking",
                FileName = request.ImageUrl.FileName,
                Stream = request.ImageUrl.OpenReadStream(),
            };

            var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);

            if (result == null)
                throw new ValidationException("File upload failed");

            var entity = new LatestThinking
            {
                Title = request.Title.Trim(),
                Subtitle = request.Subtitle?.Trim(),
                ImageUrl = result.RelativePath,
                ImageAltText = request.ImageAltText,
                Author = request.Author.Trim(),
                Description = request.Description?.Trim(),
                MetaTitle = request.MetaTitle,
                MetaDescription = request.MetaDescription,
                Slug = request.Slug,
                CanonicalUrl = request.CanonicalUrl,
                Summary = request.Summary,
                KeyTakeaways = request.KeyTakeaways,
                PublishedOn = request.PublishedOn
            };

            await _unitOfWork.LatestThinkings.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }

        public async Task UpdateAsync(UpdateLatestThinkingRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var entity = await _unitOfWork.LatestThinkings.GetByIdAsync(request.Id, cancellationToken);
            if (entity == null)
                throw new ResourceNotFoundException("LatestThinking not found.");

            if (request.ImageUrl != null)
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    ContentType = request.ImageUrl.ContentType,
                    FolderName = "latestthinking",
                    FileName = request.ImageUrl.FileName,
                    Stream = request.ImageUrl.OpenReadStream(),
                };

                var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);
                if (result == null)
                    throw new ValidationException("File upload failed");

                entity.ImageUrl = result.RelativePath;
            }

            entity.Title = request.Title.Trim();
            entity.Subtitle = request.Subtitle?.Trim();
            entity.ImageAltText = request.ImageAltText;
            entity.Author = request.Author.Trim();
            entity.Description = request.Description?.Trim();
            entity.MetaTitle = request.MetaTitle;
            entity.MetaDescription = request.MetaDescription;
            entity.Slug = request.Slug;
            entity.CanonicalUrl = request.CanonicalUrl;
            entity.Summary = request.Summary;
            entity.KeyTakeaways = request.KeyTakeaways;
            entity.PublishedOn = request.PublishedOn;

            _unitOfWork.LatestThinkings.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.LatestThinkings.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new ResourceNotFoundException("LatestThinking not found.");

            entity.IsDeleted = true;
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = "System";

            _unitOfWork.LatestThinkings.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Converts relative file paths in a LatestThinkingDTO to absolute URLs
        /// </summary>
        private LatestThinkingDTO EnrichLatestThinkingDtoWithAbsoluteUrls(LatestThinkingDTO latestThinkingDto)
        {
            if (latestThinkingDto == null)
                return latestThinkingDto;

            if (!string.IsNullOrWhiteSpace(latestThinkingDto.ImageUrl))
            {
                latestThinkingDto.ImageUrl = _fileUrlService.GetAbsoluteUrl(latestThinkingDto.ImageUrl);
            }

            return latestThinkingDto;
        }

        /// <summary>
        /// Converts relative file paths in a collection of LatestThinkingDTOs to absolute URLs
        /// </summary>
        private IEnumerable<LatestThinkingDTO> EnrichLatestThinkingDtosWithAbsoluteUrls(IEnumerable<LatestThinkingDTO> latestThinkingDtos)
        {
            if (latestThinkingDtos == null)
                return latestThinkingDtos;

            foreach (var latestThinkingDto in latestThinkingDtos)
            {
                EnrichLatestThinkingDtoWithAbsoluteUrls(latestThinkingDto);
            }

            return latestThinkingDtos;
        }
    }
}
