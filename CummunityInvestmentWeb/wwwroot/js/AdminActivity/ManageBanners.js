var bannerDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/AdminActivity/GetBannerDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[2, "desc"]],
    columns: [
        { "data": "title", "name": "title", "width": "60%" },
        { "data": "sortOrder", "name": "sortOrder", "width": "10%" },
        { "data": "updatedAt", "name": "lastUpdated", "width": "20%", render: formatDate },
        { "width": "10%" }
    ],
    columnDefs: [
        {
            targets: [0],
            orderable: false
        },
        {
            targets: 3,
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "edit": row.bannerId,
                    "delete": row.bannerId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var bannerTbl = $('#banner-table').DataTable($.extend(true, {}, datatableOptions, bannerDtOptions));

tableSearch(bannerTbl);

////-- Add Banner --////
$('#admin-add-banner-btn').on('click', function () {
    GetBannerEdit();
})

////-- Edit Banner --////
$(".admin-table").on('click', '.edit-entry-action-btn', function (e) {
    let bannerId = $(this).val();
    GetBannerEdit(bannerId);
})

function GetBannerEdit(bannerId) {
    $.ajax({
        method: 'GET',
        url: '/Administrator/AdminActivity/GetBannerEdit',
        data: { bannerId: bannerId },
        dataType: 'html',
        success: function (banner) {
            //$("#banner-form #BannerId").val(banner.bannerId);
            //$("#banner-form #Title").val(banner.title);
            //$("#banner-form #SortOrder").val(banner.sortOrder);
            //imagePreview(banner.image);
            //$("#banner-modal .modal-title").text("Edit Banner");
            $("#admin-banner-modal-dialog").html(banner);
            $("#banner-modal").modal("show");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error: " + error);
        }
    })
}

$("#banner-modal").on('change', "#NewBannerImage", function (e) {
    readURL(this);
})

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            imagePreview(e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

function imagePreview(srcUrl) {
    let imagePreviewHtml = `<img src="${srcUrl}" />`
    $("#banner-modal").find(".banner-image-preview-container").html(imagePreviewHtml);
}

//-- On Banner form submit
$("#banner-modal").on('submit', "#banner-form", function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        showBtnLoader($("#save-banner-btn"));
        var bannerFormData = new FormData($(this).get(0));
        $.ajax({
            method: 'POST',
            url: '/Administrator/AdminActivity/AddOrUpdateBanner',
            data: bannerFormData,
            contentType: false,
            processData: false,
            success: function (response) {
                hideBtnLoader($("#save-banner-btn"));
                $("#banner-modal").modal("hide");
                toastr.success(response, "Banner Saved!", toastrOptions_General);
                bannerTbl.ajax.reload(null, false);
            },
            error: function (xhr, status, error) {
                hideBtnLoader($("#save-banner-btn"));
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

////-- Remove Skill --////
$("#confirm-action").on('click', function (e) {
    var bannerId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/AdminActivity/RemoveBanner",
        data: { bannerId: bannerId },
        success: function (response) {
            bannerTbl.ajax.reload(null, false);
            toastr.success('Banner deleted successfully!', null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})