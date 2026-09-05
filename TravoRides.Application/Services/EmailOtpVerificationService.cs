using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using System.Security.Cryptography;

namespace TravoRides.Application.Services
{
    public class EmailOtpVerificationService : IOtpVerificationService
    {
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailTemplateService _templateService;

        private const int EXPIRATION_MINUTES = 10;
        private readonly IPasswordHasher _passwordHasher;

        public EmailOtpVerificationService(IUnitOfWork unitOfWork, IEmailService emailService, IEmailTemplateService templateService, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _templateService = templateService;
            _passwordHasher = passwordHasher;
            _templateService = templateService;
        }

        public async Task SendOtpAsync(string email, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default)
        {
            if (email == null)
                throw new ValidationException("Email is required.");

            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null)
                throw new AuthenticationException("If this email is registered and eligible for verification, a new OTP has been sent.");

            if (user.IsEmailVerified)
                throw new ConflictException("Email is already verified.");

            var existingOtp = await _unitOfWork.OtpVerifications.GetActiveByUserIdAsync(user.Id, purpose, cancellationToken);

            if (existingOtp != null)
            {
                var secondsSinceCreation = (DateTime.UtcNow - existingOtp.CreatedAt).TotalSeconds;

                if (secondsSinceCreation < 60)
                {
                    throw new RateLimitException("Please wait before requesting another OTP.");
                }
            }

            // Invalidate previous OTP
            await _unitOfWork.OtpVerifications.InvalidateActiveOtpsAsync(user.Id, purpose, cancellationToken);

            // Generate 6-digit OTP
            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            var otpEntity = new VerificationOtp
            {
                UserId = user.Id,
                OTPHash = _passwordHasher.HashPassword(otp),
                ExpiresAt = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTES),
                IsUsed = false,
                AttemptCount = 0,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.OtpVerifications.AddAsync(otpEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var subject = "Verify your TravoRides email";

            var template = await _templateService.GetEmailOTPVerificationTemplateAsync(otp, EXPIRATION_MINUTES);

            await _emailService.SendEmailAsync(user.Email, subject, template, true, cancellationToken);
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default)
        {
            if (email == null)
                throw new ValidationException("Email is required.");
            if (otp == null)
                throw new ValidationException("OTP is required.");

            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null)
                throw new AuthenticationException("Invalid User or OTP.");

            if (user.IsEmailVerified)
                return true;

            var otpEntity = await _unitOfWork.OtpVerifications.GetActiveByUserIdAsync(user.Id, purpose, cancellationToken);

            if (otpEntity == null)
                throw new ResourceNotFoundException("No active OTP found. Please request a new OTP.");

            // Maximum 5 attempts
            if (otpEntity.AttemptCount >= 5)
            {
                otpEntity.IsUsed = true;
                otpEntity.UsedAt = DateTime.UtcNow;

                _unitOfWork.OtpVerifications.Update(otpEntity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new RateLimitException("Maximum OTP verification attempts exceeded. Please request a new OTP.");
            }


            var isValid = _passwordHasher.VerifyPassword(otp, otpEntity.OTPHash);

            if (!isValid)
            {
                otpEntity.AttemptCount++;
                _unitOfWork.OtpVerifications.Update(otpEntity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new AuthenticationException("Invalid OTP. Please try again.");
            }

            // OTP successfully verified
            otpEntity.IsUsed = true;
            otpEntity.UsedAt = DateTime.UtcNow;

            user.IsEmailVerified = true;
            user.EmailVerifiedAt = DateTime.UtcNow;

            _unitOfWork.OtpVerifications.Update(otpEntity);
            _unitOfWork.Users.Update(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
