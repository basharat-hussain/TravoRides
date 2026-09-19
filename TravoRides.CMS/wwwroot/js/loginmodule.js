// ////$(document).ajaxSend(function (e, xhr, options) {
// ////    debugger;
// ////    if (options.type.toUpperCase() == "POST") {
// ////        var token = $("input[name='__RequestVerificationToken']").val();
// ////        xhr.setRequestHeader("RequestVerificationToken", token);
// ////    }
// ////});



// function onBegin(xhr) {

//     $("#login-btn").html("<i class='fas fa-spinner fa-spin'></i> Wait...").attr('disabled');
// }
// function onSuccess(response) {
//     debugger;
//     if (response[0] == "True") {
//         toastr.success(response[1]);
//         location.href = "/Home/Index";
//     }
//     else {
//         toastr.error(response[1]);
//     }

//     $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');
// }

// function onFailure(response) {
//     toastr.error('Unknown Server Error');
//     $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');

// }
function onBegin(xhr) {
    $("#login-btn").html("<i class='fas fa-spinner fa-spin'></i> Wait...").attr('disabled', true);
}

function onSuccess(response) {
    if (response[0] == "True") {
        toastr.success(response[1]);

        // Remember the email for next visit, only if the box was checked
        if ($('#remember').is(':checked')) {
            localStorage.setItem('rememberedEmail', $('#Email').val());
        } else {
            localStorage.removeItem('rememberedEmail');
        }

        location.href = "/Home/Index";
    }
    else {
        toastr.error(response[1]);
        $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');
    }
}

function onFailure(response) {
    toastr.error('Unknown Server Error');
    $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');
}

// On page load, pre-fill and pre-check if a value was saved
$(document).ready(function () {
    var savedEmail = localStorage.getItem('rememberedEmail');
    if (savedEmail) {
        $('#Email').val(savedEmail);
        $('#remember').prop('checked', true);
    }
});





