using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.Interfaces.Services;
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
        // GET BY ID
        // ============================================================

        public async Task<CabDTO?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cab = await _unitOfWork.Cabs
                .GetCabByCategoryIdAsync(id, cancellationToken);

            if (cab == null)
                return null;

            var cabDto = _mapper.Map<CabDTO>(cab);

            return EnrichCabDtoWithAbsoluteUrls(cabDto);
        }
        // for SelfDrive Create
        public async Task<List<CabDTO>> GetByCategoryIdAsync(Guid categoryId,CancellationToken cancellationToken = default)
        {
            var cabs = await _unitOfWork.SelfDrives.GetByCategoryIdAsync( categoryId, cancellationToken);

            return _mapper.Map<List<CabDTO>>(cabs);
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

                LuggageCapacity = request.LuggageCapacity,

                Transmission = request.Transmission?.Trim(),

                Fuel = request.Fuel,

                PricePerDay = request.PricePerDay,

                ImageUrl = result.RelativePath,
                Discount = request.Discount,

                // Foreign Key
                CategoryId = request.CategoryId
            };

            if (request.FeatureIds != null && request.FeatureIds.Any())
            {
                var distinctFeatureIds = request.FeatureIds.Distinct().ToList();
                var validFeatures = await _unitOfWork.FeatureMasters
                    .FindAsync(f => distinctFeatureIds.Contains(f.Id) && !f.IsDeleted, cancellationToken);

                foreach (var feature in validFeatures)
                {
                    cab.CabFeatures.Add(new CabFeatures
                    {
                        CabId = cab.Id,
                        FeatureId = feature.Id,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    });
                }
            }

            await _unitOfWork.Cabs.AddAsync(
                cab,
                cancellationToken);

            if (request.IsSelfDrive)
            {
                var selfDrive = new SelfDrive
                {
                    //Id = Guid.NewGuid(),
                    CabId = cab.Id,
                    //PricePerDay = cab.PricePerDay,
                    //Discount = cab.Discount
                };

                await _unitOfWork.SelfDrives.AddAsync(selfDrive);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return cab.Id;

            
        }

        // ============================================================
        // UPDATE
        // ============================================================

        public async Task UpdateAsync( UpdateCabRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException(
                    "Cab name is required.");

            // Get existing Cab
            var cab = await _unitOfWork.Cabs
                .GetCabWithFeaturesForUpdateAsync(
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

                cab.ImageUrl = result.RelativePath;
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
            cab.Discount = request.Discount;
            // Update Foreign Key
            cab.CategoryId = request.CategoryId;

            // ========================================================
            // Update Cab Features
            // ========================================================
            var requestedFeatureIds = request.FeatureIds?.Distinct().ToList() ?? new List<Guid>();
            var validFeatures = new List<FeaturesMaster>();
            if (requestedFeatureIds.Any())
            {
                validFeatures = await _unitOfWork.FeatureMasters
                    .FindAsync(f => requestedFeatureIds.Contains(f.Id) && !f.IsDeleted, cancellationToken);
            }
            var validFeatureIds = validFeatures.Select(f => f.Id).ToHashSet();

            // 1. Soft-delete removed features
            foreach (var existingCf in cab.CabFeatures.Where(cf => !cf.IsDeleted))
            {
                if (!validFeatureIds.Contains(existingCf.FeatureId))
                {
                    existingCf.IsDeleted = true;
                    existingCf.ModifiedAt = DateTime.UtcNow;
                    existingCf.ModifiedBy = "System";
                }
            }

            // 2. Add new or restore previously deleted features
            foreach (var featureId in validFeatureIds)
            {
                var existingCf = cab.CabFeatures.FirstOrDefault(cf => cf.FeatureId == featureId);
                if (existingCf != null)
                {
                    if (existingCf.IsDeleted)
                    {
                        existingCf.IsDeleted = false;
                        existingCf.ModifiedAt = DateTime.UtcNow;
                        existingCf.ModifiedBy = "System";
                    }
                }
                else
                {
                    var newCf = new CabFeatures
                    {
                        Id = Guid.Empty,
                        CabId = cab.Id,
                        FeatureId = featureId,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    };

                    await _unitOfWork.CabFeatures.AddAsync(newCf, cancellationToken);
                    cab.CabFeatures.Add(newCf);
                }
            }

            cab.ModifiedAt = DateTime.UtcNow;
            cab.ModifiedBy = "System";

            // SelfDrive handling
            if (request.IsSelfDrive)
            {
                if (cab.SelfDrive == null)
                {
                    var newSelfDrive = new SelfDrive
                    {
                        Id = Guid.Empty,
                        CabId = cab.Id,
                        PricePerDay = cab.PricePerDay,
                        Discount = cab.Discount,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false
                    };

                    await _unitOfWork.SelfDrives.AddAsync(newSelfDrive, cancellationToken);
                    cab.SelfDrive = newSelfDrive;
                }
                else
                {
                    cab.SelfDrive.PricePerDay = cab.PricePerDay;
                    cab.SelfDrive.Discount = cab.Discount;
                    cab.SelfDrive.IsDeleted = false;
                    cab.SelfDrive.ModifiedAt = DateTime.UtcNow;
                    cab.SelfDrive.ModifiedBy = "System";
                }
            }
            else
            {
                if (cab.SelfDrive != null)
                {
                    cab.SelfDrive.IsDeleted = true;
                    cab.SelfDrive.ModifiedAt = DateTime.UtcNow;
                    cab.SelfDrive.ModifiedBy = "System";
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
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
            if (cabDto == null)
                return cabDto;
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
            if (cabDtos == null)
                return cabDtos;
            foreach (var cabDto in cabDtos)
            {
                EnrichCabDtoWithAbsoluteUrls(cabDto);
            }

            return cabDtos;
        }
    }
}