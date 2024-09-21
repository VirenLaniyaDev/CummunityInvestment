const missionImgsSlider = document.querySelector('#story-imgs-slider');
if (missionImgsSlider) {
    const carousel = new bootstrap.Carousel(missionImgsSlider, {
        wrap: false
    })

    // Inject YouTube API script
    var tag = document.createElement('script');
    tag.src = "//www.youtube.com/player_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

    //var player;
    //function onYouTubePlayerAPIReady() {
    //    $('.slider-img-wrapper .slide-video').click(function () {
    //        expandedImg.addClass('d-none');
    //        expandedVideo.removeClass('d-none');
    //        expandedVideo.prop('src', $(this).prop('src'));
    //        // create the global player from the specific iframe (#video)
    //        player = new YT.Player('expanded-video', {
    //            events: {
    //                // call this function when player is ready to use
    //                'onReady': onPlayerReady
    //            }
    //        });
            
    //    })
    //}
    // Expand Image
    const expandedImg = $('.expanded-img-view .expanded-img');
    const expandedVideo = $('.expanded-img-view #expanded-video');
    const slideVideos = $('.slider-img-wrapper .slide-video');

    $('.slider-img-wrapper .slide-img').click(function () {
        expandedVideo.addClass('d-none');
        expandedImg.removeClass('d-none');
        slideVideos.find(".play-video-btn").removeClass("d-none");
        slideVideos.find(".pause-video-btn").addClass("d-none");
        expandedImg.prop('src', $(this).prop('src'));
    })

    slideVideos.click(function () {
        expandedImg.addClass('d-none');
        expandedVideo.removeClass('d-none');
        expandedVideo.prop('src', $(this).data('src'));
        slideVideos.find(".play-video-btn").removeClass("d-none");
        slideVideos.find(".pause-video-btn").addClass("d-none");
        $(this).find(".play-video-btn").toggleClass("d-none");
        $(this).find(".pause-video-btn").toggleClass("d-none");
    })
    //function onPlayerReady(event) {
    //    let state = player.getPlayerState();
    //    console.log(state);
    //    if (state == 1) {
    //        player.pauseVideo();
    //    } else {
    //        player.playVideo();
    //    }
    //}
    
}

//--- Recommend to Co-Workers
$("document").ready(function () {
    getCoWorkers();

    $(".recommend-to-coWorkers-btn").click(function (e) {
        $("#recommend-to-coWorkers-modal").modal('show');
    })
})

function getCoWorkers(searchFilter) {
    $.ajax({
        method: 'GET',
        url: '/Users/VolunteerStory/GetCoWorkers',
        data: { searchFilter: searchFilter },
        success: function (response) {
            console.log(response);
            $("#recommend-to-coWorkers-modal #coWorkers-modal-body").html(response);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error : " + error);
        }
    })
}