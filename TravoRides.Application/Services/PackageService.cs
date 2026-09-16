using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.PackageRate;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravoRides.Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorage;
        private readonly IFileUrlService _fileUrl;

        public PackageService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorage,
            IFileUrlService fileUrl)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUrl = fileUrl;
            _fileStorage = fileStorage;
        }

        //==================================== GET METHODS =======================================
        public async Task<List<PackageCabRateDTO>> GetCabsWithRatesAsync( Guid packageId,  CancellationToken cancellationToken = default)
        {
            var packageRates = await _unitOfWork.PackageRates
                .GetByPackageIdAsync(packageId, cancellationToken);

            if (!packageRates.Any())
            {
                throw new ResourceNotFoundException(
                    "No cabs are available for this package.");
            }

            return packageRates.Select(x =>  new PackageCabRateDTO
                {
                    CabId = x.CabId,
                    CabName = x.Cab.Name,
                    ImageUrl = x.Cab.ImageUrl,
                    SeatingCapacity = x.Cab.SeatingCapacity,
                    LuggageCapacity = x.Cab.LuggageCapacity,
                    Fuel = x.Cab.Fuel,
                    Transmission = x.Cab.Transmission,
                    Rate = x.Rate,
                    Discount = x.Discount,
                    FinalRate = x.Rate - (x.Discount ?? 0)
                             
            }).ToList();
        }

        public async Task<PackageCabRateDTO> GetPackageRateAsync(Guid cabId, Guid packageId, CancellationToken cancellationToken)
        {
            var packageRate = await _unitOfWork.PackageRates.GetByCabAndPackageAsync(cabId, packageId, cancellationToken);

            if (packageRate == null)
            {
                throw new ResourceNotFoundException("Rate not found for the selected cab and transit.");
            }

            // Get both discounts
            decimal packageDiscount = packageRate.Package.Discount ?? 0;
            decimal packageRateDiscount = packageRate.Discount ?? 0;

            // Use the greater discount
            decimal applicableDiscount = Math.Max(packageDiscount, packageRateDiscount);

            // Calculate final price
            decimal finalRate = packageRate.Rate - applicableDiscount;

            // Prevent negative price
          
            
                if (finalRate < 0)
                {
                    throw new ValidationException("Package discount cannot be greater than the package rate.");
                }


            return new PackageCabRateDTO
            {
                Rate = packageRate.Rate,
                Discount = applicableDiscount,
                FinalRate = finalRate

            };
        }
        public async Task<PagedResponse<PackageDTO>> GetAllAsync(SearchPackageRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Guard against malicious or invalid page values
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 8;
            if (request.PageSize > 100) request.PageSize = 100;

            // 2. Fetch structural entity pagination block from repository
            var pagedResponse = await _unitOfWork.Packages
                .GetAllAsync(request.PageNumber, request.PageSize, request.Keyword, cancellationToken);

            // 3. Map entities to DTOs and convert file URLs to absolute paths
            var packageDto = _mapper.Map<IEnumerable<PackageDTO>>(pagedResponse.Items);
            packageDto = EnrichPackageDtosWithAbsoluteUrls(packageDto);

            // 4. Assemble and return mapped generic response
            return new PagedResponse<PackageDTO>
            {
                Items = packageDto,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<PackageDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var Package = await _unitOfWork.Packages
                .GetByIdAsync(id, cancellationToken);

            if (Package == null)
                return null;

            var PackageDto = _mapper.Map<PackageDTO>(Package);
            PackageDto = EnrichPackageDtoWithAbsoluteUrls(PackageDto);
            return PackageDto;
        }
      
        //=========================================CREATE METHODS ==========================================
        public async Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Image == null)
            {
                throw new ValidationException("image is required");
            }


            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var existingPackage = await _unitOfWork.Packages
                .FindAsync(x => x.Title == request.Title.Trim(), cancellationToken);

            if (existingPackage.Any())
                throw new ValidationException("Package with the same title already exists.");


            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.Image.ContentType,
                FolderName = "Package",
                FileName = request.Image.FileName,
                Stream = request.Image.OpenReadStream(),
            };



            var result = await _fileStorage.UploadAsync(fileUploadRequest, cancellationToken);


            if (result == null)
                throw new ValidationException("File upload failed");

            var package = new Package
            {
                Title = request.Title.Trim(),
                Itinerary = request.Itinerary.Trim(),
                ImageUrl = result.AbsolutePath,
                Inclusions = request.Inclusions,
                Route = request.Route,
                Discount = request.Discount,
                Distance = request.Distance,
                Price = request.Price,
                Duration = request.Duration,
                PlacesCovered = request.PlacesCovered
               
            };

            await _unitOfWork.Packages.AddAsync(package, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return package.Id;
        }
        public async Task AddCabsToPackageAsync(Guid packageId, PackageCabRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Check whether package exists
            var package = await _unitOfWork.Packages
                .GetByIdAsync(packageId, cancellationToken);

            if (package == null || package.IsDeleted)
            {
                throw new ResourceNotFoundException("Package not found.");
            }

            // 2. Validate cab selection
            if (request == null || request.CabId == Guid.Empty)
            {
                throw new ValidationException("A valid cab must be selected.");
            }

            // 3. Check whether cab exists
            var cab = await _unitOfWork.Cabs
                .GetByIdAsync(request.CabId, cancellationToken);

            if (cab == null || cab.IsDeleted)
            {
                throw new ResourceNotFoundException("Cab not found.");
            }

            // 4. Check whether this cab is already associated with the package
            var existingPackageRate = await _unitOfWork.PackageRates
                .GetByCabAndPackageAsync( request.CabId, packageId,  cancellationToken);

            // 5. If active association already exists, don't add again
            if (existingPackageRate != null && !existingPackageRate.IsDeleted)
            {
                throw new ValidationException(
                    "This cab is already added to the package.");
            }

            // 6. If previously deleted, reactivate it
            if (existingPackageRate != null && existingPackageRate.IsDeleted)
            {
                existingPackageRate.IsDeleted = false;
                existingPackageRate.Rate = request.Rate;
                existingPackageRate.Discount = request.Discount;
                existingPackageRate.ModifiedAt = DateTime.UtcNow;
                existingPackageRate.ModifiedBy = "System";

                _unitOfWork.PackageRates.Update(existingPackageRate);

                return;
            }

            // 7. Create new PackageRate
            var packageRate = new PackageRate
            {
                PackageId = packageId,
                CabId = request.CabId,
                Rate = request.Rate,
                Discount = request.Discount
            };

            await _unitOfWork.PackageRates.AddAsync(
                packageRate,
                cancellationToken);
        }

        //=================================== UPDATE METHODS ============================================
        public async Task UpdateAsync(UpdatePackageRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("title is required.");

            var package = await _unitOfWork.Packages
                .GetByIdAsync(request.Id, cancellationToken);

            if (package == null)
                throw new ResourceNotFoundException("Package not found.");


            // 2. Handle optional Logo update
            if (request.ImageUrl != null)
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    ContentType = request.Image.ContentType,
                    FolderName = "package",
                    FileName = request.Image.FileName,
                    Stream = request.Image.OpenReadStream(),
                };

                var result = await _fileStorage.UploadAsync(fileUploadRequest, cancellationToken);
                if (result == null)
                    throw new ValidationException("File upload failed");

                // Optional: Call a service to delete the old file using client.LogoUrl here

                package.ImageUrl = result.AbsolutePath;
            }

            // 3. Update remaining properties
            package.Title = request.Title.Trim();
            package.Itinerary = request.Itinerary.Trim();
            package.Inclusions = request.Inclusions;
            package.Route = request.Route;
            package.Discount = request.Discount;
            package.Distance = request.Distance;
            package.Price = request.Price;
            package.Duration = request.Duration;
            package.PlacesCovered = request.PlacesCovered;
            _unitOfWork.Packages.Update(package);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdatePackageCabAsync(Guid packageId, Guid cabId, UpdatePackageCabRequest request, CancellationToken cancellationToken = default)
        {
            // Check package
            var package = await _unitOfWork.Packages.GetByIdAsync(packageId, cancellationToken);

            if (package == null || package.IsDeleted)
                throw new ResourceNotFoundException("Package not found.");

            // Find PackageRate
            var packageRate = await _unitOfWork.PackageRates.GetByCabAndPackageAsync(cabId, packageId, cancellationToken);

            if (packageRate == null)
                throw new ResourceNotFoundException("The selected cab is not associated with this package.");

            // Update rate and discount
            packageRate.Rate = request.Rate;
            packageRate.Discount = request.Discount;

            _unitOfWork.PackageRates.Update(packageRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        //==================================== DELETE METHODS =========================================
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var package = await _unitOfWork.Packages
                .GetByIdAsync(id, cancellationToken);

            if (package == null)
                throw new ResourceNotFoundException("Package not found.");

            package.IsDeleted = true;
            package.ModifiedAt = DateTime.UtcNow;
            package.ModifiedBy = "System"; // You

            _unitOfWork.Packages.Update(package);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task RemoveCabFromPackageAsync( Guid packageId, Guid cabId, CancellationToken cancellationToken = default)
        {
            // Check package
            var package = await _unitOfWork.Packages.GetByIdAsync(packageId, cancellationToken);

            if (package == null || package.IsDeleted)throw new ResourceNotFoundException("Package not found.");

            // Find PackageRate
            var packageRate = await _unitOfWork.PackageRates
                .GetByCabAndPackageAsync( cabId, packageId, cancellationToken);

            if (packageRate == null)
                throw new ResourceNotFoundException("The selected cab is not associated with this package.");
           
            packageRate.IsDeleted = true;
            packageRate.ModifiedAt = DateTime.UtcNow;
            packageRate.ModifiedBy = "System"; // You

            // Remove relationship
            _unitOfWork.PackageRates.Update(packageRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
     
        /// <summary>
        /// Converts relative file paths in a PackageDTO to absolute URLs
        /// </summary>
        private PackageDTO EnrichPackageDtoWithAbsoluteUrls(PackageDTO packageDto)
        {
            if (packageDto == null)
                return packageDto;

            if (!string.IsNullOrWhiteSpace(packageDto.ImageUrl))
            {
                packageDto.ImageUrl = _fileUrl.GetAbsoluteUrl(packageDto.ImageUrl);
            }

            return packageDto;
        }

        /// <summary>
        /// Converts relative file paths in a collection of PackageDTOs to absolute URLs
        /// </summary>
        private IEnumerable<PackageDTO> EnrichPackageDtosWithAbsoluteUrls(IEnumerable<PackageDTO> packageDto)
        {
            if (packageDto == null)
                return packageDto;

            foreach (var PackageDto in packageDto)
            {
                EnrichPackageDtoWithAbsoluteUrls(PackageDto);
            }

            return packageDto;
        }
    }
}