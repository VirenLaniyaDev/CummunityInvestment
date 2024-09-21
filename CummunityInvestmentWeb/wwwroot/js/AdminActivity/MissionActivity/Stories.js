var storiesDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/MissionActivity/GetStoriesDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    ordering: false,
    columns: [
        { "data": "storyTitle", "name": "storyTitle", "autoWidth": true },
        { "data": "userName", "name": "userName", "width": "10%" },
        { "data": "missionTitle", "name": "missionTitle", "autoWidth": true },
        { "width": "15%"}
    ],
    columnDefs: [
        {
            targets: 3, // action column index
            data: null,
            render: function (data, type, row, meta) {
                let actionsObj = {
                    "view": row.storyId,
                    "approve": row.storyId,
                    "reject": row.storyId,
                    "delete": row.storyId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};
var storiesTbl = $('#stories-table').DataTable($.extend(true, {}, datatableOptions, storiesDtOptions));

tableSearch(storiesTbl);

////-- Approve/Reject Mission Application --////
$(".admin-table").on('click', ".approve-entry-action-btn", function (e) {
    StoryAction(+$(this).val(), "approve");
});
$(".admin-table").on('click', ".reject-entry-action-btn", function (e) {
    StoryAction(+$(this).val(), "reject");
});

function StoryAction(storyId, storyAction) {
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/StoryAction",
        data: { storyId: storyId, storyAction: storyAction },
        success: function (response) {
            storiesTbl.ajax.reload(null, false);
            toastr.success(response, null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Looks like some issue persists. Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
}

////-- Remove Theme --////
$("#confirm-action").on('click', function (e) {
    var storyId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/RemoveStory",
        data: { storyId: storyId },
        success: function (response) {
            storiesTbl.ajax.reload(null, false);
            toastr.success(response, null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})


$(".admin-table").on('click', ".view-entry-btn", function (e) {
    let storyId = +$(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/GetStoryDetail",
        data: { storyId: storyId },
        success: function (storyDetail) {
            $("#admin-story-detail-modalBody").html(storyDetail);
            $("#admin-story-detail-modal").modal("show");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
});
