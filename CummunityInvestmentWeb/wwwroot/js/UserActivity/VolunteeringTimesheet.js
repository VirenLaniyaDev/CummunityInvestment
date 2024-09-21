var datatableOptions = {
    language: {
        "emptyTable": "No Timesheet records found!",
        //"paginate": {
        //}
    },
    pagingType: "full_numbers",
    paging: true,
    order: [[1, "desc"]],
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
        $('#timeBasedTimesheet-table, #goalBasedTimesheet-table').parent().parent().addClass('table-responsive');
        $(".dataTables_filter input[type=search]").addClass("input").attr('placeholder', "Search Volunteered Mission");
        $('.dataTables_filter').addClass('text-start').parent().addClass('col-md-12');
        $('.dataTables_filter').find('label').addClass('w-100')
        $(".timesheet-table-wrapper .pagination").addClass('pagination-sm column-gap-1 mt-2');
    },
}

//--- Enabling Datatables
var timeBasedTimesheetDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Users/UserActivity/GetUserTimesheets",
        "type": "POST",
        "dataType": "json",
        "data": {
            MissionType: "time"
        },
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    columns: [
        { "data": "missionTitle", "name": "missionTitle", "autoWidth": true },
        { "data": "dateVolunteered", "name": "dateVolunteered", "autoWidth": true, render: formatDate },
        { "data": "timespanHours", "name": "timespanHours", "autoWidth": true },
        { "data": "timespanMinutes", "name": "timespanMinutes", "autoWidth": true },
        { "width": "65px" }
    ],
    columnDefs: [
        {
            targets: 0,
            orderable: false
        },
        {
            targets: 4, // status column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                console.log(row.isEditable);
                let actions = GetTimesheetActionsHtml(row.timesheetId, "time", row.isEditable);
                return actions;
            }
        }
    ]
};

var goalBasedTimesheetDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Users/UserActivity/GetUserTimesheets",
        "type": "POST",
        "dataType": "json",
        "data": {
            MissionType: "goal"
        },
        "error": function (xhr, status, error) {
            console.log("Ajax error : " + error);
        }
    },
    columns: [
        { "data": "missionTitle", "name": "missionTitle", "autoWidth": true },
        { "data": "dateVolunteered", "name": "dateVolunteered", "autoWidth": true, render: formatDate },
        { "data": "action", "name": "action", "autoWidth": true },
        { "width": "65px" }
    ],
    columnDefs: [
        {
            targets: 0,
            orderable: false
        },
        {
            targets: 3, // status column index
            data: null,
            orderable: false,
            render: function (data, type, row, meta) {
                console.log(row.isEditable);
                let actions = GetTimesheetActionsHtml(row.timesheetId, "goal", row.isEditable);
                return actions;
            }
        }
    ]
};

function GetTimesheetActionsHtml(timesheetId, missionType, isEditable) {
    let actions;
    if (isEditable) {
        actions = `
                <button class="btn p-1 edit-timesheet-entry-btn" value=${timesheetId} data-mission-type=${missionType}>
                    <i class="bi bi-pencil-square rg-icon icon-primary"></i>
                </button>
                <button type="button" class="btn p-1 remove-timesheet-entry-btn" value=${timesheetId} data-mission-type=${missionType} data-bs-toggle="modal" data-bs-target="#confirm-remove-modal">
                    <i class="bi bi-trash rg-icon"></i>
                </button>
            `
    } else {
        actions = `
                <button class="btn btn-sm py-0 primary-btn edit-timesheet-entry-btn" value=${timesheetId} data-mission-type=${missionType}>
                    <small>
                    <i class="bi bi-eye"></i>
                    <span>View</span>
                    </small>
                </button>
                `
    }
    return actions;
}

function formatDate(data, type, row) {
    if (type === 'display') {
        return moment.utc(data).format('DD/MM/YYYY');
    } else {
        return data;
    }
}

