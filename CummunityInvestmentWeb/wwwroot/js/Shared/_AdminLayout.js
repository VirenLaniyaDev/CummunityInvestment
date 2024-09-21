//--- Toastr
const toastrOptions_General = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "7000",
    "extendedTimeOut": "4000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut",
    "allowHtml": true // enable HTML in the message
}

//--- Side nav toggle
$("#sideNav-toggle-btn").on('click', function () {
    $(".admin-side-navbar-container").toggleClass('shrinked-side-navbar');
});

//--- Clock 
$(document).ready(function () {
    function updateTime() {
        var now = new Date();
        var daysOfWeek = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        var monthsOfYear = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        var dayOfWeek = daysOfWeek[now.getDay()];
        var monthOfYear = monthsOfYear[now.getMonth()];
        var dayOfMonth = now.getDate();
        var year = now.getFullYear();
        var hours = now.getHours();
        var minutes = now.getMinutes();
        var seconds = now.getSeconds();
        var amOrPm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;
        // var currentTime = dayOfWeek + ', ' + monthOfYear + ' ' + dayOfMonth + ', ' + year + ', ' + hours + ':' + minutes + ':' + seconds + ' ' + amOrPm;
        var currentTime = dayOfWeek + ', ' + monthOfYear + ' ' + dayOfMonth + ', ' + year + ', ' + hours + ':' + minutes + ' ' + amOrPm;
        $('#currentDateTime').text(currentTime);
    }
    updateTime();
    setInterval(updateTime, 1000);
});

//--- Show and Hide button loader
function showBtnLoader(btnElement) {
    btnElement.find('.btn-content').addClass('d-none');
    btnElement.find('.btn-loader').removeClass('d-none');
    btnElement.attr('disabled', 'disabled');
}

function hideBtnLoader(btnElement) {
    btnElement.find('.btn-content').removeClass('d-none');
    btnElement.find('.btn-loader').addClass('d-none');
    btnElement.removeAttr('disabled');
}

//--- DataTables common
var datatableOptions = {
    language: {
        "emptyTable": "No record found!",
        //"paginate": {
        //}
        //"processing": "Please wait..."
    },
    pagingType: "full_numbers",
    paging: true,
    //processing: true,
    //responsive: true,
    lengthChange: false,
    info: true,
    drawCallback: function (settings) {
        $('.paginate_button.previous .page-link').html("<i class='bi bi-chevron-left'></i>");
        $('.paginate_button.next .page-link').html("<i class='bi bi-chevron-right'></i>");
        $('.paginate_button.first .page-link').html("<i class='bi bi-chevron-double-left'></i>");
        $('.paginate_button.last .page-link').html("<i class='bi bi-chevron-double-right'></i>");
    },
    initComplete: function () {
        $('.admin-table').parent().parent().addClass('table-responsive');
        $(".dataTables_filter input[type=search]").addClass("input").attr('placeholder', "Search Volunteered Mission");
        $('.dataTables_filter').addClass('text-start').parent().addClass('col-md-12');
        $('.dataTables_filter').find('label').addClass('w-100')
        $(".admin-table-wrapper .pagination").closest('.row').addClass('mt-4');
    },
}

