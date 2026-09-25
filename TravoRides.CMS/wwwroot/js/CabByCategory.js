$(document).ready(function () {

    function loadCabsByCategory(categoryId, selectedCabId = '') {

        const cabDropdown = $('#CabId');

        cabDropdown.empty().append(
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

                if (response.isSuccess &&
                    response.data &&
                    response.data.length > 0) {

                    $.each(response.data, function (index, cab) {

                        const option = $('<option>', {
                            value: cab.id,
                            text: cab.name
                        });

                        if (
                            selectedCabId &&
                            String(cab.id).toLowerCase() ===
                            String(selectedCabId).toLowerCase()
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

                cabDropdown.empty().append(
                    '<option value="">-- Select Cab --</option>'
                );

                cabDropdown.prop('disabled', true);
            }
        });
    }


    // Category changed
    $('#CategoryId').on('change', function () {

        const categoryId = $(this).val();

        // Clear previously selected cab
        loadCabsByCategory(categoryId, '');
    });


    // Edit page initial load
    const existingCategoryId = $('#CategoryId').val();
    const existingCabId = $('#CabId').attr('data-selected-cab');

    if (existingCategoryId) {

        loadCabsByCategory(
            existingCategoryId,
            existingCabId
        );
    }
    else {
        $('#CabId').prop('disabled', true);
    }

});
