using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Authentication;
using TravoRides.Application.Interfaces;
using TravoRides.Domain.Enums;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpVerificationService _otpVerificationService;
        private readonly IForgotPasswordService _forgotPassword;

        public AuthController(IAuthService authService, IForgotPasswordService forgotPassword, IOtpVerificationService otpVerificationService   )
        {
            _authService = authService;
            _forgotPassword = forgotPassword;
            _otpVerificationService = otpVerificationService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = response
            });
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RefreshTokenAsync(request, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Refresh Token Generated Successfully",
                Data = result
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(request.RefreshToken, cancellationToken);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Logged out successfully."
            });
        }
       
        //[HttpPost("send-email-otp")]
        //public async Task<IActionResult> SendEmailOtp([FromBody] SendEmailVerificationRequest request, CancellationToken cancellationToken)
        //{
        //    await _otpVerificationService.SendOtpAsync(request.Email, VerificationOtpPurpose.EmailVerification, cancellationToken);

        //    return Ok(new ApiResponse<object>
        //    {
        //        IsSuccess = true,
        //        Message = "New OTP sent successfully."
        //    });
        //}

        //[HttpPost("verify-email")]
        //public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _otpVerificationService.VerifyOtpAsync(request.Email, request.Otp, VerificationOtpPurpose.EmailVerification, cancellationToken);

        //    return Ok(new ApiResponse<object>
        //    {
        //        IsSuccess = result,
        //        Message = result ? "Email verified successfully." : "Email verification failed.",
        //        Data = result
        //    });
        //}


        [HttpPost("send-forgot-password-otp")]
        public async Task<IActionResult> SendForgotPasswordOtp([FromBody] ForgotPasswordRequest request)
        {
            await _forgotPassword.SendForgotPasswordOtpAsync(request, VerificationOtpPurpose.PasswordReset, CancellationToken.None);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "If an account exists with this email, a password reset OTP has been sent.",
                Data = 60
            });
        }

        [HttpPost("verify-password-reset-otp")]
        public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] VerifyPasswordResetOtpRequest request)
        {
            await _forgotPassword.VerifyResetPasswordOtpAsync(request);

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "OTP verified successfully.",
                Data = request.OTP

            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _forgotPassword.ResetPasswordAsync(request, CancellationToken.None);
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Password reset successfully."
            });
        }

    }
}
