using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Common.Models;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
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

        public async Task<PagedResponse<TransitDTO>> GetAllAsync(
      SearchTransitRequest request,
      CancellationToken cancellationToken = default)
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

        public async Task<Guid> CreateAsync(CreateTransitRequest request, CancellationToken cancellationToken = default)
        {
            if (request.ImageUrl == null)
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
                ContentType = request.ImageUrl.ContentType,
                FolderName = "Transit",
                FileName = request.ImageUrl.FileName,
                Stream = request.ImageUrl.OpenReadStream(),
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
                    ContentType = request.ImageFile.ContentType,
                    FolderName = "Transit",
                    FileName = request.ImageFile.FileName,
                    Stream = request.ImageFile.OpenReadStream(),
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