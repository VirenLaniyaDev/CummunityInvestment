const missionImgsSlider = document.querySelector('#story-imgs-slider');
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