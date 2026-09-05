using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;
using TravoRides.Domain.Enums;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace TravoRides.Application.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IUserRepository userRepository;
        private readonly IOtpVerificationRepository verificationRepository;
        private readonly IEmailService emailService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailTemplateService templateService;
        private readonly IPasswordHasher passwordHasher;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private const int EXPIRATION_MINUTES = 10;


        public ForgotPasswordService(IUserRepository userRepository, IOtpVerificationRepository verificationRepository, IEmailService emailService, IUnitOfWork unitOfWork, IEmailTemplateService templateService, IPasswordHasher passwordHasher, IRefreshTokenRepository refreshTokenRepository)
        {
            this.userRepository = userRepository;
            this.verificationRepository = verificationRepository;
            this.emailService = emailService;
            this.unitOfWork = unitOfWork;
            this.templateService = templateService;
            this.passwordHasher = passwordHasher;
            this.refreshTokenRepository = refreshTokenRepository;
        }
        public async Task SendForgotPasswordOtpAsync(ForgotPasswordRequest request, VerificationOtpPurpose purpose, CancellationToken cancellationToken = default)
        {
            if (request.Email == null)
                throw new ValidationException("Email is required.");

            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new ResourceNotFoundException("If this email is registered and eligible for verification, a new OTP has been sent.");

            if (!user.IsEmailVerified)
                throw new ValidationException("User is not verified yet.");


            var existingOtp = await verificationRepository.GetActiveByUserIdAsync(user.Id, purpose, cancellationToken);

            if (existingOtp != null)
            {
                var secondsSinceCreation = (DateTime.UtcNow - existingOtp.CreatedAt).TotalSeconds;

                if (secondsSinceCreation < 60)
                {
                    throw new ValidationException("Please wait before requesting another OTP.");
                }
            }

            // Invalidate previous OTP
            await verificationRepository.InvalidateActiveOtpsAsync(user.Id, purpose, cancellationToken);

            // Generate 6-digit OTP
            var otp = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            var otpEntity = new VerificationOtp
            {
                UserId = user.Id,
                OTPHash = passwordHasher.HashPassword(otp),
                ExpiresAt = DateTime.UtcNow.AddMinutes(EXPIRATION_MINUTES),
                IsUsed = false,
                AttemptCount = 0,
                Purpose = purpose,
                CreatedAt = DateTime.UtcNow
            };

            await verificationRepository.AddAsync(otpEntity, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var subject = "Verify your TravoRides email";

            var template = await templateService.GetForgotPasswordOTPTemplateAsync(otp, EXPIRATION_MINUTES);

            await emailService.SendEmailAsync(user.Email, subject, template, true, cancellationToken);
        }

        public async Task VerifyResetPasswordOtpAsync(VerifyPasswordResetOtpRequest request, CancellationToken cancellationToken = default)
        {
            if (request.Email == null)
                throw new ValidationException("Email is required.");
            if (request.OTP == null)
                throw new ValidationException("OTP is required.");

            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
                throw new ResourceNotFoundException("Invalid User or OTP.");

            var otpEntity = await verificationRepository.GetActiveByUserIdAsync(user.Id, VerificationOtpPurpose.PasswordReset, cancellationToken);

            if (otpEntity == null)
                throw new ValidationException("No active OTP found. Please request a new OTP.");

            // Maximum 5 attempts
            if (otpEntity.AttemptCount >= 5)
            {
                otpEntity.IsUsed = true;
                otpEntity.UsedAt = DateTime.UtcNow;

                verificationRepository.Update(otpEntity);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ValidationException("Maximum OTP verification attempts exceeded. Please request a new OTP.");
            }

            otpEntity.AttemptCount++;

            var isValid = passwordHasher.VerifyPassword(request.OTP, otpEntity.OTPHash);

            if (!isValid)
            {
                verificationRepository.Update(otpEntity);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ValidationException("Invalid OTP. Please try again.");
            }

        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {

            if (request.Email == null)
                throw new ValidationException("Email is required.");
            if (request.NewPassword == null)
                throw new ValidationException("New password is required.");
            if (request.ConfirmPassword == null)
                throw new ValidationException("Confirm password is required.");
            if (request.NewPassword != request.ConfirmPassword)
                throw new ValidationException("Passwords do not match.");

            var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new ValidationException("Invalid password reset request.");

            var otp = await verificationRepository.GetActiveByUserIdAsync(user.Id, VerificationOtpPurpose.PasswordReset, cancellationToken);

            if (otp == null)
                throw new ValidationException("OTP is invalid or expired.");


            const int maxAttempts = 5;

            if (otp.AttemptCount >= maxAttempts)
            {
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;

                await unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ValidationException("Maximum OTP attempts exceeded. Please request a new OTP.");

            }

            var isValid = passwordHasher.VerifyPassword(request.OTP, otp.OTPHash);

            if (!isValid)
            {
                otp.AttemptCount++;
                verificationRepository.Update(otp);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                throw new ValidationException("Invalid OTP. Please try again.");
            }

            var hashedPassword = passwordHasher.HashPassword(request.NewPassword);
            user.PasswordHash = hashedPassword;
            userRepository.Update(user);

            otp.IsUsed = true;
            otp.UsedAt = DateTime.UtcNow;

            verificationRepository.Update(otp);

            await verificationRepository.InvalidateActiveOtpsAsync(user.Id, VerificationOtpPurpose.PasswordReset, cancellationToken);

            var refreshToken = await refreshTokenRepository.GetByUserIdAsync(user.Id, cancellationToken);

            if (refreshToken != null)
            {
                refreshToken.IsActive = false;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.IsDeleted = true;
                refreshToken.ExpiresAt = DateTime.UtcNow;
                refreshTokenRepository.Update(refreshToken);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
