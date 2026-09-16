<script>

    $(document).ready(function () {

        // =========================================================
        // ADD / UPDATE CAB
        // =========================================================

        $("#cabSubmitBtn").click(function () {

            var packageId = $("#PackageId").val();
            var editingCabId = $("#EditingCabId").val();

            var cabId = $("#CabId").val();
            var rate = $("#CabRate").val();
            var discount = $("#CabDiscount").val();

            var request = {
                cabId: editingCabId || cabId,
                rate: rate,
                discount: discount || null
            };


            // =====================================================
            // UPDATE EXISTING CAB
            // =====================================================

            if (editingCabId) {

                $.ajax({
                    url: '/Package/UpdateCab/' + packageId,
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(request),

                    success: function (response) {

                        if (response[0] !== "True") {
                            return;
                        }

                        // Update existing table row
                        var row = $("#cab-row-" + editingCabId);

                        row.find("td:eq(1)").text(rate);

                        row.find("td:eq(2)")
                            .text(discount || "-");

                        // Update button values
                        var editButton =
                            row.find(".edit-cab-btn");

                        editButton.attr("data-rate", rate);
                        editButton.attr(
                            "data-discount",
                            discount
                        );

                        resetCabForm();
                    }
                });

                return;
            }


            // =====================================================
            // ADD NEW CAB
            // =====================================================

            var cabName =
                $("#CabId option:selected").data("cab-name");

            $.ajax({
                url: '/Package/AddCab/' + packageId,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(request),

                success: function (response) {

                    if (response[0] !== "True") {
                        return;
                    }

                    // Remove "No cabs" row
                    $("#noCabsRow").remove();

                    var discountText = discount || "-";

                    // Add row to table
                    var row = `
                    <tr id="cab-row-${cabId}">

                        <td>
                            ${cabName}
                        </td>

                        <td>
                            ${rate}
                        </td>

                        <td>
                            ${discountText}
                        </td>

                        <td>

                            <button type="button"
                                    class="btn btn-sm btn-primary edit-cab-btn"
                                    data-cab-id="${cabId}"
                                    data-cab-name="${cabName}"
                                    data-rate="${rate}"
                                    data-discount="${discount}">

                                <i class="fas fa-edit"></i>

                            </button>

                            <button type="button"
                                    class="btn btn-sm btn-danger delete-cab-btn"
                                    data-cab-id="${cabId}">

                                <i class="fas fa-trash"></i>

                            </button>

                        </td>

                    </tr>
                `;

                    $("#cabTableBody").append(row);

                    // Remove from available-cab dropdown
                    $("#CabId option[value='" + cabId + "']")
                        .remove();

                    resetCabForm();
                }
            });

        });


    // =========================================================
    // EDIT CAB
    // =========================================================

    $(document).on(
    "click",
    ".edit-cab-btn",
    function () {

            var cabId = $(this).data("cab-id");
    var cabName = $(this).data("cab-name");
    var rate = $(this).data("rate");
    var discount = $(this).data("discount");


    // Store editing cab
    $("#EditingCabId").val(cabId);


    // Existing cab is normally not in AvailableCabs,
    // so temporarily add it to the dropdown.
    if ($("#CabId option[value='" + cabId + "']").length === 0) {

        $("#CabId").append(
            $("<option>", {
                value: cabId,
                text: cabName
            })
        );
            }


    // Select cab
    $("#CabId").val(cabId);


    // Fill values
    $("#CabRate").val(rate);
    $("#CabDiscount").val(discount ?? "");


    // Change UI to Update mode
    $("#cabSectionTitle")
    .text("Update Cab");

    $("#cabSubmitText")
    .text("Update Cab");

    $("#cabSubmitBtn")
    .removeClass("btn-success")
    .addClass("btn-primary");

    $("#cabCancelBtn").show();
        }
    );


    // =========================================================
    // CANCEL UPDATE
    // =========================================================

    $("#cabCancelBtn").click(function () {

        resetCabForm();

    });


    // =========================================================
    // RESET FORM
    // =========================================================

    function resetCabForm() {

        var editingCabId =
    $("#EditingCabId").val();

    // If we were editing, remove the temporarily
    // added cab from dropdown.
    if (editingCabId) {

        $("#CabId option[value='" + editingCabId + "']")
            .remove();
        }


    $("#EditingCabId").val("");

    $("#CabId").val("");

    $("#CabRate").val("");

    $("#CabDiscount").val("");


    $("#cabSectionTitle")
    .text("Add Cab to Package");

    $("#cabSubmitText")
    .text("Add Cab");

    $("#cabSubmitBtn")
    .removeClass("btn-primary")
    .addClass("btn-success");

    $("#cabCancelBtn")
    .hide();
    }

});

