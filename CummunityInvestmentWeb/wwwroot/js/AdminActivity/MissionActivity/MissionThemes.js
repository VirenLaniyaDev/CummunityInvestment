var missionThemesDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/MissionActivity/GetMissionThemesDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[1, "desc"]],
    columns: [
        { "data": "title", "name": "missionTitle", "width": "70%" },
        { "width": "15%" },
        { "width": "15%" }
    ],
    columnDefs: [
        {
            targets: 0,
            orderable: false
        },
        {
            targets: 1,
            data: null,
            render: function (data, type, row, meta) {
                return GetStatusHtml(row.status);
            }
        },
        {
            targets: 2, // status column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "edit": row.missionThemeId,
                    "delete": row.missionThemeId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var missionThemesTbl = $('#mission-themes-table').DataTable($.extend(true, {}, datatableOptions, missionThemesDtOptions));

tableSearch(missionThemesTbl);

////-- Remove Theme --////
$("#confirm-action").on('click', function (e) {
    var missionThemeId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/RemoveMissionTheme",
        data: { missionThemeId: missionThemeId },
        success: function (response) {
            missionThemesTbl.ajax.reload(null, false);
            toastr.success('Mission Theme successfully deleted!', null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})

//-----------------
$("#admin-add-missionTheme-btn").on('click', function () {
    $("#mission-theme-form").trigger('reset');
    $("#mission-theme-form input, #mission-theme-form select").removeClass('is-invalid');
    $("#mission-theme-form #MissionThemeId").val("");
    $("#mission-theme-modal .modal-title").text("Add New Mission Theme");
})
// Get Mission Theme Edit 
$(".admin-table").on('click', '.edit-entry-action-btn', function (e) {
    $("#mission-theme-form input, #mission-theme-form select").removeClass('is-invalid');
    let missionThemeId = $(this).val();
    GetMissionThemeEdit(missionThemeId);
})

$("#mission-theme-form").on('submit', function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        let missionThemeFormData = $(this).serialize();
        $.ajax({
            method: 'POST',
            url: '/Administrator/MissionActivity/AddOrUpdateMissionTheme',
            data: missionThemeFormData,
            success: function (response) {
                console.log(response);
                missionThemesTbl.ajax.reload(null, false);
                toastr.success(response, null, toastrOptions_General);
                $("#mission-theme-modal").modal("hide");
            },
            error: function (xhr, status, error) {
                toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
                console.log("Ajax error: " + error);
            }
        })
    }
})

function GetMissionThemeEdit(missionThemeId) {
    $.ajax({
        method: 'GET',
        url: '/Administrator/MissionActivity/GetMissionThemeEdit',
        data: { missionThemeId: missionThemeId },
        dataType: 'json',
        success: function (missionTheme) {
            console.log(missionTheme);
            $("#mission-theme-form #MissionThemeId").val(missionTheme.missionThemeId);
            $("#mission-theme-form #ThemeTitle").val(missionTheme.title);
            $(`#mission-theme-form #Status option[value="${missionTheme.status}"]`).prop('selected', true);
            $("#mission-theme-modal .modal-title").text("Edit Mission Theme");
            $("#mission-theme-modal").modal("show");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error: " + error);
        }
    })
}
