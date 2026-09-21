$(document).ready(function () {

    /* =========================================================
       ELEMENTS
    ========================================================= */
    const cabForm = $("#cabForm");
    const transitIdInput = $("#TransitId");
    const editingCabIdInput = $("#EditingCabId");
    const cabIdInput = $("#CabId");
    const cabRateInput = $("#CabRate");
    const cabDiscountInput = $("#CabDiscount");
    const submitBtn = $("#cabSubmitBtn");
    const submitIcon = $("#cabSubmitIcon");
    const submitText = $("#cabSubmitText");
    const cancelBtn = $("#cabCancelBtn");
    const cabTableBody = $("#cabTableBody");

    /* =========================================================
       URLS
    ========================================================= */
    const addUrl = "/Transit/AddCab";
    const updateUrl = "/Transit/UpdateTransitCab";
    const deleteUrl = "/Transit/RemoveCabFromTransit";

    /* =========================================================
       ORIGINAL transit ID

       Captured once on load. The old resetCabForm() wrote the
       literal string "@Model.Updatetransit.Id", because Razor
       does not run inside a static .js file.
    ========================================================= */
    const currentTransitId = transitIdInput.val();

    /* =========================================================
       HELPERS
    ========================================================= */
    function escapeHtml(value) {
        return $("<div>")
            .text(value === undefined || value === null ? "" : value)
            .html();
    }

    /* ---------------------------------------------------------
       BUILD A TABLE ROW

       Mirrors the Razor foreach block in the view exactly:
       same columns, same button markup, same icon classes,
       and Rate / Discount printed raw (no decimal formatting)
       so added rows match server-rendered ones.
    --------------------------------------------------------- */
    function buildCabRow(cab) {
        const discountText =
            (cab.discount === "" || cab.discount === null || cab.discount === undefined)
                ? "-"
                : escapeHtml(cab.discount);

        return `
            <tr id="cab-row-${escapeHtml(cab.id)}">
                <td>
                    ${escapeHtml(cab.name)}
                </td>
                <td>
                    ${escapeHtml(cab.rate)}
                </td>
                <td>
                    ${discountText}
                </td>
                <td>
                    <!-- Edit -->
                    <button type="button"
                            class="btn btn-sm btn-primary edit-cab-btn"
                            data-transit-id="${escapeHtml(currentTransitId)}"
                            data-cab-id="${escapeHtml(cab.id)}"
                            data-rate="${escapeHtml(cab.rate)}"
                            data-discount="${escapeHtml(cab.discount)}">
                        <i class="fas fa-edit"></i>
                    </button>

                    <!-- Delete -->
                    <button type="button"
                            class="btn btn-sm btn-danger delete-cab-btn"
                            data-transit-id="${escapeHtml(currentTransitId)}"
                            data-cab-id="${escapeHtml(cab.id)}">
                        <i class="fas fa-trash"></i>
                    </button>
                </td>
            </tr>
        `;
    }

    /* ---------------------------------------------------------
       INSERT OR REPLACE THE ROW
    --------------------------------------------------------- */
    function upsertCabRow(cab) {
        /* Remove the "no cabs" placeholder if it is present. */
        $("#noCabsRow").remove();

        const rowHtml = buildCabRow(cab);
        const existingRow = $("#cab-row-" + cab.id);

        if (existingRow.length) {
            existingRow.replaceWith(rowHtml);
        } else {
            cabTableBody.append(rowHtml);
        }

        /* Brief highlight so the user sees what changed. */
        const row = $("#cab-row-" + cab.id);
        row.addClass("table-success");
        setTimeout(function () {
            row.removeClass("table-success");
        }, 1500);
    }

    /* ---------------------------------------------------------
       SHOW EMPTY MESSAGE WHEN TABLE IS EMPTY
    --------------------------------------------------------- */
    function showEmptyRowIfNeeded() {
        if (cabTableBody.find("tr").length === 0) {
            cabTableBody.html(`
                <tr id="noCabsRow">
                    <td colspan="4" class="text-center text-muted">
                        No cabs added to this transit.
                    </td>
                </tr>
            `);
        }
    }

    /* ---------------------------------------------------------
       DISABLE / ENABLE A CAB IN THE DROPDOWN

       Stops the same cab being added to the transit twice.
    --------------------------------------------------------- */
    function disableCabOption(cabId, disabled) {
        $("#CabId option[value='" + cabId + "']").prop("disabled", disabled);
    }

    /* =========================================================
       SET ADD / EDIT MODE
    ========================================================= */
    function setCabFormMode(isEditMode) {
        if (isEditMode) {
            submitText.text("Update Cab");
            submitIcon.removeClass("fa-plus fa-spinner fa-spin").addClass("fa-save");
            cancelBtn.show();

            /* While editing, the cab itself should not change. */
            cabIdInput.prop("disabled", true);
        } else {
            submitText.text("Add Cab");
            submitIcon.removeClass("fa-save fa-spinner fa-spin").addClass("fa-plus");
            cancelBtn.hide();
            editingCabIdInput.val("");
            cabIdInput.prop("disabled", false);
        }
    }

    /* =========================================================
       RESET CAB FORM
    ========================================================= */
    function resetCabForm() {
        cabForm[0].reset();
        transitIdInput.val(currentTransitId);
        editingCabIdInput.val("");
        cabForm.attr("action", addUrl);
        setCabFormMode(false);
    }

    /* =========================================================
       ADD / UPDATE CAB
    ========================================================= */
    submitBtn.on("click", function (e) {
        e.preventDefault();
        e.stopPropagation();

        const button = $(this);
        const transitId = transitIdInput.val();
        const editingCabId = editingCabIdInput.val();
        const isEditMode = !!editingCabId;

        /* In edit mode the select is disabled, so take the id
           from the hidden EditingCabId field instead. */
        const cabId = isEditMode ? editingCabId : cabIdInput.val();
        const rate = cabRateInput.val();
        const discount = cabDiscountInput.val();

        /* Name for the table row, read from the dropdown. */
        const cabName = $("#CabId option[value='" + cabId + "']").text().trim();

        /* ---------------------------------------------------------
           VALIDATION
        --------------------------------------------------------- */
        if (!transitId) {
            toastr.error("transit ID is missing.");
            return;
        }
        if (!cabId) {
            toastr.error("A valid cab must be selected.");
            return;
        }
        if (!rate) {
            toastr.error("Please enter the cab price.");
            return;
        }

        /* ---------------------------------------------------------
           URL
        --------------------------------------------------------- */
        const url = isEditMode ? updateUrl : addUrl;

        /* ---------------------------------------------------------
           DATA
        --------------------------------------------------------- */
        const data = {
            transitId: transitId,
            cabId: cabId,
            rate: rate,
            discount: discount
        };

        if (isEditMode) {
            data.editingCabId = editingCabId;
        }

        /* ---------------------------------------------------------
           AJAX
        --------------------------------------------------------- */
        $.ajax({
            url: url,
            type: "POST",
            data: data,

            /* -----------------------------------------------------
               BEFORE SEND
            ----------------------------------------------------- */
            beforeSend: function () {
                button.prop("disabled", true);
                submitIcon.removeClass("fa-plus fa-save").addClass("fa-spinner fa-spin");
                submitText.text(isEditMode ? "Updating..." : "Adding...");
            },

            /* -----------------------------------------------------
               SUCCESS

               Expected response: ["True", "message"]

               If you later change the server to return the saved
               cab, prefer its values over the form values:
                   const cab = response.cab || { id: cabId, ... };
            ----------------------------------------------------- */
            success: function (response) {
                console.log("Add / Update response:", response);

                if (response && response[0] === "True") {
                    toastr.success(
                        response[1] ||
                        (isEditMode ? "Cab updated successfully." : "Cab added successfully.")
                    );

                    /* ---------------------------------------------
                       UPDATE THE TABLE IN PLACE (no reload)
                    --------------------------------------------- */
                    upsertCabRow({
                        id: cabId,
                        name: cabName,
                        rate: rate,
                        discount: discount
                    });

                    /* An added cab should no longer be selectable. */
                    disableCabOption(cabId, true);

                    resetCabForm();
                } else {
                    toastr.error(response?.[1] || "Operation failed.");
                }
            },

            /* -----------------------------------------------------
               ERROR
            ----------------------------------------------------- */
            error: function (xhr) {
                console.log("Request failed.");
                console.log("Status:", xhr.status);
                console.log("Response:", xhr.responseText);
                toastr.error("Something went wrong while saving the cab.");
            },

            /* -----------------------------------------------------
               COMPLETE

               resetCabForm() already restored the label on success;
               this covers the failure paths.
            ----------------------------------------------------- */
            complete: function () {
                button.prop("disabled", false);

                if (editingCabIdInput.val()) {
                    submitText.text("Update Cab");
                    submitIcon.removeClass("fa-spinner fa-spin fa-plus").addClass("fa-save");
                } else {
                    submitText.text("Add Cab");
                    submitIcon.removeClass("fa-spinner fa-spin fa-save").addClass("fa-plus");
                }
            }
        });
    });

    /* =========================================================
       EDIT CAB
    ========================================================= */
    $(document).on("click", ".edit-cab-btn", function () {
        const button = $(this);
        const transitId = button.data("transit-id");
        const cabId = button.data("cab-id");
        const rate = button.data("rate");
        const discount = button.data("discount");

        /* -----------------------------------------------------
           VALIDATION
        ----------------------------------------------------- */
        if (!transitId || !cabId) {
            toastr.error("transit ID or Cab ID is missing.");
            return;
        }

        /* -----------------------------------------------------
           CHECK CAB EXISTS IN DROPDOWN
        ----------------------------------------------------- */
        const cabOption = $("#CabId option[value='" + cabId + "']");
        if (cabOption.length === 0) {
            toastr.error("This cab is not available in the cab dropdown.");
            return;
        }

        /* -----------------------------------------------------
           SET IDS
        ----------------------------------------------------- */
        transitIdInput.val(transitId);
        editingCabIdInput.val(cabId);

        /* -----------------------------------------------------
           SET FIELDS

           The option is disabled (it is already in the transit),
           so re-enable it to make it selectable for display.
        ----------------------------------------------------- */
        cabOption.prop("disabled", false);
        cabIdInput.val(cabId);
        cabRateInput.val(rate);
        cabDiscountInput.val(discount === undefined || discount === null ? "" : discount);

        /* -----------------------------------------------------
           CHANGE ACTION + ENTER EDIT MODE
        ----------------------------------------------------- */
        cabForm.attr("action", updateUrl);
        setCabFormMode(true);

        /* Bring the form into view on small screens. */
        $("html, body").animate({
            scrollTop: cabForm.offset().top - 80
        }, 300);
    });

    /* =========================================================
       CANCEL EDIT
    ========================================================= */
    cancelBtn.on("click", function (e) {
        e.preventDefault();

        const editingCabId = editingCabIdInput.val();
        if (editingCabId) {
            /* Still in the transit, so keep it unselectable. */
            disableCabOption(editingCabId, true);
        }

        resetCabForm();
    });

    /* =========================================================
       DELETE CAB
    ========================================================= */
    $(document).on("click", ".delete-cab-btn", function () {
        const button = $(this);
        const transitId = button.data("transit-id");
        const cabId = button.data("cab-id");

        /* -----------------------------------------------------
           VALIDATION
        ----------------------------------------------------- */
        if (!transitId || !cabId) {
            toastr.error("Transit ID or Cab ID is missing.");
            return;
        }

        /* -----------------------------------------------------
           CONFIRM VIA TOASTR
        ----------------------------------------------------- */
        showDeleteConfirmToast(function () {
            performCabDelete(button, transitId, cabId);
        });
    });

    function showDeleteConfirmToast(onConfirm) {
        toastr.options = {
            closeButton: false,
            tapToDismiss: false,
            timeOut: 0,
            extendedTimeOut: 0,
            positionClass: "toast-top-right"
        };

        const message =
            "Are you sure you want to remove this cab from the transit?" +
            '<div class="mt-2">' +
            '<button type="button" class="btn btn-sm btn-light mr-2 toast-confirm-yes">Yes</button>' +
            '<button type="button" class="btn btn-sm btn-secondary toast-confirm-no">No</button>' +
            "</div>";

        const $toast = toastr.warning(message, "Confirm Removal", { allowHtml: true });

        $toast.find(".toast-confirm-yes").on("click", function () {
            toastr.clear($toast);
            onConfirm();
        });

        $toast.find(".toast-confirm-no").on("click", function () {
            toastr.clear($toast);
        });

        // reset options back to your normal defaults so other toasts aren't affected
        toastr.options = {
            closeButton: true,
            progressBar: true,
            timeOut: 5000
        };
    }

    function performCabDelete(button, transitId, cabId) {
        $.ajax({
            url: deleteUrl,
            type: "POST",
            data: {
                transitId: tra,
                cabId: cabId
            },
            beforeSend: function () {
                button.prop("disabled", true);
            },
            success: function (response) {
                console.log("Delete response:", response);

                if (response && response.isSuccess) {
                    toastr.success(response.message || "Cab removed successfully.");

                    $("#cab-row-" + cabId).remove();
                    disableCabOption(cabId, false);
                    showEmptyRowIfNeeded();

                    if (editingCabIdInput.val() === cabId.toString()) {
                        resetCabForm();
                    }
                } else {
                    toastr.error(response?.message || "Failed to remove cab.");
                    button.prop("disabled", false);
                }
            },
            error: function (xhr) {
                console.log("Delete failed:", xhr.status);
                console.log(xhr.responseText);
                toastr.error("Something went wrong while removing the cab.");
                button.prop("disabled", false);
            }
        });
    }

    /* =========================================================
       ON LOAD: DISABLE ALREADY-ADDED CABS IN THE DROPDOWN
    ========================================================= */
    cabTableBody.find("tr[id^='cab-row-']").each(function () {
        const cabId = this.id.replace("cab-row-", "");
        disableCabOption(cabId, true);
    });
});