// ======================================================
// SEND FORGOT PASSWORD OTP
// ======================================================
function sendForgotPasswordOtp() {

    var email = $("#Email").val().trim();

    if (email === "") {
        toastr.error("Please enter your email address.");
        return;
    }

    $.ajax({
        url: '/Login/SendForgotPasswordOtp',
        type: 'POST',
       
        data: {
            email: email
        },
        beforeSend: function () {
            $("#send-otp-btn").prop("disabled", true);
        },
        success: function (response) {

            if (response.isSuccess) {

                toastr.success(response.message);

                // Show OTP section
                $("#otp-section").show();

                // Optional: hide email section
                $("#email-section").hide();

            }
            else {
                toastr.error(response.message || "Unable to send OTP.");
            }
        },
        error: function (xhr) {

            var message = "Something went wrong while sending OTP.";

            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }

            toastr.error(message);
        },
        complete: function () {
            $("#send-otp-btn").prop("disabled", false);
        }
    });
}


// ======================================================
// VERIFY PASSWORD RESET OTP
// ======================================================
function verifyPasswordResetOtp() {

    var email = $("#Email").val().trim();
    var otp = $("#OTP").val().trim();

    if (email === "") {
        toastr.error("Email address is required.");
        return;
    }

    if (otp === "") {
        toastr.error("Please enter the OTP.");
        return;
    }

    $.ajax({
        url: '/Login/VerifyPasswordResetOtp',
        type: 'POST',
        data: {
            email: email,
            otp: otp
        },
        beforeSend: function () {
            $("#verify-otp-btn").prop("disabled", true);
        },
        success: function (response) {

            if (response.isSuccess) {

                toastr.success(response.message);

                // Show password reset section
                $("#reset-password-section").show();

                // Hide OTP section
                $("#otp-section").hide();

            }
            else {
                toastr.error(response.message || "Invalid OTP.");
            }
        },
        error: function (xhr) {

            var message = "Something went wrong while verifying OTP.";

            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }

            toastr.error(message);
        },
        complete: function () {
            $("#verify-otp-btn").prop("disabled", false);
        }
    });
}


// ======================================================
// RESET PASSWORD
// ======================================================
function resetPassword() {

    var email = $("#Email").val().trim();
    var otp = $("#OTP").val().trim();
    var newPassword = $("#NewPassword").val();
    var confirmPassword = $("#ConfirmPassword").val();

    if (email === "") {
        toastr.error("Email address is required.");
        return;
    }

    if (newPassword === "") {
        toastr.error("Please enter a new password.");
        return;
    }

    if (confirmPassword === "") {
        toastr.error("Please confirm your password.");
        return;
    }

    if (newPassword !== confirmPassword) {
        toastr.error("New password and confirm password do not match.");
        return;
    }

    $.ajax({
        url: '/Login/ResetPassword',
        type: 'POST',
       
        data: {
            email: email,
            otp: otp,
            newPassword: newPassword
        },
        beforeSend: function () {
            $("#reset-password-btn").prop("disabled", true);
        },
        success: function (response) {

            if (response.isSuccess) {

                toastr.success(response.message);

                // Redirect to login after successful reset
                setTimeout(function () {
                    window.location.href = '/Login/Index';
                }, 1500);

            }
            else {
                toastr.error(response.message || "Unable to reset password.");
            }
        },
        error: function (xhr) {

            var message = "Something went wrong while resetting password.";

            if (xhr.responseJSON && xhr.responseJSON.message) {
                message = xhr.responseJSON.message;
            }

            toastr.error(message);
        },
        complete: function () {
            $("#reset-password-btn").prop("disabled", false);
        }
    });
}