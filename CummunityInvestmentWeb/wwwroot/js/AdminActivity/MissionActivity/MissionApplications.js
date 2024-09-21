var missionApplicationsDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/MissionActivity/GetMissionApplicationsDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[4, "asc"]],
    columns: [
        { "data": "missionTitle", "name": "missionTitle", "autoWidth": true },
        { "data": "missionId", "name": "missionId", "autoWidth": true },
        { "data": "userId", "name": "userId", "autoWidth": true },
        { "data": "userName", "name": "userName", "autoWidth": true },
        { "data": "appliedAt", "name": "appliedAt", "autoWidth": true, render: formatDate },
        { "autoWidth": true }
    ],
    columnDefs: [
        {
            targets: [0, 1, 2, 3],
            orderable: false
        },
        {
            targets: 5, // action column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "approve": row.missionApplicationId,
                    "reject": row.missionApplicationId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};
var missionApplicationsTbl = $('#mission-applications-table').DataTable($.extend(true, {}, datatableOptions, missionApplicationsDtOptions));

tableSearch(missionApplicationsTbl);

////-- Approve/Reject Mission Application --////
$(".admin-table").on('click', ".approve-entry-action-btn", function (e) {
    MissionApplicationAction(+$(this).val(), "approve");
});
$(".admin-table").on('click', ".reject-entry-action-btn", function (e) {
    MissionApplicationAction(+$(this).val(), "reject");
});

function MissionApplicationAction(missionApplicationId, MAaction) {
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/MissionApplicationAction",
        data: { missionApplicationId: missionApplicationId, MAaction: MAaction },
        success: function (response) {
            missionApplicationsTbl.ajax.reload(null, false);
            toastr.success(response, null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Looks like some issue persists. Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
}