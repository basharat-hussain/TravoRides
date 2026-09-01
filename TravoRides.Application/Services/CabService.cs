using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Common.Models;
using TravoRiders.Application.Interfaces.Services;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class CabService : ICabService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;
        private readonly IFileUrlService _fileUrlService;

        public CabService(
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

        // ============================================================
        // GET ALL - PAGINATED + SEARCH + CATEGORY FILTER
        // ============================================================

        public async Task<PagedResponse<CabDTO>> GetAllAsync(
            SearchCabRequest request,
            CancellationToken cancellationToken = default)
        {
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            var pagedResponse = await _unitOfWork.Cabs
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    request.CategoryId,
                    cancellationToken);

            var cabDtos = _mapper.Map<IEnumerable<CabDTO>>(
                pagedResponse.Items);

            cabDtos = EnrichCabDtosWithAbsoluteUrls(cabDtos);

            return new PagedResponse<CabDTO>
            {
                Items = cabDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        // ============================================================
        // GET ALL
        // ============================================================

        public async Task<IEnumerable<CabDTO>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var cabs = await _unitOfWork.Cabs
                .GetAllAsync(cancellationToken);

            var cabDtos = _mapper.Map<List<CabDTO>>(cabs);

            return EnrichCabDtosWithAbsoluteUrls(cabDtos);
        }

        // ============================================================
        // GET BY ID
        // ============================================================

        public async Task<CabDTO?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cab = await _unitOfWork.Cabs
                .GetByIdAsync(id, cancellationToken);

            if (cab == null)
                return null;

            var cabDto = _mapper.Map<CabDTO>(cab);

            return EnrichCabDtoWithAbsoluteUrls(cabDto);
        }

        // ============================================================
        // CREATE
        // ============================================================

        public async Task<Guid> CreateAsync(
            CreateCabRequest request,
            CancellationToken cancellationToken = default)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Cab name is required.");

            // Validate image
            if (request.Image == null)
                throw new ValidationException("Cab image is required.");

            // Validate Category
            var category = await _unitOfWork.Categories
                .GetByIdAsync(
                    request.CategoryId,
                    cancellationToken);

            if (category == null || category.IsDeleted)
                throw new ResourceNotFoundException(
                    "Category not found.");

            // Check duplicate cab name
            var existingCab = await _unitOfWork.Cabs
                .FindAsync(
                    x => x.Name == request.Name.Trim(),
                    cancellationToken);

            if (existingCab.Any())
                throw new ValidationException(
                    "Cab with the same name already exists.");

            // ========================================================
            // Upload Image
            // ========================================================

            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.Image.ContentType,
                FolderName = "cabs",
                FileName = request.Image.FileName,
                Stream = request.Image.OpenReadStream()
            };

            var result = await _fileStorageService.UploadAsync(
                fileUploadRequest,
                cancellationToken);

            if (result == null)
                throw new ValidationException(
                    "File upload failed.");

            // ========================================================
            // Create Entity
            // ========================================================

            var cab = new Cab
            {
                Name = request.Name.Trim(),

                Description = request.Description?.Trim(),

                SeatingCapacity = request.SeatingCapacity,

                LuggageCapacity =
                    request.LuggageCapacity,

                Transmission =
                    request.Transmission?.Trim(),

                Fuel = request.Fuel,

                PricePerDay = request.PricePerDay,

                ImageUrl = result.AbsolutePath,
                Discount = request.Discount,

                // Foreign Key
                CategoryId = request.CategoryId
            };

            await _unitOfWork.Cabs.AddAsync(
                cab,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return cab.Id;
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync(
            UpdateCabRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException(
                    "Cab name is required.");

            // Get existing Cab
            var cab = await _unitOfWork.Cabs
                .GetByIdAsync(
                    request.Id,
                    cancellationToken);

            if (cab == null)
                throw new ResourceNotFoundException(
                    "Cab not found.");

            // ========================================================
            // Validate Category
            // ========================================================

            var category = await _unitOfWork.Categories
                .GetByIdAsync(
                    request.CategoryId,
                    cancellationToken);

            if (category == null || category.IsDeleted)
                throw new ResourceNotFoundException(
                    "Category not found.");

            // ========================================================
            // Check duplicate name
            // ========================================================

            var existingCab = await _unitOfWork.Cabs
                .FindAsync(
                    x => x.Name == request.Name.Trim()
                         && x.Id != request.Id,
                    cancellationToken);

            if (existingCab.Any())
                throw new ValidationException(
                    "Cab with the same name already exists.");

            // ========================================================
            // Optional Image Update
            // ========================================================

            if (request.Image != null)
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    ContentType = request.Image.ContentType,
                    FolderName = "cabs",
                    FileName = request.Image.FileName,
                    Stream = request.Image.OpenReadStream()
                };

                var result = await _fileStorageService.UploadAsync(
                    fileUploadRequest,
                    cancellationToken);

                if (result == null)
                    throw new ValidationException(
                        "File upload failed.");
                    
                // Optional:
                // Delete the old image here if your
                // FileStorageService supports it.

                cab.ImageUrl = result.AbsolutePath;
            }

            // ========================================================
            // Update Properties
            // ========================================================

            cab.Name = request.Name.Trim();

            cab.Description =
                request.Description?.Trim();

            cab.SeatingCapacity =
                request.SeatingCapacity;

            cab.LuggageCapacity =
                request.LuggageCapacity;

            cab.Transmission =
                request.Transmission?.Trim();

            cab.Fuel = request.Fuel;

            cab.PricePerDay = request.PricePerDay;

            // Update Foreign Key
            cab.CategoryId = request.CategoryId;

            _unitOfWork.Cabs.Update(cab);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // DELETE - SOFT DELETE
        // ============================================================

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cab = await _unitOfWork.Cabs
                .GetByIdAsync(
                    id,
                    cancellationToken);

            if (cab == null)
                throw new ResourceNotFoundException(
                    "Cab not found.");

            cab.IsDeleted = true;
            cab.ModifiedAt = DateTime.UtcNow;
            cab.ModifiedBy = "System";

            _unitOfWork.Cabs.Update(cab);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }

        // ============================================================
        // IMAGE URL - SINGLE DTO
        // ============================================================

        private CabDTO EnrichCabDtoWithAbsoluteUrls(
            CabDTO cabDto)
        {
            if (!string.IsNullOrWhiteSpace(cabDto.ImageUrl))
            {
                cabDto.ImageUrl =
                    _fileUrlService.GetAbsoluteUrl(
                        cabDto.ImageUrl);
            }

            return cabDto;
        }

        // ============================================================
        // IMAGE URL - COLLECTION
        // ============================================================

        private IEnumerable<CabDTO> EnrichCabDtosWithAbsoluteUrls(
            IEnumerable<CabDTO> cabDtos)
        {
            foreach (var cabDto in cabDtos)
            {
                EnrichCabDtoWithAbsoluteUrls(cabDto);
            }

            return cabDtos;
        }
    }
}