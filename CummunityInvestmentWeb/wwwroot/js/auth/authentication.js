const carouselDescriptions = document.querySelectorAll('.carousel-caption .description');

if (screen.width < 556) {
    // Reducing description
    let noOfCharAllowed = 200;
    carouselDescriptions.forEach(carouselDescription => {
        const content = carouselDescription.textContent;
        if (content.length > noOfCharAllowed) {
            carouselDescription.textContent = content.slice(0, noOfCharAllowed) + "...";
        }
    })
}

// Password visible toggle
const passwordVisibleBtn = $("#password-visible-btn");
const confirmPassword = document.getElementById("confimPassword");

passwordVisibleBtn.on('click', function () {
    const passwordField = $("#password-field").get(0);
    const passVisibleIcon = passwordVisibleBtn.find('#password-visible-icon');
    const passInvisibleIcon = passwordVisibleBtn.find('#password-invisible-icon');
    passVisibleIcon.toggleClass("d-none");
    passInvisibleIcon.toggleClass("d-none");
    if (passVisibleIcon.hasClass("d-none")) {
        passwordField.type = 'text';
    }
    if (passInvisibleIcon.hasClass("d-none")) {
        passwordField.type = 'password';
    }
})