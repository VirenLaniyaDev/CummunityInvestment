$(function () {
    //// Previewing Selected image ////
    $('#NewUserAvatar').on('change', (event) => {
        console.log("Hits");
        $('#up-user-avatar img').attr('src', URL.createObjectURL(event.target.files[0]));
    });

    //// Manipulating Profile name on input ////
    const FirstNameInput = $(".up-basic-information-container #FirstName");
    const LastNameInput = $(".up-basic-information-container #LastName");
    FirstNameInput.on('input', function (e) {
        $("#up-user-name").text(FirstNameInput.val() + " " + LastNameInput.val());
    })
    LastNameInput.on('input', function (e) {
        $("#up-user-name").text(FirstNameInput.val() + " " + LastNameInput.val());
    })

    // Password visible toggle
    const passwordVisibleBtn = $("#password-visible-btn");

    passwordVisibleBtn.on('click', function () {
        const passwordField = $("#NewPassword").get(0);
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

    //// Change/Update Password ////
    $(".cancel-change-pass-btn").on('click', function (e) {
        $("#change-password-form").trigger('reset');
        $("#change-password-form input").removeClass('is-invalid');
    });

    $("#change-password-form").submit(function (e) {
        e.preventDefault();
        if ($(this).valid()) {
            console.log("Form is valid!");
            let changePasswordBtn = $("#up-change-password-btn");
            showBtnLoader(changePasswordBtn);
            var changePasswordData = {
                OldPassword: $(this).find("#OldPassword").val(),
                NewPassword: $(this).find("#NewPassword").val(),
                ConfirmPassword: $(this).find("#ConfirmPassword").val()
            }
            console.log(changePasswordData);
            $.ajax({
                method: 'POST',
                url: '/Users/UserActivity/ChangePassword',
                data: changePasswordData,
                success: function (response) {
                    console.log(response);
                    hideBtnLoader(changePasswordBtn);
                    $("#up-change-password-modal").modal("hide");
                    toastr.success(response.message, response.title, toastrOptions_General);
                    $("#change-password-form").trigger('reset');
                },
                error: function (xhr, status, error) {
                    console.log(xhr);
                    hideBtnLoader(changePasswordBtn);
                    let errorMessage = xhr.responseJSON.detail;
                    switch (errorMessage) {
                        case "Invalid Current Password!":
                            $("#OldPassword").addClass("is-invalid");
                            $("#OldPassword").find("~.invalid-feedback").text(errorMessage);
                            toastr.error('Entered current password is not valid!', errorMessage, toastrOptions_General);
                            break;
                        case "New password should not be same as Current password!":
                            $("#NewPassword").addClass("is-invalid");
                            $("#NewPassword").find("~.invalid-feedback").text(errorMessage);
                            break;
                        case "401":
                            toastr.error('You are not allowed to perform this action!', 'Unauthorized!', toastrOptions_General);
                            break;
                        default:
                            toastr.error('Some error occurs while changing your password!', 'Something went wrong!', toastrOptions_General);
                            break;
                    }
                }
            })
        } 
    })

    //// Active skills /////
    filterActive(); // For initial selections
    $("#up-add-skills-modal #up-skills-select-list").on("change", "input[name='Skills']", () => filterActive())
    $("#up-add-skills-modal #up-selected-skills-list").on("change", "input[name='selectedSkills']", () => SelectedSkillsSFilterActive())

    function filterActive() {
        $('#up-add-skills-modal #up-skills-select-list .list-group-item input').each(function () {
            if ($(this).prop('checked')) {
                $(this).parent().addClass('active');
            } else {
                $(this).parent().removeClass('active');
            }
        })
    }

    function SelectedSkillsSFilterActive() {
        $('#up-add-skills-modal #up-selected-skills-list .list-group-item input').each(function () {
            if ($(this).prop('checked')) {
                $(this).parent().addClass('active');
            } else {
                $(this).parent().removeClass('active');
            }
        })
    }

    //// Getting Cities based on Country selected ////
    $(".up-address-information-container #CountryId").on('change', function (e) {
        const CitySelect = $('.up-address-information-container #CityId');
        let countryId = +$(this).val();
        console.log(countryId);
        if (countryId) {
            $.ajax({
                method: 'GET',
                url: '/Users/UserActivity/GetCitiesByCountryId',
                data: { countryId: countryId },
                dataType: 'json',
                success: function (cities) {
                    console.log(cities);
                    CitySelect.empty();
                    CitySelect.append(`<option value="">-- Select your city --</option>`);
                    $.each(cities, function (index, city) {
                        console.log(city);
                        CitySelect.append(`<option value="${city.cityId}">${city.name}</option>`);
                    });
                },
                error: function (xhr, status, error) {
                    console.log("Ajax error: " + error);
                }
            })
        } else {
            CitySelect.empty();
        }
    })

    //// Add Skills Modal ////
    var skillsSelectList = $("#up-add-skills-modal #up-skills-select-list");
    var selectedSkillsList = $("#up-add-skills-modal #up-selected-skills-list");

    var addSelectedSkillsBtn = $("#up-add-skills-modal .skill-selector-item #add-selected-skills-btn");
    var removeSelectedSkillsBtn = $("#up-add-skills-modal .skill-selector-item #remove-selected-skills-btn");

    addSelectedSkillsBtn.on('click', function (e) {
        addSelectedSkills();
    })

    removeSelectedSkillsBtn.on('click', function (e) {
        let removeSelectedSkillValues = [];
        selectedSkillsList.find("input[name=selectedSkills]").each(function () {
            if ($(this).prop('checked')) {
                removeSelectedSkillValues.push($(this).val());
            }
        });
        if (removeSelectedSkillValues.length > 0) {
            skillsSelectList.find("input[name=Skills]").each(function () {
                if (removeSelectedSkillValues.includes($(this).val())) {
                    $(this).prop('checked', false);
                }
            })
            filterActive();
            addSelectedSkills();
        }
    })

    $("#up-save-skills-btn").on('click', function (e) {
        saveSelectedSkills();
        $("#up-add-skills-modal").modal("hide");
    })

    function addSelectedSkills() {
        selectedSkillsList.empty();
        skillsSelectList.find("input[name=Skills]").each(function (e) {
            if ($(this).prop('checked')) {
                let skillId = $(this).val();
                let skillName = $(this).find("~label").text();
                addSkillToSelectedSkillsList(skillId, skillName);
            }
        });
    }

    function addSkillToSelectedSkillsList(skillId, skillName) {
        let skillListItem = `
        <li class="list-group-item py-1 border-bottom-0 mb-1">
            <input type="checkbox" name="selectedSkills" class="btn-check" id="up-selectedSkills-${skillId}" value=${skillId} autocomplete="off" />
            <label class="stretched-link" for="up-selectedSkills-${skillId}">${skillName}</label>
        </li>
    `;
        selectedSkillsList.append(skillListItem);
    }

    function saveSelectedSkills() {
        const savedUserSkillsList = $("#up-user-skills-list");
        savedUserSkillsList.empty();
        selectedSkillsList.find("input[name=selectedSkills]").each(function () {
            let skillId = $(this).val();
            let skillName = $(this).find("~label").text();
            let savedSkillListItem = `
            <li class="list-group-item py-1">
                <input type="checkbox" name="SkillIds" value=${skillId} checked hidden readonly>
                <label>${skillName}</label>
            </li>
            `;
            savedUserSkillsList.append(savedSkillListItem);
        });
    }

    function setToDefaultSkills() {
        const savedUserSkillsList = $("#up-user-skills-list");
        console.log(savedUserSkillsList);
        let savedSkillValues = [];
        savedUserSkillsList.find("li input").each(function () {
            console.log($(this));
            savedSkillValues.push($(this).val());
        });
        console.log(savedSkillValues);
        skillsSelectList.find("input[name=Skills]").each(function () {
            if (savedSkillValues.includes($(this).val()))
                $(this).prop('checked', true);
            else
                $(this).prop('checked', false);
        })
        filterActive();
        addSelectedSkills();
    }

    $(".close-select-skills-btn").on('click', function () {
        setToDefaultSkills();
    })
})