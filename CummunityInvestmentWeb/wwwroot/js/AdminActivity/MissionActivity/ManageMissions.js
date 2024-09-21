var missionsDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/MissionActivity/GetMissionsDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[1, "desc"]],
    columns: [
        { "data": "title", "name": "title", "autoWidth": true },
        { "data": "missionType", "name": "missionType", "autoWidth": true },
        { "data": "startDate", "name": "startDate", "autoWidth": true, render: formatDate },
        { "data": "endDate", "name": "endDate", "autoWidth": true, render: formatDate },
        { "autoWidth": true }
    ],
    columnDefs: [
        {
            targets: [0],
            orderable: false
        },
        {
            targets: 4, // action column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "editLink": "/Administrator/MissionActivity/EditMission/" + row.missionId,
                    "delete": row.missionId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var missionsTbl = $('#missions-table').DataTable($.extend(true, {}, datatableOptions, missionsDtOptions));

tableSearch(missionsTbl);

////-- Remove Mission --////
$("#confirm-action").on('click', function (e) {
    var missionId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/RemoveMission",
        data: { missionId: missionId },
        success: function (response) {
            missionsTbl.ajax.reload(null, false);
            toastr.success(response, null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Looks like some issue persists. Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})