</script>



// <script>

//     $(document).ready(function () {

//         // =========================================================
//         // ADD / UPDATE CAB BUTTON
//         // =========================================================

//         $("#cabSubmitBtn").click(function () {

//             var packageId = $("#PackageId").val();
//             var cabId = $("#CabId").val();
//             var rate = $("#CabRate").val();
//             var discount = $("#CabDiscount").val();

//             // -----------------------------------------------------
//             // Validation
//             // -----------------------------------------------------

//             if (!cabId) {
//                 alert("Please select a cab.");
//                 return;
//             }

//             if (!rate) {
//                 alert("Please enter the cab price.");
//                 return;
//             }

//             // -----------------------------------------------------
//             // Check whether we are editing
//             // -----------------------------------------------------

//             var editingCabId = $("#EditingCabId").val();

//             // =====================================================
//             // UPDATE EXISTING CAB
//             // =====================================================

//             if (editingCabId) {

//                 var updateRequest = {
//                     cabId: editingCabId,
//                     rate: parseFloat(rate),
//                     discount: discount === ""
//                         ? null
//                         : parseFloat(discount)
//                 };

//                 $.ajax({

//                     url: '/Package/UpdateCab/' + packageId,

//                     type: 'POST',

//                     contentType: 'application/json',

//                     data: JSON.stringify(updateRequest),

//                     success: function (response) {

//                         if (response[0] !== "True") {

//                             alert(response[1]);

//                             return;
//                         }

//                         // -----------------------------------------
//                         // Update table row
//                         // -----------------------------------------

//                         var row =
//                             $("#cab-row-" + editingCabId);

//                         row.find("td:eq(1)")
//                             .text(rate);

//                         row.find("td:eq(2)")
//                             .text(
//                                 discount === ""
//                                     ? "-"
//                                     : discount
//                             );

//                         // -----------------------------------------
//                         // Update edit button data
//                         // -----------------------------------------

//                         var editButton =
//                             row.find(".edit-cab-btn");

//                         editButton.attr(
//                             "data-rate",
//                             rate
//                         );

//                         editButton.attr(
//                             "data-discount",
//                             discount
//                         );

//                         // -----------------------------------------
//                         // Reset form
//                         // -----------------------------------------

//                         resetCabForm();

//                         alert("Cab updated successfully.");
//                     },

//                     error: function (xhr) {

//                         console.log(xhr);

//                         alert("Error while updating cab.");
//                     }
//                 });

//                 return;
//             }


//             // =====================================================
//             // ADD NEW CAB
//             // =====================================================

//             var cabName =
//                 $("#CabId option:selected")
//                     .data("cab-name");

//             var addRequest = {

//                 cabId: cabId,

//                 rate: parseFloat(rate),

//                 discount: discount === ""
//                     ? null
//                     : parseFloat(discount)
//             };


//             $.ajax({

//                 url: '/Package/AddCab/' + packageId,

//                 type: 'POST',

//                 contentType: 'application/json',

//                 data: JSON.stringify(addRequest),

//                 success: function (response) {

//                     if (response[0] !== "True") {

//                         alert(response[1]);

//                         return;
//                     }

//                     // ---------------------------------------------
//                     // Remove "No cabs" row
//                     // ---------------------------------------------

//                     $("#noCabsRow").remove();


//                     // ---------------------------------------------
//                     // Discount display
//                     // ---------------------------------------------

//                     var discountText =
//                         discount === ""
//                             ? "-"
//                             : discount;


//                     // ---------------------------------------------
//                     // Create new table row
//                     // ---------------------------------------------

//                     var row = `
//                     <tr id="cab-row-${cabId}">

