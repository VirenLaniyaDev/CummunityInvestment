//-- Tiny Text Editor for Mission Description
tinymce.init({
    selector: 'textarea#mission-description, textarea#organization-detail',
    plugins: 'anchor autolink charmap codesample emoticons link lists searchreplace table visualblocks wordcount',
    toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link table | checklist numlist bullist indent outdent | align lineheight | emoticons charmap | removeformat',
    tinycomments_author: 'Viren Laniya',
    height: 300,
    min_height: 300
});

////-- Getting Cities based on Country selected --////
$("#CountryId").on('change', function (e) {
    SetCountryCities(+$(this).val());
})

////-- Disable/Enable Input field based on Time or Goal mission --////
EnableDisableInputFields($("#MissionType").val());
$("#MissionType").on('change', function (e) {
    EnableDisableInputFields($(this).val());
})

function EnableDisableInputFields(missionType) {
    if (missionType == "goal") {
        $("input#StartDate, input#EndDate").prop('disabled', true);
        $("input#GoalObjectiveText").prop('disabled', false);
        $("label#GoalValueLabel").text("Goal Value");
    } else {
        $("input#StartDate, input#EndDate").prop('disabled', false);
        $("input#GoalObjectiveText").prop('disabled', true);
        $("label#GoalValueLabel").text("Total Seats");
    }
}

