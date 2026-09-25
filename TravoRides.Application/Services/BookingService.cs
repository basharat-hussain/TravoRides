using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.BookingReport;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;

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
                .GetAllSearchAsync(request, cancellationToken);

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

        public async Task<BookingReportResponse> GetBookingReportAsync(SearchBookingRequest request, CancellationToken cancellationToken = default)
        {
            // Defensive pagination
            if (request.PageNumber < 1)
                request.PageNumber = 1;

            if (request.PageSize < 1)
                request.PageSize = 8;

            if (request.PageSize > 100)
                request.PageSize = 100;

            // Get report from repository
            var report = await _unitOfWork.Bookings
                .GetBookingReportAsync(
                    request.PageNumber,
                    request.PageSize,
                    request.Keyword,
                    request.FromDate,
                    request.ToDate,
                    request.IsConfirmed,
                    cancellationToken);

            // Map booking records
            var bookingDtos = _mapper.Map<IEnumerable<BookingReportDTO>>(
                report.Bookings.Items);

            return new BookingReportResponse
            {
                Summary = report.Summary,

                Bookings = new PagedResponse<BookingReportDTO>
                {
                    Items = bookingDtos,
                    PageNumber = report.Bookings.PageNumber,
                    PageSize = report.Bookings.PageSize,
                    TotalCount = report.Bookings.TotalCount,
                    TotalPages = report.Bookings.TotalPages
                }
            };
        }

        public static int CalculateTotalDays(DateTime travelDate, DateTime? returnDate)
        {
            if (!returnDate.HasValue)
            {
                return 1;
            }

            // Compare calendar dates irrespective of hours
            var diffDays = (returnDate.Value.Date - travelDate.Date).Days;
            if (diffDays < 0)
            {
                return 1;
            }

            // Same day is 1 day (diffDays = 0, so diffDays + 1 = 1)
            // Second day drop is 2 days (diffDays = 1, so diffDays + 1 = 2)
            return diffDays + 1;
        }

        private async Task<decimal> GetBookingAmountAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var totalDays = CalculateTotalDays(request.TravelDate, request.ReturnDate);

            decimal basePrice;
            decimal discount;

            switch (request.BookingType)
            {
                case BookingType.Transit:
                    (basePrice, discount) = await GetTransitPricingAsync(request, cancellationToken);
                    break;

                case BookingType.Package:
                    (basePrice, discount) = await GetPackagePricingAsync(request, cancellationToken);
                    break;

                case BookingType.Cab:
                    (basePrice, discount) = await GetCabPricingAsync(request, cancellationToken);
                    break;

                case BookingType.SelfDrive:
                    (basePrice, discount) = await GetSelfDrivePricingAsync(request, cancellationToken);
                    break;

                default:
                    throw new ValidationException("Invalid booking type.");
            }

            var subtotal = basePrice * totalDays;
            var discountAmount = Math.Min(subtotal, Math.Max(0, discount));
            return Math.Max(0, subtotal - discountAmount);
        }
        public async Task<BookingDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id, cancellationToken);
            if (booking == null) return null;
            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<Guid> CreateAsync(CreateBookingRequest request, CancellationToken cancellationToken = default)
        {
            ValidateBookingType(request);
            // Verify cab
            var cab = await _unitOfWork.Cabs.GetByIdAsync(
                request.CabId,
                cancellationToken);

            if (cab == null)
            {
                throw new ResourceNotFoundException(
                    "Cab not found.");
            }

            // Calculate rate based on booking type
            var totalAmount = await GetBookingAmountAsync(request, cancellationToken);

            // Create booking
            var booking = new Booking
            {
                Id = Guid.NewGuid(),

                BookingNo = request.BookingNo,

                CabId = request.CabId,

                BookingType = request.BookingType,

                TransitId = request.TransitId,

                PackageId = request.PackageId,

                Name = request.Name,

                Email = request.Email,

                Phone = request.PhoneNo,

                WhatsApp = request.WhatsApp,

                TravelDate = request.TravelDate,

                ReturnDate = request.ReturnDate,

                PickupLocation = request.PickupLocation,

                DropLocation = request.DropLocation,

                PickupTime = request.PickupTime,

                Passengers = request.Passengers,

                Luggage = request.Luggage,

                SpecialRequirements = request.SpecialRequirements,

                IsConfirmed = false,
                // Server-calculated amount
                TotalAmount = totalAmount
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
            booking.ReturnDate = request.ReturnDate;
            booking.PickupLocation = request.PickupLocation?.Trim();
            booking.DropLocation = request.DropLocation?.Trim();
            booking.PickupTime = request.PickupTime;
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
        private async Task<(decimal BasePrice, decimal Discount)> GetTransitPricingAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            if (!request.TransitId.HasValue)
            {
                throw new ValidationException("Transit is required for a transit booking.");
            }

            var transitRate = await _unitOfWork.TransitRates.GetByCabAndTransitAsync(request.CabId, request.TransitId.Value, cancellationToken);

            if (transitRate == null)
            {
                throw new ResourceNotFoundException("Rate not found for the selected cab and transit.");
            }

            // Get both discounts
            decimal transitDiscount = transitRate.Transit?.Discount ?? 0;
            decimal transitRateDiscount = transitRate.Discount ?? 0;

            // Use the greater discount
            decimal applicableDiscount = Math.Max(transitDiscount, transitRateDiscount);

            return (transitRate.Rate, applicableDiscount);
        }

        private async Task<(decimal BasePrice, decimal Discount)> GetPackagePricingAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            if (!request.PackageId.HasValue)
            {
                throw new ValidationException("PackageId is required for package booking.");
            }

            var packageRate = await _unitOfWork.PackageRates.GetByCabAndPackageAsync(request.CabId, request.PackageId.Value, cancellationToken);

            if (packageRate == null)
            {
                throw new ResourceNotFoundException("No rate found for the selected cab and package.");
            }

            // Get both discounts
            decimal packageDiscount = packageRate.Package?.Discount ?? 0;
            decimal packageRateDiscount = packageRate.Discount ?? 0;

            // Use the greater discount
            decimal applicableDiscount = Math.Max(packageDiscount, packageRateDiscount);

            return (packageRate.Rate, applicableDiscount);
        }

        private async Task<(decimal BasePrice, decimal Discount)> GetCabPricingAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var cab = await _unitOfWork.Cabs.GetByIdAsync(request.CabId, cancellationToken);

            if (cab == null)
            {
                throw new ResourceNotFoundException("Cab not found.");
            }

            var discount = cab.Discount ?? 0;

            return (cab.PricePerDay, discount);
        }

        private async Task<(decimal BasePrice, decimal Discount)> GetSelfDrivePricingAsync(CreateBookingRequest request, CancellationToken cancellationToken)
        {
            var selfDrive = await _unitOfWork.SelfDrives.GetByCabIdAsync(request.CabId, cancellationToken);

            if (selfDrive == null)
            {
                throw new ResourceNotFoundException("Self-drive rate not found for the selected cab.");
            }

            // Get both discounts
            decimal cabDiscount = selfDrive.Cab?.Discount ?? 0;
            decimal selfDriveDiscount = selfDrive.Discount ?? 0;

            // Use the greater discount
            decimal applicableDiscount = Math.Max(selfDriveDiscount, cabDiscount);

            return (selfDrive.PricePerDay, applicableDiscount);
        }

        private void ValidateBookingType(CreateBookingRequest request)
        {
            switch (request.BookingType)
            {
                case BookingType.Transit:

                    if (!request.TransitId.HasValue)
                        throw new ValidationException("TransitId is required for Transit booking.");

                    if (request.PackageId.HasValue)
                        throw new ValidationException("PackageId should not be provided for Transit booking.");

                    break;


                case BookingType.Package:

                    if (!request.PackageId.HasValue)
                        throw new ValidationException("PackageId is required for Package booking.");

                    if (request.TransitId.HasValue)
                        throw new ValidationException("TransitId should not be provided for Package booking.");

                    break;


                case BookingType.Cab:
                case BookingType.SelfDrive:

                    if (request.TransitId.HasValue ||
                        request.PackageId.HasValue)
                    {
                        throw new ValidationException("TransitId and PackageId are not required for this booking type.");
                    }

                    break;

                default:

                    throw new ValidationException("Invalid booking type.");
            }
        }
    }

}
