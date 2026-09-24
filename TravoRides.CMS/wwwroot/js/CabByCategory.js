$(document).ready(function () {

    // ============================
    // Load Cabs by Category
    // ============================
    function loadCabsByCategory(categoryId, selectedCabId = '') {

        const cabDropdown = $('#CabId');

        cabDropdown.empty();
        cabDropdown.append(
            '<option value="">-- Select Cab --</option>'
        );

        if (!categoryId) {
            cabDropdown.prop('disabled', true);
            return;
        }

        cabDropdown.prop('disabled', true);

        $.ajax({
            url: '/SelfDrive/GetCabsByCategory',
            type: 'GET',
            data: {
                categoryId: categoryId
            },

            success: function (response) {

                if (response.isSuccess && response.data && response.data.length > 0) {

                    $.each(response.data, function (index, cab) {

                        const option = $('<option>', {
                            value: cab.id,
                            text: cab.name
                        });

                        // Select existing Cab while editing
                        if (
                            selectedCabId &&
                            cab.id.toString().toLowerCase() ===
                            selectedCabId.toString().toLowerCase()
                        ) {
                            option.prop('selected', true);
                        }

                        cabDropdown.append(option);
                    });

                    cabDropdown.prop('disabled', false);
                }
                else {

                    cabDropdown.append(
                        '<option value="">No cabs available</option>'
                    );

                    cabDropdown.prop('disabled', true);
                }
            },

            error: function () {

                toastr.error('Unable to load cabs.');

                cabDropdown.empty();
                cabDropdown.append(
                    '<option value="">-- Select Cab --</option>'
                );

                cabDropdown.prop('disabled', true);
            }
        });
    }


    // ============================
    // Category Changed
    // ============================
    $('#CategoryId').on('change', function () {

        const categoryId = $(this).val();

        // Category changed by user,
        // therefore old Cab must be cleared.
        loadCabsByCategory(categoryId);
    });


    // ============================
    // Edit Page Initial Load
    // ============================
    const existingCategoryId = $('#CategoryId').val();
    const existingCabId = $('#CabId').data('selected-cab');

    if (existingCategoryId) {

        loadCabsByCategory(
            existingCategoryId,
            existingCabId
        );
    }
});