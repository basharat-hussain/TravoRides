using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.TransitRate;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class TransitService : ITransitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IFileStorageService _fileStorageService;
        private readonly IFileUrlService _fileUrlService;

        public TransitService(IUnitOfWork unitOfWork, IMapper mapper, IFileStorageService fileStorageService, IFileUrlService fileUrlService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
            _fileUrlService = fileUrlService;
        }
       
        //=========================================== GET METHODS ===================================
        public async Task<List<TransitCabRateDTO>> GetCabsWithRatesAsync(Guid transitId, CancellationToken cancellationToken = default)
        {
            var transitRates = await _unitOfWork.TransitRates
                .GetByTransitIdAsync(transitId, cancellationToken);

            if (!transitRates.Any())
            {
                throw new ResourceNotFoundException(
                    "No cabs are available for this transit.");
            }

            return transitRates.Select(x => new TransitCabRateDTO
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
                    FinalRate = x.Rate -(x.Discount ?? 0)
                
            }).ToList();
        }
        public async Task<PagedResponse<TransitDTO>> GetAllAsync( SearchTransitRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Guard against malicious or invalid page values
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 8;
            if (request.PageSize > 100) request.PageSize = 100;

            // 2. Fetch structu
            var pagedResponse = await _unitOfWork.Transit
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    cancellationToken);
            // 3. Map entities to DTOs and convert file URLs to absolute paths
            var TransitDtos = _mapper.Map<IEnumerable<TransitDTO>>(pagedResponse.Items);
            TransitDtos = EnrichTransitDtosWithAbsoluteUrls(TransitDtos);

            // 4. Assemble and return mapped generic response
            return new PagedResponse<TransitDTO>
            {
                Items = TransitDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }
        public async Task<TransitDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var Transit = await _unitOfWork.Transit
                .GetByIdAsync(id, cancellationToken);

            if (Transit == null)
                return null;

            var TransitDto = _mapper.Map<TransitDTO>(Transit);
            TransitDto = EnrichTransitDtoWithAbsoluteUrls(TransitDto);
            return TransitDto;
        }
        public async Task<TransitCabRateDTO> GetTransitRateAsync(Guid cabId, Guid transitId, CancellationToken cancellationToken)
        {
            var transitRate = await _unitOfWork.TransitRates.GetByCabAndTransitAsync(cabId, transitId, cancellationToken);

            if (transitRate == null)
            {
                throw new ResourceNotFoundException("Rate not found for the selected cab and transit.");
            }

            // Get both discounts
            decimal transitDiscount = transitRate.Transit.Discount ?? 0;
            decimal transitRateDiscount = transitRate.Discount ?? 0;

            // Use the greater discount
            decimal applicableDiscount = Math.Max(transitDiscount, transitRateDiscount);

            // Calculate final price
            decimal finalRate = transitRate.Rate - applicableDiscount;

            // Prevent negative price
            if (finalRate < 0)
            {
                throw new ValidationException("Transit discount cannot be greater than the transit rate.");

            }
            return new TransitCabRateDTO
            {
                FinalRate = finalRate,
                Rate = transitRate.Rate,
                Discount = applicableDiscount
            };
        }
       
        //=========================================== CREATE METHODS =====================================
        public async Task<Guid> CreateAsync(CreateTransitRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Image == null)
            {
                throw new ValidationException("image is required");
            }


            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("Title is required.");

            var existingTransit = await _unitOfWork.Transit
                .FindAsync(x => x.Title == request.Title.Trim(), cancellationToken);

            if (existingTransit.Any())
                throw new ValidationException("Category-based entry with the same title already exists.");


            var fileUploadRequest = new FileUploadRequest
            {
                ContentType = request.Image.ContentType,
                FolderName = "Transit",
                FileName = request.Image.FileName,
                Stream = request.Image.OpenReadStream(),
            };



            var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);


            if (result == null)
                throw new ValidationException("File upload failed");

            var Transit = new Transit
            {
                Title = request.Title.Trim(),
               Price = request.Price,
               Discount = request.Discount,
                ImageUrl = result.AbsolutePath,
                Description = request.Description?.Trim()
            };

            await _unitOfWork.Transit.AddAsync(Transit, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Transit.Id;
        }
        public async Task AddCabsToTransitAsync(
            Guid TransitId,
            TransitCabRequest request,
            CancellationToken cancellationToken = default)
        {
            // 1. Check whether Transit exists
            var Transit = await _unitOfWork.Transit
                .GetByIdAsync(TransitId, cancellationToken);

            if (Transit == null || Transit.IsDeleted)
            {
                throw new ResourceNotFoundException("Transit not found.");
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

            // 4. Check whether this cab is already associated with the Transit
            var existingTransitRate = await _unitOfWork.TransitRates
                .GetByCabAndTransitAsync(
                    request.CabId,
                    TransitId,
                    cancellationToken);

            // 5. If active association already exists, don't add again
            if (existingTransitRate != null && !existingTransitRate.IsDeleted)
            {
                throw new ValidationException(
                    "This cab is already added to the Transit.");
            }

            // 6. If previously deleted, reactivate it
            if (existingTransitRate != null && existingTransitRate.IsDeleted)
            {
                existingTransitRate.IsDeleted = false;
                existingTransitRate.Rate = request.Rate;
                existingTransitRate.Discount = request.Discount;
                existingTransitRate.ModifiedAt = DateTime.UtcNow;
                existingTransitRate.ModifiedBy = "System";

                _unitOfWork.TransitRates.Update(existingTransitRate);

                return;
            }

            // 7. Create new TransitRate
            var TransitRate = new TransitRate
            {
                TransitId = TransitId,
                CabId = request.CabId,
                Rate = request.Rate,
                Discount = request.Discount
            };

            await _unitOfWork.TransitRates.AddAsync(
                TransitRate,
                cancellationToken);
        }
        //====================================== UPDATE METHODS =======================================
        public async Task UpdateAsync(UpdateTransitRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ValidationException("title is required.");

            var Transit = await _unitOfWork.Transit
                .GetByIdAsync(request.Id, cancellationToken);

            if (Transit == null)
                throw new ResourceNotFoundException("Category-based entry not found.");


            // 2. Handle optional Logo update
            if (request.ImageUrl != null)
            {
                var fileUploadRequest = new FileUploadRequest
                {
                    ContentType = request.Image.ContentType,
                    FolderName = "Transit",
                    FileName = request.Image.FileName,
                    Stream = request.Image.OpenReadStream(),
                };

                var result = await _fileStorageService.UploadAsync(fileUploadRequest, cancellationToken);
                if (result == null)
                    throw new ValidationException("File upload failed");

                // Optional: Call a service to delete the old file using client.LogoUrl here

                Transit.ImageUrl = result.AbsolutePath;
            }

            // 3. Update remaining properties
            Transit.Title = request.Title.Trim();
            Transit.Description = request.Description?.Trim();
            Transit.Price = request.Price;
            Transit.Discount = request.Discount;
            

            _unitOfWork.Transit.Update(Transit);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateTransitCabAsync(Guid TransitId, Guid cabId, UpdateTransitCabRequest request, CancellationToken cancellationToken = default)
        {
            // Check Transit
            var Transit = await _unitOfWork.Transit.GetByIdAsync(TransitId, cancellationToken);

            if (Transit == null || Transit.IsDeleted) throw new ResourceNotFoundException("Transit not found.");

            // Find TransitRate
            var TransitRate = await _unitOfWork.TransitRates.GetByCabAndTransitAsync(cabId, TransitId, cancellationToken);

            if (TransitRate == null)
                throw new ResourceNotFoundException("The selected cab is not associated with this Transit.");

            // Update rate and discount
            TransitRate.Rate = request.Rate;
            TransitRate.Discount = request.Discount;

            _unitOfWork.TransitRates.Update(TransitRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }


        //========================================= DELETE METHODS ==============================
        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var portfolio = await _unitOfWork.Transit
                .GetByIdAsync(id, cancellationToken);

            if (portfolio == null)
                throw new ResourceNotFoundException("Category-based entry not found.");

            portfolio.IsDeleted = true;
            portfolio.ModifiedAt = DateTime.UtcNow;
            portfolio.ModifiedBy = "System"; // You

            _unitOfWork.Transit.Update(portfolio);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveCabFromTransitAsync(Guid TransitId, Guid cabId, CancellationToken cancellationToken = default)
        {
            // Check Transit
            var Transit = await _unitOfWork.Transit.GetByIdAsync(TransitId, cancellationToken);

            if (Transit == null || Transit.IsDeleted) throw new ResourceNotFoundException("Transit not found.");

            // Find TransitRate
            var TransitRate = await _unitOfWork.TransitRates
                .GetByCabAndTransitAsync(cabId, TransitId, cancellationToken);

            if (TransitRate == null)
                throw new ResourceNotFoundException("The selected cab is not associated with this Transit.");

            TransitRate.IsDeleted = true;
            TransitRate.ModifiedAt = DateTime.UtcNow;
            TransitRate.ModifiedBy = "System"; // You

            // Remove relationship
            _unitOfWork.TransitRates.Update(TransitRate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Converts relative file paths in a TransitDTO to absolute URLs
        /// </summary>
        private TransitDTO EnrichTransitDtoWithAbsoluteUrls(TransitDTO TransitDto)
        {
            if (TransitDto == null)
                return TransitDto;

            if (!string.IsNullOrWhiteSpace(TransitDto.ImageUrl))
            {
                TransitDto.ImageUrl = _fileUrlService.GetAbsoluteUrl(TransitDto.ImageUrl);
            }

            return TransitDto;
        }

        /// <summary>
        /// Converts relative file paths in a collection of TransitDTOs to absolute URLs
        /// </summary>
        private IEnumerable<TransitDTO> EnrichTransitDtosWithAbsoluteUrls(IEnumerable<TransitDTO> TransitDtos)
        {
            if (TransitDtos == null)
                return TransitDtos;

            foreach (var TransitDto in TransitDtos)
            {
                EnrichTransitDtoWithAbsoluteUrls(TransitDto);
            }

            return TransitDtos;
        }
    }
}