function SetCountryCities(countryId) {
    const CitySelect = $('#CityId');
    if (countryId) {
        $.ajax({
            method: 'GET',
            url: '/Administrator/MissionActivity/GetCitiesByCountry',
            data: { countryId: countryId },
            dataType: 'json',
            async: false,
            success: function (cities) {
                CitySelect.empty();
                CitySelect.append(`<option value="">-- Select your city --</option>`);
                $.each(cities, function (index, city) {
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
}

////--- Upload Mission Images and Documents ---////
Dropzone.autoDiscover = false;
//-- Drag and Drop Images
$(document).ready(function () {
    //-- Apply CSS classes on drag and drop events
    $("#drag-drop-zone, #missionDocs-drag-drop-zone").on("dragover", function (event) {
        event.preventDefault();
        $(this).addClass("dragover");
    });

    $("#drag-drop-zone, #missionDocs-drag-drop-zone").on("dragleave", function (event) {
        event.preventDefault();
        $(this).removeClass("dragover");
    });

    $("#drag-drop-zone, #missionDocs-drag-drop-zone").on("drop", function (event) {
        event.preventDefault();
        $(this).removeClass("dragover");
    });

    //-- DropZone
    var toBeSubmitted = false;
    var missionId = +$("#mission-container").data("missionId");
    console.log(missionId);
    var submitTypeBtn = $("#mission-save-btn");

    // Mission Images Dropzone
    var images = [];
    var uniqueMissionImagesNames = [];
    var previewTemplate = `
    <div class="dz-preview dz-image-preview">
        <div class="dz-image">
            <img data-dz-thumbnail />
        </div>
      <div class="dz-details">
        <div class="dz-size" data-dz-size></div>
        <div class="dz-filename"><span data-dz-name></span></div>
      </div>
      <div class="dz-progress"><span class="dz-upload" data-dz-uploadprogress></span></div>
      <div class="dz-success-mark"><i class="bi bi-check-circle-fill mark-icon"></i></div>
      <div class="dz-error-mark"><i class="bi bi-x-circle-fill mark-icon"></i></div>
      <div class="dz-error-message"><span data-dz-errormessage></span></div>
      <button type="button" class="btn dz-remove" data-dz-remove>
        <i class="bi bi-x-lg rg-icon"></i>
      </button>
    </div>`;
    var MissionImagesDropzone = new Dropzone("#drag-drop-zone", {
        dictDefaultMessage: `<i class="bi bi-plus-lg add-icon"></i><p class="drag-drop-text">Drag and Drop Pictures here</p>`,
        previewsContainer: "#mission-image-preview",
        previewTemplate: previewTemplate,
        //paramName: "StoryImages",
        method: 'POST',
        url: "/Administrator/MissionActivity/UploadMissionImages",
        autoProcessQueue: false,
        maxFilesize: 4, // MB
        maxFiles: 20,
        acceptedFiles: ".jpeg,.jpg,.png,.gif",
        addRemoveLinks: false,
        parallelUploads: 30,
        uploadMultiple: true,
        init: function () {
            if (missionId) {
                $.ajax({
                    method: 'GET',
                    url: '/Administrator/MissionActivity/GetUploadedImages',
                    data: { missionId: missionId },
                    success: function (response) {
                        toBeSubmitted = false;
                        let uploadedImages = response;
                        uploadedImages.forEach(uploadedImage => {
                            MissionImagesDropzone.displayExistingFile(uploadedImage, uploadedImage.path);
                        })
                    },
                    error: function (xhr, status, error) {
                        console.log("Ajax error: " + error);
                    }
                });
            }

            this.on("addedfile", function (file) {
                if (!file.type.match(/image.*/)) {
                    alert("You can only upload image files!");
                    this.removeFile(file);
                    return;
                }
                images.push(file);
                console.log(images);
            });

            this.on("removedfile", function (file) {
                var index = images.indexOf(file);
                if (index > -1) {
                    images.splice(index, 1);
                }
                if (missionId && file.missionImageId) {
                    $.ajax({
                        method: 'POST',
                        url: '/Administrator/MissionActivity/RemoveUploadedImage',
                        data: { missionImageId: file.missionImageId },
                        success: function (response) {
                            console.log(response);
                        },
                        error: function (xhr, status, error) {
                            console.log("Ajax error:" + error);
                        }
                    });
                }
            });

            this.on("uploadprogress", function (file) {
                if (file.previewElement) {
                    file.previewElement.classList.add("opacity-100");
                }
            })

            this.on("error", function (file, response) {
                hideBtnLoader(submitTypeBtn);
            });

            this.on("success", function (file, response) {
                uniqueMissionImagesNames = response;
                console.log(uniqueMissionImagesNames);
            });

            this.on("complete", function (file) {
                $("#MissionImagesUniqueNames").val(uniqueMissionImagesNames);
                if (CheckForQueuedOrUpdloadingFiles() && toBeSubmitted) {
                    const missionForm = document.querySelector('#mission-form');
                    setTimeout(function () {
                        missionForm.submit();
                    }, 1000);
                }
            });
        }
    });

    // Mission Documents Dropzone
    var documents = [];
    var uniqueMissionDocsNames = [];
    var previewNode = document.querySelector("#missionDocs-template");
    //previewNode.id = "";
    var missionDocsPreviewTemplate = previewNode.parentNode.innerHTML;
    previewNode.parentNode.removeChild(previewNode);
    var MissionDocumentsDropzone = new Dropzone("#missionDocs-drag-drop-zone", {
        dictDefaultMessage: `<i class="bi bi-plus-lg add-icon"></i><p class="drag-drop-text">Drag and Drop Documents here</p>`,
        previewsContainer: "#mission-documents-preview",
        previewTemplate: missionDocsPreviewTemplate,
        method: 'POST',
        url: "/Administrator/MissionActivity/UploadMissionDocuments",
        autoProcessQueue: false,
        maxFilesize: 4, // MB
        maxFiles: 20,
        acceptedFiles: ".pdf,.docx,.pptx,.xlsx",
        addRemoveLinks: false,
        parallelUploads: 30,
        uploadMultiple: true,
        init: function () {
            if (missionId) {
                $.ajax({
                    method: 'GET',
                    url: '/Administrator/MissionActivity/GetUploadedDocuments',
                    data: { missionId: missionId },
                    success: function (response) {
                        toBeSubmitted = false;
                        let uploadedDocuments = response;
                        uploadedDocuments.forEach(uploadedDocument => {
                            MissionDocumentsDropzone.displayExistingFile(uploadedDocument, uploadedDocument.path);
                        })
                    },
                    error: function (xhr, status, error) {
                        console.log("Ajax error: " + error);
                    }
                });
            }

            this.on("addedfile", function (file) {
                //if (!file.type.match(/image.*/)) {
                //    alert("You can only upload image files!");
                //    this.removeFile(file);
                //    return;
                //}
                setDocumentThumbnail(file);
                documents.push(file);
                console.log(documents);
            });

            this.on('thumbnail', function (file) {
                console.log(file.previewElement);
                setDocumentThumbnail(file);
            });

            this.on("removedfile", function (file) {
                var index = documents.indexOf(file);
                if (index > -1) {
                    documents.splice(index, 1);
                }
                console.log(file.missionDocumentId);
                if (missionId && file.missionDocumentId) {
                    $.ajax({
                        method: 'POST',
                        url: '/Administrator/MissionActivity/RemoveUploadedDocument',
                        data: { missionDocumentId: file.missionDocumentId },
                        success: function (response) {
                            console.log(response);
                        },
                        error: function (xhr, status, error) {
                            console.log("Ajax error:" + error);
                        }
                    });
                }
            });

            this.on("uploadprogress", function (file) {
                if (file.previewElement) {
                    file.previewElement.classList.add("opacity-100");
                }
            })

            this.on("error", function (file, response) {
                hideBtnLoader(submitTypeBtn);
            });

            this.on("success", function (file, response) {
                console.log(response);
                uniqueMissionDocsNames = response;
                console.log(uniqueMissionDocsNames);
            });

            this.on("complete", function (file) {
                $("#MissionDocumentsUniqueNames").val(uniqueMissionDocsNames);
                if (CheckForQueuedOrUpdloadingFiles() && toBeSubmitted) {
                    const missionForm = document.querySelector('#mission-form');
                    const missionFormData = new FormData(missionForm);
                    console.log(missionFormData);
                    console.log(missionFormData);
                    setTimeout(function () {
                        missionForm.submit();
                    }, 1000);
                }
            });
        }
    });

    function setDocumentThumbnail(file) {
        switch (file.type) {
            case "application/pdf":
            case ".pdf":
                file.previewElement.querySelector("img.dz-image").src = "/assets/file-pdf-regular.svg";
                break;
            case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
            case ".docx":
                file.previewElement.querySelector("img").src = "/assets/file-word-regular.svg";
                break;
            case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
            case ".xlsx":
                file.previewElement.querySelector("img").src = "/assets/file-excel-regular.svg";
                break;
            case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
            case ".pptx":
                file.previewElement.querySelector("img").src = "/assets/file-powerpoint-regular.svg";
                break;
            default:
                file.previewElement.querySelector("img").src = "/assets/question-solid.svg";
                break;
        }
    }

    $("#mission-form").submit(function (event) {
        event.preventDefault();
        if ($(this).valid()) {
            showBtnLoader(submitTypeBtn);
            if (CheckForQueuedOrUpdloadingFiles()) {
                $(this).off("submit").submit();
            } else {
                toBeSubmitted = true;
                MissionImagesDropzone.processQueue();
                MissionDocumentsDropzone.processQueue();
            }
        }
    });

    function CheckForQueuedOrUpdloadingFiles() {
        let totalQueuedFiles = MissionImagesDropzone.getQueuedFiles().length + MissionDocumentsDropzone.getQueuedFiles().length;
        let totalUploadingFiles = MissionImagesDropzone.getUploadingFiles().length + MissionDocumentsDropzone.getQueuedFiles().length;
        if (totalQueuedFiles == 0 && totalUploadingFiles == 0)
            return true;
        return false;
    }
})


////--- Add Mission Skills ---////
filterActive(); // For initial selections
$("#mission-add-skills-modal #mission-skills-select-list").on("change", "input[name='Skills']", () => filterActive())
$("#mission-add-skills-modal #mission-selected-skills-list").on("change", "input[name='selectedSkills']", () => SelectedSkillsSFilterActive())

function filterActive() {
    $('#mission-add-skills-modal #mission-skills-select-list .list-group-item input').each(function () {
        if ($(this).prop('checked')) {
            $(this).parent().addClass('active');
        } else {
            $(this).parent().removeClass('active');
        }
    })
}

function SelectedSkillsSFilterActive() {
    $('#mission-add-skills-modal #mission-selected-skills-list .list-group-item input').each(function () {
        if ($(this).prop('checked')) {
            $(this).parent().addClass('active');
        } else {
            $(this).parent().removeClass('active');
        }
    })
}

////-- Add Skills Modal --////
var skillsSelectList = $("#mission-add-skills-modal #mission-skills-select-list");
var selectedSkillsList = $("#mission-add-skills-modal #mission-selected-skills-list");

var addSelectedSkillsBtn = $("#mission-add-skills-modal .skill-selector-item #add-selected-skills-btn");
var removeSelectedSkillsBtn = $("#mission-add-skills-modal .skill-selector-item #remove-selected-skills-btn");

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

$("#mission-save-skills-btn").on('click', function (e) {
    saveSelectedSkills();
    $("#mission-add-skills-modal").modal("hide");
})

$(".close-select-skills-btn").on('click', function () {
    setToDefaultSkills();
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
            <input type="checkbox" name="selectedSkills" class="btn-check" id="mission-selectedSkills-${skillId}" value=${skillId} autocomplete="off" />
            <label class="stretched-link" for="mission-selectedSkills-${skillId}">${skillName}</label>
        </li>
    `;
    selectedSkillsList.append(skillListItem);
}

function saveSelectedSkills() {
    const savedUserSkillsList = $("#saved-mission-skills-list");
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
    const savedUserSkillsList = $("#saved-mission-skills-list");
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