var missionView = document.querySelector('.mission-main');
var missionCards = document.querySelectorAll('.mission-card');
var gridViewBtn = document.getElementById('grid-view-btn');
var listViewBtn = document.getElementById('list-view-btn');

function enableListView() {
    IsListView = true;
    // Button toggling : Focusing on List view Btn
    gridViewBtn.classList.remove('active-view');
    listViewBtn.classList.add('active-view');
    // Adding List view on Main container
    missionView.classList.add('list-view');
    missionView.classList.remove('grid-view');
    missionView.classList.remove('row-cols-md-2');
    missionView.classList.remove('row-cols-lg-3');
    // Apply styling for all Mission Card
    missionCards.forEach(missionCard => {
        let missionImgContent = missionCard.querySelector('.mission-img-content');
        let missionContent = missionCard.querySelector('.mission-content');
        missionImgContent.classList.add('col-lg-3');
        missionContent.classList.add('col-lg-9');

        let visibleOnListView = missionCard.querySelectorAll('.visible-on-list');
        visibleOnListView.forEach(element => {
            element.classList.remove('d-none');
        })
    })
}

function enableGridView() {
    IsListView = false;
    // Button toggling
    gridViewBtn.classList.add('active-view');
    listViewBtn.classList.remove('active-view');
    // Adding List view on Main container
    missionView.classList.remove('list-view');
    missionView.classList.add('grid-view');
    missionView.classList.add('row-cols-md-2');
    missionView.classList.add('row-cols-lg-3');
    // Apply styling for all Mission Card
    missionCards.forEach(missionCard => {
        let missionImgContent = missionCard.querySelector('.mission-img-content');
        let missionContent = missionCard.querySelector('.mission-content');
        missionImgContent.classList.remove('col-lg-3');
        missionContent.classList.remove('col-lg-9');

        let visibleOnListView = missionCard.querySelectorAll('.visible-on-list');
        visibleOnListView.forEach(element => {
            element.classList.add('d-none');
        })
    })
}

gridViewBtn.addEventListener('click', (e) => {
    enableGridView();
})

listViewBtn.addEventListener('click', (e) => {
    enableListView();
})


var lgScreen = window.matchMedia("(max-width: 992px)");
lgScreen.addEventListener('change', (e) => {
    if (missionView.classList.contains('list-view')) {
        enableGridView();
    }
})
if (lgScreen.matches && missionView.classList.contains('list-view')) {
    enableGridView();
}

// Reducing content
function reduceContent(elementsArr, allowedChar) {
    elementsArr.forEach(element => {
        const content = element.textContent;
        if (content.length > allowedChar) {
            element.textContent = content.slice(0, allowedChar) + "...";
        }
    })
}
reduceContent(document.querySelectorAll('.card-text'), 100);

$(document).ready(function () {
    $('#filter-form input[name="CityFilter"]').change((e) => FilterMission(e))
    $('#filter-form input[name="CityFilter"]').change(() => filterActive())
    $("#pagination-form input").change((e) => FilterMission(e))
    $('#filter-form .filters-container input').change((e) => FilterLabels(e))
    $('#mission-sort-form #mission-sort').change((e) => FilterMission(e));

    // Favorite mission
    $('.mission-favorite-form').each(function () {
        let thisForm = $(this);
        $(this).submit(function (e) {
            e.preventDefault();
            const missionFavoriteFormData = $(this).serialize();
            $.ajax({
                method: 'POST',
                url: '/Users/Home/MissionFavorite',
                data: missionFavoriteFormData,
                success: function (IsFavorite) {
                    if (IsFavorite) {
                        thisForm.find(".favorite-btn .favorite-icon").removeClass("bi-heart icon-primary");
                        thisForm.find(".favorite-btn .favorite-icon").addClass("bi-heart-fill icon-primary");
                    }
                    else {
                        thisForm.find(".favorite-btn .favorite-icon").removeClass("bi-heart-fill icon-primary");
                        thisForm.find(".favorite-btn .favorite-icon").addClass("bi-heart");
                    }
                },
                error: function (xhr, status, error) {
                    if (xhr.status == 401)
                        GoToLogin(window.location.pathname);
                    console.log("Ajax error : " + error);
                }
            })
        })
    })
})

