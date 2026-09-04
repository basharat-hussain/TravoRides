using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.SelfDrive;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class SelfDriveService : ISelfDriveService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SelfDriveService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<SelfDriveDTO>> GetAllAsync(SearchSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            var pagedResponse = await _unitOfWork.SelfDrives
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    request.CabId,
                    cancellationToken);

            var selfDriveDtos = _mapper.Map<IEnumerable<SelfDriveDTO>>(
                pagedResponse.Items);


            return new PagedResponse<SelfDriveDTO>
            {
                Items = selfDriveDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<SelfDriveDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var selfDrive = await _unitOfWork.SelfDrives.GetSelfDriveByCabAsync(id, cancellationToken);
            if (selfDrive == null) return null;
            return _mapper.Map<SelfDriveDTO>(selfDrive);
        }

        public async Task<Guid> CreateAsync(CreateSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ResourceNotFoundException("SelfDrive not found");
            // Validate Cab
            var cab = await _unitOfWork.Cabs
                .GetByIdAsync(
                    request.CabId,
                    cancellationToken);

            if (cab == null || cab.IsDeleted)
                throw new ResourceNotFoundException(
                    "Cab not found.");

            var selfDrive = new SelfDrive
            {
                CabId = request.CabId,
                PricePerDay = request.PricePerDay,
                Discount = request.Discount,
            };

            await _unitOfWork.SelfDrives.AddAsync(selfDrive, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return selfDrive.Id;
        }

        public async Task UpdateAsync(UpdateSelfDriveRequest request, CancellationToken cancellationToken = default)
        {
            var selfDrive = await _unitOfWork.SelfDrives.GetByIdAsync(request.Id, cancellationToken);
            if (selfDrive == null) throw new ResourceNotFoundException("Self-drive not found.");

            selfDrive.PricePerDay = request.PricePerDay;
            selfDrive.Discount = request.Discount;

            _unitOfWork.SelfDrives.Update(selfDrive);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var selfDrive = await _unitOfWork.SelfDrives.GetByIdAsync(id, cancellationToken);
            if (selfDrive == null) throw new ResourceNotFoundException("Self-drive not found.");

            selfDrive.IsDeleted = true;
            selfDrive.ModifiedAt = DateTime.UtcNow;
            selfDrive.ModifiedBy = "System";

            _unitOfWork.SelfDrives.Update(selfDrive);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
