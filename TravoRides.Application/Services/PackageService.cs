using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.Common.Models;
using TravoRiders.Application.Interfaces.Services;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

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

        public async Task<PagedResponse<PackageDTO>> GetAllAsync(
      SearchPackageRequest request,
      CancellationToken cancellationToken = default)
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
            var portfolio = await _unitOfWork.Packages
                .GetByIdAsync(id, cancellationToken);

            if (portfolio == null)
                return null;

            var portfolioDto = _mapper.Map<PackageDTO>(portfolio);
            portfolioDto = EnrichPackageDtoWithAbsoluteUrls(portfolioDto);
            return portfolioDto;
        }

        public async Task<Guid> CreateAsync(CreatePackageRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Image == null)
            {
                throw new ValidationException("image is required");
            }


            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var existingPortfolio = await _unitOfWork.Packages
                .FindAsync(x => x.Title == request.Title.Trim(), cancellationToken);

            if (existingPortfolio.Any())
                throw new ValidationException("Package with the same title already exists.");


            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.Image.ContentType,
                FolderName = "portfolio",
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

        /// <summary>
        /// Converts relative file paths in a PortfolioDTO to absolute URLs
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
        /// Converts relative file paths in a collection of PortfolioDTOs to absolute URLs
        /// </summary>
        private IEnumerable<PackageDTO> EnrichPackageDtosWithAbsoluteUrls(IEnumerable<PackageDTO> packageDto)
        {
            if (packageDto == null)
                return packageDto;

            foreach (var portfolioDto in packageDto)
            {
                EnrichPackageDtoWithAbsoluteUrls(portfolioDto);
            }

            return packageDto;
        }
    }
}