//                         <td>
//                             ${cabName}
//                         </td>

//                         <td>
//                             ${rate}
//                         </td>

//                         <td>
//                             ${discountText}
//                         </td>

//                         <td>

//                             <button type="button"
//                                     class="btn btn-sm btn-primary edit-cab-btn"
//                                     data-cab-id="${cabId}"
//                                     data-cab-name="${cabName}"
//                                     data-rate="${rate}"
//                                     data-discount="${discount}">

//                                 <i class="fas fa-edit"></i>

//                             </button>

//                             <button type="button"
//                                     class="btn btn-sm btn-danger delete-cab-btn"
//                                     data-cab-id="${cabId}">

//                                 <i class="fas fa-trash"></i>

//                             </button>

//                         </td>

//                     </tr>
//                 `;


//                     $("#cabTableBody").append(row);


//                     // ---------------------------------------------
//                     // Remove cab from available dropdown
//                     // ---------------------------------------------

//                     $("#CabId option[value='" + cabId + "']")
//                         .remove();


//                     // ---------------------------------------------
//                     // Reset form
//                     // ---------------------------------------------

//                     resetCabForm();

//                 },

//                 error: function (xhr) {

//                     console.log(xhr);

//                     alert("Error while adding cab.");
//                 }

//             });

//         });


//     // =========================================================
//     // EDIT CAB BUTTON
//     // =========================================================

//     $(document).on(
//     "click",
//     ".edit-cab-btn",
//     function () {

//             var cabId =
//     $(this).data("cab-id");

//     var cabName =
//     $(this).data("cab-name");

//     var rate =
//     $(this).data("rate");

//     var discount =
//     $(this).data("discount");


//     // -------------------------------------------------
//     // Store the cab currently being edited
//     // -------------------------------------------------

//     $("#EditingCabId")
//     .val(cabId);


//     // -------------------------------------------------
//     // Put cab into dropdown
//     //
//     // Normally it is not there because AvailableCabs
//     // contains only unassigned cabs.
//     // -------------------------------------------------

//     var existingOption =
//     $("#CabId option[value='" + cabId + "']");


//     if (existingOption.length === 0) {

//         $("#CabId").append(
//             $("<option>", {
//                 value: cabId,
//                 text: cabName
//             })
//         );

//             }


//     // -------------------------------------------------
//     // Select cab
//     // -------------------------------------------------

//     $("#CabId")
//     .val(cabId);


//     // -------------------------------------------------
//     // Fill Price
//     // -------------------------------------------------

//     $("#CabRate")
//     .val(rate);


//     // -------------------------------------------------
//     // Fill Discount
//     // -------------------------------------------------

//     if (
//     discount === undefined ||
//     discount === null ||
//     discount === ""
//     ) {

//         $("#CabDiscount").val("");

//             }
//     else {

//         $("#CabDiscount")
//             .val(discount);

//             }


//     // -------------------------------------------------
//     // Change heading
//     // -------------------------------------------------

//     $("#cabSectionTitle")
//     .text("Update Cab");


//     // -------------------------------------------------
//     // Change button
//     // -------------------------------------------------

//     $("#cabSubmitText")
//     .text("Update Cab");


//     $("#cabSubmitBtn")
//     .removeClass("btn-success")
//     .addClass("btn-primary");


//     // -------------------------------------------------
//     // Show Cancel
//     // -------------------------------------------------

//     $("#cabCancelBtn")
//     .show();

//         }
//     );


//     // =========================================================
//     // CANCEL EDIT
//     // =========================================================

//     $("#cabCancelBtn").click(function () {

//         resetCabForm();

//     });


//     // =========================================================
//     // RESET FORM
//     // =========================================================

//     function resetCabForm() {

//         $("#EditingCabId").val("");

//     $("#CabId").val("");

//     $("#CabRate").val("");

//     $("#CabDiscount").val("");


//     $("#cabSectionTitle")
//     .text("Add Cab to Package");


//     $("#cabSubmitText")
//     .text("Add Cab");


//     $("#cabSubmitBtn")
//     .removeClass("btn-primary")
//     .addClass("btn-success");


//     $("#cabCancelBtn")
//     .hide();

//     }

// });

// </script>