// Filtering Missions
function FilterMission(e) {
    e?.preventDefault();
    $('#loader').removeClass('d-none');
    //$("#mission-container").empty();
    var filterFormData = $("#filter-form").serialize();
    var paginationFormData = $("#pagination-form").serialize();
    var missionSortFormData = $("#mission-sort-form").serialize();
    var formData;
    if (missionSortFormData) {
        missionSortBy = $("#mission-sort").val();
        formData = filterFormData + '&' + paginationFormData + '&' + missionSortFormData;
    } else {
        formData = filterFormData + '&' + paginationFormData;
    }
    $.ajax({
        method: 'POST',
        url: '/Users/Home/FilterMissions',
        datatype: "html",
        data: formData,
        success: function (response) {
            setTimeout(() => {
                $("main").html(response);
                if (IsListView)
                    enableListView();
                // Setting Sort by option
                $("#mission-sort option").each(function () {
                    if ($(this).val() == missionSortBy) {
                        $(this).prop('selected', true);
                    }
                })
                $("#loader").addClass('d-none');
            }, 500);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error : " + error);
        }
    })
}

// Manipulating Filter Labels
var resetFilterBtn;
var labels;
var filterLabelsContainer = $('.filter-labels-container');
function FilterLabels(e) {
    const filterCheckboxes = $('.filters-container .filters input');
    const checkedValues = [];
    filterLabelsContainer.empty();
    labels = [];
    filterCheckboxes.each(function () {
        if ($(this).is(':checked')) {
            checkedValues.push($(this).val());
            let label = $('<button>', {
                class: "btn label-btn rounded-pill my-2 mx-1"
            }).html($(this).val() + ' <i class="bi bi-x-lg label-close"></i>');
            labels.push(label);
            filterLabelsContainer.append(label);
        }
    });
    if (checkedValues.length > 0) {
        resetFilterBtn = $('<button>').attr('type', 'reset').addClass('btn rounded-pill my-2 mx-1').text("Clear All");
        filterLabelsContainer.append(resetFilterBtn);
    }

    //-- Clear all filters
    resetFilterBtn?.on('click', () => {
        //$("#filter-form").trigger('reset');
        $('#filter-form .filters input[type=checkbox]').each(function () {
            $(this).prop('checked', false);
        })
        FilterMission(e);
        filterActive();
        FilterLabels();
    })

    //-- Removing filter using Labels
    filterLabelsContainer.find('.label-btn .label-close')?.click(function (e) {
        let labelBtn = $(this).parent();
        let labelBtnTextVal = $.trim(labelBtn.text());
        $('#filter-form .filters input[type=checkbox]').each(function () {
            if ($(this).val() == labelBtnTextVal) {
                $(this).prop('checked', false);
            }
        })
        FilterMission(e);
        filterActive();
        FilterLabels();
    })
}

//-- Recommend to Coworker
$("document").ready(function (e) {
    if (userIdentifier)
        getCoWorkers(userIdentifier);
    $(".recommend-to-coWorkers-btn").click(function (e) {
        if (!userIdentifier) {
            GoToLogin(window.location.pathname);
        } else {
            inviteMissionId = $(this).data('inviteMissionId');
            $("#recommend-to-coWorkers-modal").modal('show');
        }
    })
})

function getCoWorkers(userId, searchFilter) {
    $.ajax({
        method: 'GET',
        url: '/Users/VolunteeringMission/GetCoWorkers',
        data: { userId: userId, searchFilter: searchFilter },
        success: function (response) {
            $("#recommend-to-coWorkers-modal #coWorkers-modal-body").html(response);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error : " + error);
        }
    })
}