function GetAdminActionsHtml(actionsObj) {
    let actions = "";
    console.log(actionsObj);
    for (const actionKey in actionsObj) {
        console.log(actionKey);
        if (actionKey == "viewLink") {
            actions += `
                    <a class="btn btn-sm primary-btn view-entry-btn" href=${actionsObj[actionKey]}>
                        <small>View</small>
                    </a>
                `
        }
        else if (actionKey == "view") {
            actions += `
                    <button class="btn btn-sm primary-btn view-entry-btn" value=${actionsObj[actionKey]}>
                        <span>View</span>
                    </button>
                `
        }
        else if (actionKey == "edit") {
            actions += `
                    <button class="btn p-1 edit-entry-action-btn" value=${actionsObj[actionKey]}>
                        <i class="bi bi-pencil-square md-icon icon-primary"></i>
                    </button>
                `
        }
        else if (actionKey == "editLink") {
            actions += `
                    <a class="btn p-1 edit-entry-action-btn" href=${actionsObj[actionKey]}>
                        <i class="bi bi-pencil-square md-icon icon-primary"></i>
                    </a>
                `
        }
        else if (actionKey == "approve") {
            actions += `
                    <button class="btn p-1 approve-entry-action-btn" value=${actionsObj[actionKey]}>
                        <i class="bi bi-check-circle lg-icon text-success"></i>
                    </button>
                `
        }
        else if (actionKey == "reject") {
            actions += `
                    <button class="btn p-1 reject-entry-action-btn" value=${actionsObj[actionKey]}>
                        <i class="bi bi-x-circle lg-icon text-danger"></i>
                    </button>
                `
        }
        else if (actionKey == "delete") {
            actions += `
                    <button type="button" class="btn p-1 remove-entry-action-btn" value=${actionsObj[actionKey]} data-bs-toggle="modal" data-bs-target="#confirm-remove-modal">
                        <i class="bi bi-trash md-icon"></i>
                    </button>
                `
        }
    }
    return actions;
}

function GetStatusHtml(status) {
    let statusHtml = "";
    console.log(status)
    if (status == "1" || status == "approved")
        statusHtml += `<span class="text-success">Active</span>`;
    else if (status == "pending")
        statusHtml += `<span class="text-warning">Pending</span>`;
    else if (status == "0" || status == "rejected")
        statusHtml += `<span class="text-danger">In-Active</span>`;
    return statusHtml;
}

//-- Custom search enable with datatables
function tableSearch(dataTable) {
    $('#table-search').on('keyup', function () {
        var searchValue = $(this).val();
        console.log(searchValue);
        dataTable.search(searchValue).draw();
    });
}

function formatDate(data, type, row) {
    if (type === 'display') {
        return moment(data).isValid() ? moment(data).format('DD/MM/YYYY') : "-";
    } else {
        return data;
    }
}

//-- Remove Datatable entry
$(".admin-table").on('click', ".remove-entry-action-btn", function (e) {
    $("#confirm-action").val($(this).val());
})


////-- Previewing Selected image in Admin Avatar --////
$('#admin-profile-modal').on('change', '#NewAdminAvatar', (event) => {
    $('#admin-avatar img').attr('src', URL.createObjectURL(event.target.files[0]));
});

////// User Profile //////
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

$("#admin-profile-form").validate({
    rules: {
        AdminFirstName: {
            required: true,
            maxlength: 128
        },
        AdminLastName: {
            required: true,
            maxlength: 128
        }
    },
    messages: {
        AdminFirstName: {
            required: 'The First Name is required!',
            maxlength: 'First Name should less than 128 characters!'
        },
        AdminLastName: {
            required: 'The Last Name is required!',
            maxlength: 'Last Name should less than 128 characters!'
        }
    },
    highlight: function (element, errorClass, validClass) {
        $(element).addClass("is-invalid");
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).removeClass("is-invalid");
    },
    errorPlacement: function (error, element) {
        error.addClass('invalid-feedback');
        element.find("~.invalid-feedback").text(error.text());
    }
});

$(".ap-cancel-action-btn").click(function (e) {
    $("#admin-profile-form").trigger('reset');
    $('#admin-avatar img').attr('src', $("#admin-avatar").data("currentAvatar"));
})

$("#admin-profile-form").submit(function (e) {
    e.preventDefault();
    const adminProfileForm = $(this);
    if (adminProfileForm.valid()) {
        const adminProfileFormData = new FormData(this);
        showBtnLoader($("#save-admin-profile-btn"));
        $.ajax({
            method: 'POST',
            url: '/Administrator/AdminActivity/SaveProfile',
            data: adminProfileFormData,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log(response);
                hideBtnLoader($("#save-admin-profile-btn"));
                $("#admin-profile-modal").modal('hide');
                toastr.success(null, "Profile Updated!");
                setTimeout(() => {
                    location.reload();
                }, 1000);
            },
            error: function (xhr, status, error) {
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
                hideBtnLoader($("#save-admin-profile-btn"));
                console.log("Ajax error :" + error);
            }
        })
    }
})


