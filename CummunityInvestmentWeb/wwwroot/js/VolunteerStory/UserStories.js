$("document").ready(function () {
    UserStoryFilter();
    $("#u-story-filter-form").submit((e) => UserStoryFilter(e));
    $("#u-story-filter-form #SearchStory").on('input', (e) => UserStoryFilter(e));
    $("#u-story-filter-form #StoryStatus").change((e) => UserStoryFilter(e));
    $("#u-story-filter-form #StorySortBy").change((e) => UserStoryFilter(e));
})

function UserStoryFilter(e) {
    e?.preventDefault();
    let UStoryFormData = $("#u-story-filter-form").serialize();
    let paginationFormData = $("#pagination-form").serialize();
    console.log(paginationFormData);
    let formData = UStoryFormData + "&" + paginationFormData;
    console.log(formData);
    console.log(UStoryFormData);
    $.ajax({
        method: 'POST',
        url: '/Users/VolunteerStory/GetUserStories',
        data: formData,
        datatype: 'html',
        success: function (response) {
            console.log(response);
            $("#user-stories-tbl-container").html(response);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:" + error);
        }
    })
}

$("#confirm-action").on('click', function (e) {
    var storyId = $(this).val();
    var currentPage = $('#user-stories-pager').data('currentPage');
    console.log(storyId);
    console.log(currentPage);
    $("#confirm-remove-modal").modal("hide");
    $.ajax({
        method: 'POST',
        url: "/Users/VolunteerStory/RemoveUserStoryAjax",
        datatype: 'html',
        data: { StoryId: storyId, pg: currentPage },
        success: function (response) {
            //console.log(response);
            $("#user-stories-tbl-container").html(response);
            toastr.success('Story Removed!', 'On your request the story is successfully deleted!', toastrOptions_General);
        },
        error: function (xhr, status, error) {
            toastr.error('Something went wrong!', 'Story is not deleted, try again!', toastrOptions_General);
            console.log("Ajax error:" + error);
        }
    })
})

//function StoryRemove(storyId, e) {
//    ConfirmAction(function (confirm) {
//            if (confirm) {
//                console.log(storyId);
//                $.ajax({
//                    method: 'POST',
//                    url: "/Users/VolunteerStory/RemoveUserStoryAjax",
//                    datatype: 'html',
//                    data: { StoryId: storyId },
//                    cache: false,
//                    success: function (response) {
//                        console.log(response);
//                        $("#user-stories-tbl-container").html(response);
//                        toastr.success('Story Removed!', 'On your request the story is successfully deleted!', toastrOptions_General);
//                    },
//                    error: function (xhr, status, error) {
//                        toastr.error('Something went wrong!', 'Story is not deleted, try again!', toastrOptions_General);
//                        console.log("Ajax error:" + error);
//                    }
//                })
//                e.stopImmediatePropagation();
//                return false;
//            }
//        });
//}



