var missionSkillsDtOptions = {
    serverSide: true,
    ajax: {
        "url": "/Administrator/MissionActivity/GetMissionSkillsDataTable",
        "type": "POST",
        "dataType": "json",
        "error": function (xhr, status) {
            console.log("Ajax error : " + xhr.responseText);
        }
    },
    order: [[1, "desc"]],
    columns: [
        { "data": "skillName", "name": "skillName", "width": "70%" },
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
                console.log(row);
                let actionsObj = {
                    "edit": row.skillId,
                    "delete": row.skillId
                };
                let actions = GetAdminActionsHtml(actionsObj);
                return actions;
            }
        }
    ]
};

var missionSkillsTbl = $('#mission-skills-table').DataTable($.extend(true, {}, datatableOptions, missionSkillsDtOptions));

tableSearch(missionSkillsTbl);

////-- Remove Skill --////
$("#confirm-action").on('click', function (e) {
    var skillId = $(this).val();
    $.ajax({
        method: 'GET',
        url: "/Administrator/MissionActivity/RemoveSkill",
        data: { skillId: skillId },
        success: function (response) {
            missionSkillsTbl.ajax.reload(null, false);
            toastr.success('Skill deleted successfully!', null, toastrOptions_General);
            $("#confirm-remove-modal").modal("hide");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})

//-----------------
$("#admin-add-missionSkill-btn").on('click', function () {
    $("#mission-skill-form").trigger('reset');
    $("#mission-skill-form input, #mission-skill-form select").removeClass('is-invalid');
    $("#mission-skill-form #SkillId").val("");
    $("#mission-skill-modal .modal-title").text("Add New Skill");
})
// Get Mission Theme Edit 
$(".admin-table").on('click', '.edit-entry-action-btn', function (e) {
    let SkillId = $(this).val();
    $("#mission-skill-form input, #mission-skill-form select").removeClass('is-invalid');
    GetMissionSkillEdit(SkillId);
})

$("#mission-skill-form").on('submit', function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        let missionSkillFormData = $(this).serialize();
        $.ajax({
            method: 'POST',
            url: '/Administrator/MissionActivity/AddOrUpdateMissionSkill',
            data: missionSkillFormData,
            success: function (response) {
                console.log(response);
                missionSkillsTbl.ajax.reload(null, false);
                toastr.success(response, null, toastrOptions_General);
                $("#mission-skill-modal").modal("hide");
            },
            error: function (xhr, status, error) {
                toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
                console.log("Ajax error: " + error);
            }
        })
    }
})

function GetMissionSkillEdit(skillId) {
    $.ajax({
        method: 'GET',
        url: '/Administrator/MissionActivity/GetMissionSkillEdit',
        data: { skillId: skillId },
        dataType: 'json',
        success: function (missionSkill) {
            console.log(missionSkill);
            $("#mission-skill-form #SkillId").val(missionSkill.skillId);
            $("#mission-skill-form #SkillName").val(missionSkill.skillName);
            $(`#mission-skill-form #Status option[value="${missionSkill.status}"]`).prop('selected', true);
            $("#mission-skill-modal .modal-title").text("Edit Skill");
            $("#mission-skill-modal").modal("show");
        },
        error: function (xhr, status, error) {
            toastr.error('Action can not be performed, Please try again!', 'Something went wrong!', toastrOptions_General);
            console.log("Ajax error: " + error);
        }
    })
}
