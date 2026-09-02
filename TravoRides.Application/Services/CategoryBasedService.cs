using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Common.Models;
using TravoRiders.Application.Interfaces.Services;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.CategoryBased;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class CategoryBasedService : ICategoryBasedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IFileStorageService _fileStorageService;
        private readonly IFileUrlService _fileUrlService;

        public CategoryBasedService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService, IFileUrlService fileUrlService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _fileUrlService = fileUrlService;
        }

        public async Task<PagedResponse<CategoryBasedDTO>> GetAllAsync(
      SearchCategoryBasedRequest request,
      CancellationToken cancellationToken = default)
        {
            // 1. Guard against malicious or invalid page values
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 8;
            if (request.PageSize > 100) request.PageSize = 100;

            // 2. Fetch structu
            var pagedResponse = await _unitOfWork.CategoryBased
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    cancellationToken);
            // 3. Map entities to DTOs and convert file URLs to absolute paths
            var categoryBasedDtos = _mapper.Map<IEnumerable<CategoryBasedDTO>>(pagedResponse.Items);
            categoryBasedDtos = EnrichCategoryBasedDtosWithAbsoluteUrls(categoryBasedDtos);

            // 4. Assemble and return mapped generic response
            return new PagedResponse<CategoryBasedDTO>
            {
                Items = categoryBasedDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<CategoryBasedDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var categoryBased = await _unitOfWork.CategoryBased
                .GetByIdAsync(id, cancellationToken);

            if (categoryBased == null)
                return null;

            var categoryBasedDto = _mapper.Map<CategoryBasedDTO>(categoryBased);
            categoryBasedDto = EnrichCategoryBasedDtoWithAbsoluteUrls(categoryBasedDto);
            return categoryBasedDto;
        }

        public async Task<Guid> CreateAsync(CreateCategoryBasedRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ImageUrl == null)
            {
                throw new ValidationException("image is required");
            }


            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var existingCategoryBased = await _unitOfWork.CategoryBased
                .FindAsync(x => x.Title == request.Title.Trim(), cancellationToken);

            if (existingCategoryBased.Any())
                throw new ValidationException("Category-based entry with the same title already exists.");


            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.ImageUrl.ContentType,
                FolderName = "categoryBased",
                FileName = request.ImageUrl.FileName,
                Stream = request.ImageUrl.OpenReadStream(),
            };



            var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);


            if (result == null)
                throw new ValidationException("File upload failed");

            var categoryBased = new CategoryBased
            {
                Title = request.Title.Trim(),
               Price = request.Price,
               Discount = request.Discount,
                ImageUrl = result.AbsolutePath,
                Description = request.Description?.Trim()
            };

            await _unitOfWork.CategoryBased.AddAsync(categoryBased, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return categoryBased.Id;
        }

        public async Task UpdateAsync(UpdateCategoryBasedRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("title is required.");

            var categoryBased = await _unitOfWork.CategoryBased
                .GetByIdAsync(request.Id, cancellationToken);

            if (categoryBased == null)
                throw new ResourceNotFoundException("Category-based entry not found.");


            // 2. Handle optional Logo update
            if (request.ImageUrl != null)
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    ContentType = request.ImageFile.ContentType,
                    FolderName = "categoryBased",
                    FileName = request.ImageFile.FileName,
                    Stream = request.ImageFile.OpenReadStream(),
                };

                var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);
                if (result == null)
                    throw new ValidationException("File upload failed");

                // Optional: Call a service to delete the old file using client.LogoUrl here

                categoryBased.ImageUrl = result.AbsolutePath;
            }

            // 3. Update remaining properties
            categoryBased.Title = request.Title.Trim();
            categoryBased.Description = request.Description?.Trim();
            categoryBased.Price = request.Price;
            categoryBased.Discount = request.Discount;

            _unitOfWork.CategoryBased.Update(categoryBased);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var portfolio = await _unitOfWork.CategoryBased
                .GetByIdAsync(id, cancellationToken);

            if (portfolio == null)
                throw new ResourceNotFoundException("Category-based entry not found.");

            portfolio.IsDeleted = true;
            portfolio.ModifiedAt = DateTime.UtcNow;
            portfolio.ModifiedBy = "System"; // You

            _unitOfWork.CategoryBased.Update(portfolio);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Converts relative file paths in a CategoryBasedDTO to absolute URLs
        /// </summary>
        private CategoryBasedDTO EnrichCategoryBasedDtoWithAbsoluteUrls(CategoryBasedDTO categoryBasedDto)
        {
            if (categoryBasedDto == null)
                return categoryBasedDto;

            if (!string.IsNullOrWhiteSpace(categoryBasedDto.ImageUrl))
            {
                categoryBasedDto.ImageUrl = _fileUrlService.GetAbsoluteUrl(categoryBasedDto.ImageUrl);
            }

            return categoryBasedDto;
        }

        /// <summary>
        /// Converts relative file paths in a collection of CategoryBasedDTOs to absolute URLs
        /// </summary>
        private IEnumerable<CategoryBasedDTO> EnrichCategoryBasedDtosWithAbsoluteUrls(IEnumerable<CategoryBasedDTO> categoryBasedDtos)
        {
            if (categoryBasedDtos == null)
                return categoryBasedDtos;

            foreach (var categoryBasedDto in categoryBasedDtos)
            {
                EnrichCategoryBasedDtoWithAbsoluteUrls(categoryBasedDto);
            }

            return categoryBasedDtos;
        }
    }
}