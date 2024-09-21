var IsListView = false;
var missionSortBy;
var inviteMissionId;

//--- Add .active on check in filters
$("document").ready(function () {
    $('.filter-dropdown-menu .list-group .list-group-item input').change(() => filterActive());
    //$('.filter-dropdown-menu .list-group .list-group-item input[name=CountryFilter]').each(function () {
    //    console.log($(this).val());
    //    if ($(this).val() == "India") {
    //        $(this).prop('checked', true);
    //    }
    //})
    FilterMission();
    filterActive();
    FilterLabels();
    ChangeCitiesByCountry();
})

function filterActive() {
    $('.filter-dropdown-menu .list-group .list-group-item input').each(function () {
        if ($(this).prop('checked')) {
            $(this).parent().addClass('active');
        } else {
            $(this).parent().removeClass('active');
        }
    })
}

$(document).ready(function () {
    $("#filter-form").submit((e) => FilterMission(e))
    $('#filter-form input[name="SearchInput"]').on('input', (e) => FilterMission(e))
    //$('#filter-form input[name="CountryFilter"]').change((e) => FilterMission(e))
    $('#filter-form input[name="MissionThemeFilter"]').change((e) => FilterMission(e))
    $('#filter-form input[name="MissionSkillFilter"]').change((e) => FilterMission(e))
    $('#filter-form input[name="CountryFilter"]').change(() => ChangeCitiesByCountry())
    $("#filter-form #offcanvas-form-clear").on('click', function (e) {
        //$("#filter-form").trigger('reset');
        $('#filter-form .filters input[type=checkbox]').each(function () {
            $(this).prop('checked', false);
        })
        FilterMission(e);
        filterActive();
        FilterLabels();
    });
})

function ChangeCitiesByCountry() {
    var countryData = [];
    $('#filter-form input[name="CountryFilter"]').each(function () {
        if ($(this).is(':checked')) {
            countryData.push($(this).val());
        }
    });
    $.ajax({
        method: 'POST',
        url: '/Users/Home/GetCitiesByCountry',
        data: { CountryFilter: countryData },
        dataType: 'html',
        success: function (response) {
            $("#city-dropdown-menu").html(response);
            FilterMission();
            FilterLabels();
        },
        error: function (xhr, status, error) {
            console.log("Ajax error : " + error);
        }
    })
}


