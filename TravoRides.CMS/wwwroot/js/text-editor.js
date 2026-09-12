document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll(".text-editor").forEach(function (element) {

        ClassicEditor
            .create(element)
            .catch(function (error) {
                console.error(error);
            });

    });

});