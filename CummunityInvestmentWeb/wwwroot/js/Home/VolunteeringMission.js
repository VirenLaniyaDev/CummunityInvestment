const missionImgsSlider = document.querySelector('#mission-imgs-slider');
if (missionImgsSlider) {
    const carousel = new bootstrap.Carousel(missionImgsSlider, {
        wrap: false
    })

    // Expand Image
    const sliderImgs = document.querySelectorAll('.slider-img-wrapper img');
    const expandedImg = document.querySelector('.expanded-img-view .expanded-img');
    sliderImgs.forEach(image => {
        image.addEventListener('click', (e) => {
            expandedImg.src = image.src;
        })
    })
}

// Changing Tabs
const navLinks = document.querySelectorAll('.nav-tabs .nav-link');
const tabContents = document.querySelectorAll('.tab-content');

for (let i = 0; i < navLinks.length; i++) {
    navLinks[i].addEventListener('click', (e) => {
        navLinks.forEach(navLink => {
            navLink.classList.remove('active');
        })
        navLinks[i].classList.add('active');
        tabContents.forEach(tabContent => {
            tabContent.classList.add('d-none');
        });
        tabContents[i].classList.remove('d-none');
    })
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

// Favorite mission
$('.mission-favorite-form').each(function () {
    let thisForm = $(this);
    $(this).submit(function (e) {
        e.preventDefault();
        const missionFavoriteFormData = $(this).serialize();
        console.log(missionFavoriteFormData);
        $.ajax({
            method: 'POST',
            url: '/Users/Home/MissionFavorite',
            data: missionFavoriteFormData,
            success: function (IsFavorite) {
                console.log(IsFavorite);
                if (IsFavorite) {
                    thisForm.find(".favorite-btn .favorite-icon").removeClass("bi-heart icon-primary");
                    thisForm.find(".favorite-btn .favorite-icon").addClass("bi-heart-fill icon-primary");
                    thisForm.find("#favorite-btn-text")?.text('Remove from Favorite');
                }
                else {
                    thisForm.find(".favorite-btn .favorite-icon").removeClass("bi-heart-fill icon-primary");
                    thisForm.find(".favorite-btn .favorite-icon").addClass("bi-heart");
                    thisForm.find("#favorite-btn-text")?.text('Add to Favorite');
                }
            },
            error: function (xhr, status, error) {
                console.log("Ajax error : " + error);
                if (xhr.status == 401) {
                    GoToLogin(window.location.pathname);
                }
            }
        })
    })
})

// Feedback Rate
$("#feedback-ratings-form .feedback-rate input[name=rating]").change(function (e) {
    e.preventDefault();
    let inputRating = $(this).val();
    console.log(inputRating);
    const ratingsFormData = $("#feedback-ratings-form").serialize();
    console.log(ratingsFormData);
    $.ajax({
        method: 'POST',
        url: '/Users/VolunteeringMission/UserRate',
        data: ratingsFormData,
        success: function (response) {
            console.log(response);
            updateRatings(inputRating);
            toastr.success(response.message, response.title, toastrOptions_General);
        },
        error: function (xhr, status, error) {
            toastr.error('Sorry Ratings is not submitted because of some issues!', 'Ratings not submitted!', toastrOptions_General);
            console.log("Ajax error : " + error);
        }
    })
})

const ratingLabels = $("#feedback-ratings-form .feedback-rate input~label");
function updateRatings(inputRating) {
    let i = 1;
    ratingLabels.each(function () {
        let starIcon = $(this).find('.star-icon');
        console.log(starIcon);
        let starFillIcon = $(this).find('.star-fill-icon');
        if (i <= inputRating) {
            starIcon.addClass('d-none');
            starFillIcon.removeClass('d-none');
        } else {
            starIcon.removeClass('d-none');
            starFillIcon.addClass('d-none');
        }
        i++;
    })
}

// Posting Comment
$("#CommentText").on('keyup', function () {
    $(this).removeClass('is-invalid');
    $(this).siblings(".invalid-feedback").text("");
})

function ValidateCommentForm() {
    let errorText;
    let validateStatus;
    if ($("#CommentText").val().length == 0) {
        errorText = "Please enter something!";
        validateStatus = false;
    } else if ($("#CommentText").val().length > 600) {
        errorText = "Only 600 Characters are allowed!";
        validateStatus = false;
    } else {
        validateStatus = true;
    }
    if (!validateStatus) {
        $("#CommentText").focus();
        $("#CommentText").addClass("is-invalid");
        $("#CommentText").siblings(".invalid-feedback").text(errorText);
    }
    console.log(validateStatus);
    return validateStatus;
}

$("#comment-form").submit(function (e) {
    e.preventDefault();
    if (ValidateCommentForm()) {
        var commentFormData = $(this).serialize();
        $.ajax({
            method: 'POST',
            url: '/Users/VolunteeringMission/PostComment',
            data: commentFormData,
            success: function (response) {
                $("#comment-form textarea[name=CommentText]").val("");
                $("#mission-comments-wrapper").html(response);
            },
            error: function (xhr, status, error) {
                toastr.error('Sorry comment is not submitted because of some issues!', 'Comment not submitted!', toastrOptions_General);
                console.log("Ajax error : " + error);
            }
        })
    }
})


//--- Recommend to Co-Workers
var inviteMissionId;
$("document").ready(function () {
    if (userIdentifier)
        getCoWorkers(userIdentifier);

    $(".recommend-to-coWorkers-btn").click(function (e) {
        if (!userIdentifier) {
            GoToLogin(window.location.pathname);
        } else {
            inviteMissionId = $(this).data('inviteMissionId');
            console.log(inviteMissionId);
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

//-- Apply on mission
$("document").ready(function (e) {
    $(".mission-action").on('click', '#apply-mission-btn', function (e) {
        const missionId = $(this).val();
        $.ajax({
            method: 'POST',
            url: '/Users/VolunteeringMission/Apply',
            data: { MissionId: missionId },
            success: function (response) {
                switch (response) {
                    case "Approved":
                        toastr.error('You already applied on this mission!', 'Already Applied!', toastrOptions_General);
                        break;
                    case "Pending":
                        toastr.warning('You once applied for this mission and please wait for approval!', 'Already Applied, Wait for approval!', toastrOptions_General)
                        break;
                    case "Rejected":
                    case "AppliedSuccess":
                        $("#view-mission-details-btn").toggleClass("d-none");
                        $("#apply-mission-btn").toggleClass("d-none");
                        toastr.success('Your application is submitted, please wait for approval!', 'Applied Successfully!', toastrOptions_General);
                        break;
                    case "InvalidStatus":
                        toastr.error('Please try again!', 'Something went wrong', toastrOptions_General);
                        break;
                }
                console.log(response);
            },
            error: function (xhr, status, error) {
                if (xhr.status == 401) {
                    GoToLogin(window.location.pathname);
                }
                console.log("Ajax error : " + error);
                toastr.error('Please try again!', 'Something went wrong', toastrOptions_General);
            }
        })
    })
})

//-- View Mission details
// get the target element by ID
const target = $('#mission-details');
// add an event listener to the anchor link
$('a[href="#mission-details"]').on('click', (event) => {
    // prevent the default behavior of the anchor link
    event.preventDefault();
    console.log("hits")
    const offset = target.offset().top - 200;
    // scroll to the target with the offset
    $('html, body').animate({
        scrollTop: offset,
        easing: "linear",
        speed: 500
    });
});