// Password visible toggle
const adminPasswordVisibleBtn = $("#admin-password-visible-btn");
adminPasswordVisibleBtn.on('click', function () {
    const passwordField = $("#NewPassword").get(0);
    const passVisibleIcon = adminPasswordVisibleBtn.find('#password-visible-icon');
    const passInvisibleIcon = adminPasswordVisibleBtn.find('#password-invisible-icon');
    passVisibleIcon.toggleClass("d-none");
    passInvisibleIcon.toggleClass("d-none");
    if (passVisibleIcon.hasClass("d-none")) {
        passwordField.type = 'text';
    }
    if (passInvisibleIcon.hasClass("d-none")) {
        passwordField.type = 'password';
    }
})

//// Change/Update Password ////
$(".cancel-change-pass-btn").on('click', function (e) {
    $("#admin-change-password-form").trigger('reset');
    $("#admin-change-password-form input").removeClass('is-invalid');
});

jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

// Define a custom validation method with a regex pattern
$.validator.addMethod("notEqualTo", function (value, element, param) {
    var paramVal = $(param).val();
    return paramVal !== value;
});

$.validator.addMethod("regex", function (value, element, pattern) {
    var regex = new RegExp(pattern);
    return this.optional(element) || regex.test(value);
});

$("#admin-change-password-form").validate({
    rules: {
        OldPassword: {
            required: true,
            maxlength: 128,
        },
        NewPassword: {
            required: true,
            maxlength: 128,
            regex: "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[#$^+=!*()@%&]).{8,}$",
            notEqualTo: "#OldPassword"
        },
        ConfirmPassword: {
            required: true,
            maxlength: 128,
            equalTo: "#NewPassword"
        }
    },
    messages: {
        OldPassword: {
            required: 'Please enter Current Password!',
            maxlength: 'Current Password should less than 128 characters!'
        },
        NewPassword: {
            required: 'Please enter New Password!',
            maxlength: 'New Password should less than 128 characters!',
            regex: "New Password should be 8 character long with, 1 Lower case, 1 Upper case, 1 Digit and 1 Special character",
            notEqualTo: "New Password can not be as Old Password!"
        },
        ConfirmPassword: {
            required: 'Please confirm your New Password!',
            maxlength: 'Confirm Password should less than 128 characters!',
            equalTo: "Passwords does not match!"
        }
    },
    highlight: function (element, errorClass, validClass) {
        $(element).addClass("is-invalid");
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).removeClass("is-invalid");
    },
    errorPlacement: function (error, element) {
        error.addClass('invalid-feedback');
        element.find("~.invalid-feedback").text(error.text());
    }
});

$("#admin-change-password-form").submit(function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        let changePasswordBtn = $("#admin-change-password-btn");
        showBtnLoader(changePasswordBtn);
        var changePasswordData = $(this).serialize();
        console.log(changePasswordData);
        $.ajax({
            method: 'POST',
            url: '/Administrator/AdminActivity/ChangePassword',
            data: changePasswordData,
            success: function (response) {
                console.log(response);
                hideBtnLoader(changePasswordBtn);
                $("#admin-change-password-modal").modal("hide");
                toastr.success('The Password updated successfully!', "Password Updated!", toastrOptions_General);
                $("#admin-change-password-form").trigger('reset');
            },
            error: function (xhr, status, error) {
                console.log(xhr);
                hideBtnLoader(changePasswordBtn);
                switch (xhr.responseJSON.detail) {
                    case "Invalid OldPassword":
                        $("#OldPassword").addClass("is-invalid");
                        $("#OldPassword").find("~.invalid-feedback").text("The Old password is Invalid!");
                        toastr.error('Entered current password is not valid!', 'Invalid Old Password!', toastrOptions_General);
                        break;
                    case "New and Old password Same":
                        $("#NewPassword").addClass("is-invalid");
                        $("#NewPassword").find("~.invalid-feedback").text("New password should not be same as Old password!");
                        break;
                    case "401":
                        toastr.error('You are not allowed to perform this action!', 'Unauthorized!', toastrOptions_General);
                    default:
                        toastr.error('Some error occurs while changing your password!', 'Something went wrong!', toastrOptions_General);
                }
            }
        })
    }
})