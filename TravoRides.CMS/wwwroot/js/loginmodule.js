////$(document).ajaxSend(function (e, xhr, options) {
////    debugger;
////    if (options.type.toUpperCase() == "POST") {
////        var token = $("input[name='__RequestVerificationToken']").val();
////        xhr.setRequestHeader("RequestVerificationToken", token);
////    }
////});



function onBegin(xhr) {

    $("#login-btn").html("<i class='fas fa-spinner fa-spin'></i> Wait...").attr('disabled');
}
function onSuccess(response) {
    debugger;
    if (response[0] == "True") {
        toastr.success(response[1]);
        location.href = "/Home/Index";
    }
    else {
        toastr.error(response[1]);
    }

    $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');
}

function onFailure(response) {
    toastr.error('Unknown Server Error');
    $("#login-btn").html("<i class='fas fa-sign-in-alt mr-2'></i> Login").removeAttr('disabled');

}






