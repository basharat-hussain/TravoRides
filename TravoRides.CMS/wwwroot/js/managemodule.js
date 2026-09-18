////$(document).ajaxSend(function (e, xhr, options) {
////    debugger;
////    if (options.type.toUpperCase() == "POST") {
////        var token = $("input[name='__RequestVerificationToken']").val();
////        xhr.setRequestHeader("RequestVerificationToken", token);
////    }
////});

if (typeof module !== "undefined") {
    $(".lbl-module").html(module ? module : "");
}

function onBegin(xhr) {

    $(".save-btn")
        .html("<i class='fas fa-spinner fa-spin'></i> Saving...")
        .prop("disabled", true);

    var isFileValidated = true;

    if (
        module == "Cab" ||
        module == "LatestThinking" ||
        module == "Package" ||
        module == "Transit"
    ) {

        if (
            typeof operation !== "undefined" &&
            operation == "Edit" &&
            $(".img-uploader").val() == ""
        ) {
            isFileValidated = true;
        }

        // Uncomment if you want file validation
        // else {
        //     isFileValidated = fileValidation(
        //         $(".img-uploader"),
        //         ".jpg, .jpeg, .png",
        //         "100",
        //         "60",
        //         "180"
        //     );
        // }
    }

    if (!isFileValidated) {
        $(".save-btn")
            .html("<i class='fas fa-save'></i> Save")
            .prop("disabled", false);
    }

    return isFileValidated;
}


function onSuccess(response) {

    if (response[0] == "True") {

        toastr.success(response[1]);

        $("#addform").trigger("reset");

        $(".image-previewer")
            .attr("src", "/dist/img/200x100.png");
    }
    else {
        toastr.error(response[1]);
    }

    $(".save-btn")
        .html("<i class='fas fa-save'></i> Save")
        .prop("disabled", false);
}


function onFailure(response) {

    toastr.error("Unknown Server Error");

    $(".save-btn")
        .html("<i class='fas fa-save'></i> Save")
        .prop("disabled", false);
}


/* =========================================================
   DELETE
   ========================================================= */

var $rowToDelete = null; // module-level variable

$("#data-grid").on("click", ".btn-delete", function () {
    var parent = $(this).parent().parent();
    var name = parent.find(".name").html();
    var id = parent.find(".hdn-id").val();

    $rowToDelete = parent; // remember it

    $(".lbl").html("<strong>'" + name + "'</strong>");

    $(".delete-btn-confirm")
        .attr("data-ajax-url", "/" + module + "/Delete/" + id)
        .attr("data-ajax-method", "POST");
});

function onDeleteBegin() {

    $(".delete-btn-confirm")
        .html("<i class='fas fa-spinner fa-spin'></i> &nbsp;Deleting...");
}


function onDeleteSuccess(response) {
    $(".delete-btn-confirm").html("<i class='fas fa-check'></i> &nbsp;Yes");
    $("#modal-delete").modal("hide");

    if (response[0] === "True") {
        toastr.success(response[1] || (module + " Deleted Successfully"));
        if ($rowToDelete) {
            $rowToDelete.fadeOut(300, function () {
                $(this).remove();
            });
        }
    } else {
        toastr.error(response[1] || (module + " Deletion Failed!"));
    }
}


function onDeleteFailure(xhr) {
    toastr.error(module + " Deletion Failed!");
    $("#modal-delete").modal("hide");
    $(".delete-btn-confirm").html("<i class='fas fa-check'></i> &nbsp;Yes");
}


/* =========================================================
   GET
   ========================================================= */

function onGetBegin() {

    $(this).html(
        "<i class='fas fa-spinner fa-spin'></i>"
    );
}


function onGetSuccess() {

    if (module == "File") {
        $("#modal-getbyid-file").modal("show");
    }
    else {
        $("#modal-getbyid").modal("show");
    }

    $(this).html("<i class='fas fa-copy'></i>");
}


function onGetFailure() {

    toastr.error("Unknown server error");

    $(this).html("<i class='fas fa-copy'></i>");
}


/* =========================================================
   IMAGE
   ========================================================= */

var imgWidth = 0;
var imgHeight = 0;

$(".img-uploader").change(function (event) {

    var file = event.target.files[0];

    if (!file) {
        return;
    }

    var imageSource = URL.createObjectURL(file);

    $(".image-previewer")
        .attr("src", imageSource);

    var img = new Image();

    img.src = imageSource;

    img.onload = function () {

        imgWidth = this.width;
        imgHeight = this.height;
    };
});


function fileValidation(
    control,
    extensions,
    size,
    height,
    width
) {

    if (control.val() != "" && size != "") {

        var fileExtension =
            control.val().split(".").pop();

        var fileSize =
            control[0].files[0].size / 1024;

        if (!extensions.includes(
            fileExtension.toLowerCase()
        )) {

            toastr.error(
                "Invalid file selected (" +
                fileExtension +
                "), only " +
                extensions.toString() +
                " files allowed"
            );

            return false;
        }

        else if (fileSize > size) {

            toastr.error(
                "Please select a file less than " +
                size +
                " KB"
            );

            return false;
        }

        else {
            return true;
        }
    }

    else {

        toastr.error("Please select a file");

        return false;
    }
}