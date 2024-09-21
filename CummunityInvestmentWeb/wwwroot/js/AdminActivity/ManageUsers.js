var usersDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/AdminActivity/GetUsersDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[5, "desc"]],
    columns: [
        { "data": "firstName", "name": "firstName", "autoWidth": true },
        { "data": "lastName", "name": "lastName", "autoWidth": true },
        { "data": "email", "name": "email", "autoWidth": true },
        { "data": "employeeId", "name": "employeeId", "autoWidth": true },
        { "data": "department", "name": "department", "autoWidth": true },
        { "data": "status", "name": "status", "autoWidth": true },
        { "autoWidth": true }
    ],
    columnDefs: [
        {
            targets: [0, 1, 2, 3],
            orderable: false
        },
        {
            targets: 5,
            data: null,
            render: function (data, type, row, meta) {
                return GetStatusHtml(row.status);
            }
        },
        {
            targets: 6, // status column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "edit": row.userId,
                    "delete": row.userId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var usersTbl = $('#users-table').DataTable($.extend(true, {}, datatableOptions, usersDtOptions));

tableSearch(usersTbl);

//-----------------
function GetAddOrUpdateUserFormModal(userId) {
    $.ajax({
        method: 'GET',
        url: '/Administrator/AdminActivity/GetUserEdit',
        data: { userId: userId },
        dataType: 'html',
        success: function (response) {
            $("#admin-addOrUpdate-user-modal-dialog").html(response);
            $("#admin-addOrUpdate-user-modal").modal("show");
        },
        error: function (xhr, status, error) {
            console.log("Ajax error: " + error);
        }
    })
}

////-- Add new user --////
$("#admin-add-user-btn").on('click', function (e) {
    GetAddOrUpdateUserFormModal();
    $("#admin-addOrUpdate-user-modal").modal("show");
})

////-- Edit User Profile --////
$(".admin-table").on('click', '.edit-entry-action-btn', function (e) {
    let userId = $(this).val();
    GetAddOrUpdateUserFormModal(userId);
})

////-- Remove User --////
$("#confirm-action").on('click', function (e) {
    var userId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/AdminActivity/RemoveUser",
        data: { userId: userId },
        success: function (response) {
            usersTbl.ajax.reload(null, false);
            toastr.success('User successfully deleted!', null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})

$(".admin-table").on('click', ".remove-entry-action-btn", function (e) {
    $("#confirm-action").val($(this).val());
})

////-- Previewing Selected image --////
$('#admin-addOrUpdate-user-modal').on('change', '#NewUserAvatar', (event) => {
    $('#up-user-avatar img').attr('src', URL.createObjectURL(event.target.files[0]));
});

////-- Password visible toggle --////
$("#admin-addOrUpdate-user-modal").on('click', '#password-visible-btn', function () {
    const passwordField = $("#Password").get(0);
    const passVisibleIcon = $(this).find('#password-visible-icon');
    const passInvisibleIcon = $(this).find('#password-invisible-icon');
    passVisibleIcon.toggleClass("d-none");
    passInvisibleIcon.toggleClass("d-none");
    if (passVisibleIcon.hasClass("d-none")) {
        passwordField.type = 'text';
    }
    if (passInvisibleIcon.hasClass("d-none")) {
        passwordField.type = 'password';
    }
})

////-- Getting Cities based on Country selected --////
$("#admin-addOrUpdate-user-modal").on('change', '#CountryId', function (e) {
    SetCountryCities(+$(this).val());
})

function SetCountryCities(countryId, cityVal) {
    const CitySelect = $('.up-address-information-container #CityId');
    if (countryId) {
        $.ajax({
            method: 'GET',
            url: '/Users/UserActivity/GetCitiesByCountryId',
            data: { countryId: countryId },
            dataType: 'json',
            async: false,
            success: function (cities) {
                CitySelect.empty();
                CitySelect.append(`<option value="">-- Select your city --</option>`);
                $.each(cities, function (index, city) {
                    CitySelect.append(`<option value="${city.cityId}">${city.name}</option>`);
                });
                if (cityVal)
                    CitySelect.val(cityVal);
            },
            error: function (xhr, status, error) {
                console.log("Ajax error: " + error);
            }
        })
    } else {
        CitySelect.empty();
    }
}

////-- Submit user add or update form --////
$("#admin-addOrUpdate-user-modal").on('submit', "#addOrUpdate-user-form", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        showBtnLoader($("#user-save-btn"));
        var userFormData = new FormData($(this).get(0));
        $.ajax({
            method: 'POST',
            url: '/Administrator/AdminActivity/AddOrUpdateUser',
            data: userFormData,
            contentType: false,
            processData: false,
            success: function (response) {
                hideBtnLoader($("#user-save-btn"));
                $("#admin-addOrUpdate-user-modal").modal("hide");
                toastr.success(response, "User Saved!", toastrOptions_General);
                usersTbl.ajax.reload(null, false);
            },
            error: function (xhr, status, error) {
                hideBtnLoader($("#user-save-btn"));
                if (xhr.responseJSON.errors && xhr.responseJSON.status == 400) {
                    let modelStateErrors = xhr.responseJSON.errors;
                    for (var key in modelStateErrors) {
                        if (modelStateErrors.hasOwnProperty(key)) {
                            var errorMessages = modelStateErrors[key];
                            var errorMessage = errorMessages[0]; // Take the first error message in the array
                            var inputElement = $('input[name="' + key + '"], textarea[name="' + key + '"]');
                            var invalidFeedback = inputElement.siblings('.invalid-feedback');

                            inputElement.addClass('is-invalid');
                            invalidFeedback.html(errorMessage);
                        }
                    }
                    console.log(modelStateErrors);
                }
                else if (xhr.status == 401) {
                    toastr.error('You are not authorize to perform this action!', "Unauthorize access!", toastrOptions_General);
                } else {
                    toastr.error('Something unexpected error occured!', 'Something went wrong!', toastrOptions_General);
                }
                console.log("Ajax error: " + error);
            }
        })
    }
})