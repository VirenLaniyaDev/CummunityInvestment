const searchBtn = document.getElementById('search-btn');
const searchFilterNav = document.querySelector('header .search-filter-nav');
searchBtn?.addEventListener('click', (e) => {
    searchFilterNav.classList.toggle('d-none');
    searchBtn.classList.toggle('focused');
});

// Enabling Tooltip
const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl, {
    trigger: 'hover'
}))

// Loader
//$(window).on('load', function () {
//    $("#mission-spinner-wrapper").removeClass("d-none");
//});
//$(document).ready( function () {
//    $("#mission-spinner-wrapper").addClass("d-none");
//});

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

function GoToLogin(returnURL) {
    let url = "/Users/Authentication/Login";
    if (returnURL) {
        const encodedReturnUrl = encodeURIComponent(returnURL);
        url = `${url}?returnUrl=${encodedReturnUrl}`;
    }
    window.location.href = url;
}

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

function textAreaLineLimit(numberOfLines, textAreaEl) {
    textAreaEl.keydown(function (e) {
        countLines = $(this).val().split("\n").length;
        if (e.keyCode == 13 && countLines >= numberOfLines) {
            return false;
        }
    })
}

////// Contact Us //////
jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});

$("#contact-us-form").validate({
    rules: {
        Subject: {
            required: true,
            maxlength: 255
        },
        Message: {
            required: true,
            maxlength: 60000
        }
    },
    messages: {
        Subject: {
            required: 'The Subject is required!',
            maxlength: 'Only 255 character allowed in Subject!'
        },
        Message: {
            required: 'Message Description is required!',
            maxlength: 'Only 60000 character allowed in Message Text field!'
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

$(".cancel-action-btn").click(function (e) {
    $("#contact-us-form").trigger('reset');
})

$("#contact-us-form").submit(function (e) {
    e.preventDefault();
    const contactUsForm = $(this);
    if (contactUsForm.valid()) {
        const contactUsFormData = contactUsForm.serialize();
        showBtnLoader($("#contact-form-submit-btn"));
        $.ajax({
            method: 'POST',
            url: '/Users/Home/ContactUs',
            data: contactUsFormData,
            success: function (response) {
                contactUsForm.trigger('reset');
                hideBtnLoader($("#contact-form-submit-btn"));
                $("#contact-us-modal").modal('hide');
                toastr.success('We received your message please wait for Response!', "Message Sent!", toastrOptions_General);
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
                }
                else if (xhr.status == 401) {
                    toastr.error('You are not authorize to perform this action!', "Unauthorize access!", toastrOptions_General);
                } else {
                    toastr.error('Something unexpected error occured!', 'Something went wrong!', toastrOptions_General);
                }
                hideBtnLoader($("#contact-form-submit-btn"));
                console.log("Ajax error :" + error);
            }
        })
    }
})