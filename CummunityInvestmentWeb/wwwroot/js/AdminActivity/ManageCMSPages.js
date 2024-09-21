var cmsPagesDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/AdminActivity/GetCMSDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[1, "desc"]],
    columns: [
        { "data": "title", "name": "title", "width": "70%"},
        { "data": "status", "name": "status", "width": "10%" },
        { "width": "20%" }
    ],
    columnDefs: [
        {
            targets: [0],
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
                    "editLink": "/Administrator/AdminActivity/EditCMSPage/"+row.cmsPageId,
                    "delete": row.cmsPageId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var cmsTbl = $('#cms-pages-table').DataTable($.extend(true, {}, datatableOptions, cmsPagesDtOptions));

tableSearch(cmsTbl);

////-- Remove CMS Page --////
$("#confirm-action").on('click', function (e) {
    var cmsPageId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/AdminActivity/RemoveCMSPage",
        data: { cmsPageId: cmsPageId },
        success: function (response) {
            cmsTbl.ajax.reload(null, false);
            toastr.success(response, null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})