var timeBasedTimesheetTbl = $('#timeBasedTimesheet-table').DataTable($.extend(true, {}, datatableOptions, timeBasedTimesheetDtOptions));
var goalBasedTimesheetTbl = $('#goalBasedTimesheet-table').DataTable($.extend(true, {}, datatableOptions, goalBasedTimesheetDtOptions));

//-- Custom timesheet search enable with datatables
$('#timeBasedTimesheet-table-search').on('keyup', function () {
    var searchValue = $(this).val();
    timeBasedTimesheetTbl.search(searchValue).draw();
});
$('#goalBasedTimesheet-table-search').on('keyup', function () {
    var searchValue = $(this).val();
    goalBasedTimesheetTbl.search(searchValue).draw();
});

//--- Add or Update Timesheet entry
$('.add-timesheet-entry-btn').click(function (e) {
    let missionType = $(this).val();
    console.log(missionType);
    getTimesheetModalForm(missionType);
})

$('#timeBasedTimesheet-table').on('click', '.edit-timesheet-entry-btn', function (e) {
    let timesheetId = +$(this).val();
    getTimesheetModalForm("time", timesheetId)
})
$('#goalBasedTimesheet-table').on('click', '.edit-timesheet-entry-btn', function (e) {
    let timesheetId = +$(this).val();
    getTimesheetModalForm("goal", timesheetId)
})


function getTimesheetModalForm(missionType, timesheetId) {
    $.ajax({
        method: 'POST',
        url: '/Users/UserActivity/GetTimesheetModalForm',
        data: { missionType: missionType, timesheetId: timesheetId },
        success: function (response) {
            console.log(response);
            $("#vt-addOrUpdate-timesheet-dialog").html(response);
            $("#vt-addOrUpdate-timesheet-modal").modal('show');
        },
        error: function (xhr, status, error) {
            console.log("Ajax error: " + error);
        }
    });
}

function submitTimesheetForm(timesheetForm, timesheetSubmitBtn) {
    let timesheetFormData = timesheetForm.serialize();
    console.log(timesheetFormData);
    //let timesheetSubmitBtn = timesheetSubmitBtn;
    showBtnLoader(timesheetSubmitBtn);
    let timesheetMissionType = timesheetSubmitBtn.data("missionType");
    $.ajax({
        method: 'POST',
        url: '/Users/UserActivity/AddOrUpdateTimesheetEntry',
        data: timesheetFormData,
        success: function (response) {
            console.log(response);
            if (timesheetMissionType == "time")
                timeBasedTimesheetTbl.ajax.reload();
            else if (timesheetMissionType == "goal")
                goalBasedTimesheetTbl.ajax.reload();
            toastr.success('Timesheet submitted and updated successfully!', 'Timesheet Updated successfully!', toastrOptions_General);
            $("#vt-addOrUpdate-timesheet-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            console.log("Ajax error: " + error);
            hideBtnLoader(timesheetSubmitBtn);
            toastr.error('Sorry action is not performed, try again!', 'Something went wrong!', toastrOptions_General);
        }
    });
}

$("#confirm-action").on('click', function (e) {
    var timesheetId = $(this).val();
    var timesheetMissionType = $(this).data("missionType");
    console.log(timesheetId);
    console.log(timesheetMissionType);
    $.ajax({
        method: 'POST',
        url: "/Users/UserActivity/RemoveUserTimesheetEntry",
        data: { timesheetId: timesheetId },
        success: function (response) {
            console.log(response);
            toastr.success('Your timesheet entry is successfully deleted!', 'Timesheet entry Removed!', toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
            if (timesheetMissionType == "time")
                timeBasedTimesheetTbl.ajax.reload(null, false);
            else if (timesheetMissionType == "goal")
                goalBasedTimesheetTbl.ajax.reload(null, false);
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})

$(".timesheet-table").on('click', ".remove-timesheet-entry-btn", function (e) {
    $("#confirm-action").val($(this).val());
    $("#confirm-action").data("missionType", $(this).data("missionType"));
})