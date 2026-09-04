using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PagedResponse<BookingDTO>> GetAllAsync(SearchBookingRequest request, CancellationToken cancellationToken = default)
        {
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            var pagedResponse = await _unitOfWork.Bookings
                .GetAllSearchAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    cancellationToken);

            var bookingDtos = _mapper.Map<IEnumerable<BookingDTO>>(
                pagedResponse.Items);


            return new PagedResponse<BookingDTO>
            {
                Items = bookingDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };
        }

        public async Task<BookingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
            if (booking == null) return null;
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<Guid> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Booking is required");

            var booking = new Booking
            {

                Name = request.Name?.Trim(),
               Phone = request.PhoneNo?.Trim(),
                WhatsApp = request.WhatsApp?.Trim(),
                Email = request.Email?.Trim(),
                TravelDate = request.TravelDate,
                PickupLocation = request.PickupLocation?.Trim(),
                DropLocation = request.DropLocation?.Trim(),
                PickupTime = request.PickupTime?.Trim(),
                Passengers = request.Passengers?.Trim(),
                Luggage = request.Luggage?.Trim(),
                SpecialRequirements = request.SpecialRequirements?.Trim()   
            };

            await _unitOfWork.Bookings.AddAsync(booking, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }

        public async Task UpdateAsync(UpdateBookingRequest request, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(request.Id, cancellationToken);
            if (booking == null) throw new ResourceNotFoundException("Booking not found.");

            booking.Name = request.Name?.Trim();
            booking.Phone = request.PhoneNo?.Trim();
            booking.WhatsApp = request.WhatsApp?.Trim();
            booking.Email = request.Email?.Trim();
            booking.TravelDate = request.TravelDate;
            booking.PickupLocation = request.PickupLocation?.Trim();
            booking.DropLocation = request.DropLocation?.Trim();
            booking.PickupTime = request.PickupTime?.Trim();
            booking.Passengers = request.Passengers?.Trim();
            booking.Luggage = request.Luggage?.Trim();
            booking.SpecialRequirements = request.SpecialRequirements?.Trim();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
            if (booking == null) throw new ResourceNotFoundException("Booking not found.");

            booking.IsDeleted = true;
            booking.ModifiedAt = DateTime.UtcNow;
            booking.ModifiedBy = "System";

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

    }
}
