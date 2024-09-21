tinymce.init({
    selector: 'textarea#story-description',
    plugins: 'anchor autolink charmap codesample emoticons link lists searchreplace table visualblocks wordcount',
    toolbar: 'undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link table | checklist numlist bullist indent outdent | align lineheight | emoticons charmap | removeformat',
    tinycomments_author: 'Viren Laniya',
    height: 300,
    min_height: 300
});

//-- 
$(document).ready(function () {


});

Dropzone.autoDiscover = false;
$("document").ready(function () {


    textAreaLineLimit(20, $("#StoryVideoURLs"));



    //    var imageFiles = [];

    //-- Drag and Drop Images
    $("#drag-drop-zone").on("dragover", function (event) {
        event.preventDefault();
        $(this).addClass("dragover");
    });

    $("#drag-drop-zone").on("dragleave", function (event) {
        event.preventDefault();
        $(this).removeClass("dragover");
    });

    $("#drag-drop-zone").on("drop", function (event) {
        event.preventDefault();
        $(this).removeClass("dragover");
    });

    //    // Browse functionality
    //    $("#drag-drop-zone").on("change", function (event) {
    //        var files = event.target.files;
    //        imageFiles = imageFiles.concat(Array.from(files));
    //        displayImages(imageFiles);
    //    });

    //    // Function to display selected images
    //    function displayImages(files) {
    //        $("#drap-drop-preview").empty();
    //        if (files.length > 0) {
    //            for (var i = 0; i < files.length; i++) {
    //                var file = files[i];
    //                var reader = new FileReader();
    //                reader.onload = function (event) {
    //                    var img = $("<img>").attr("src", event.target.result);
    //                    $("#drap-drop-preview").append(img);
    //                };
    //                reader.readAsDataURL(file);
    //            }
    //        } else {
    //            $("#drap-drop-preview").html("<p>No images selected</p>");
    //        }
    //    }

    //-- DropZone
    var toBeSubmitted = false;
    var images = [];
    var uniqueStoryImagesNames = [];
    var submitTypeBtn = $("#story-form-saveBtn");

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

    var StoryImagesDropzone = new Dropzone("#drag-drop-zone", {
        dictDefaultMessage: `<i class="bi bi-plus-lg add-icon"></i><p class="drag-drop-text">Drag and Drop Pictures here</p>`,
        previewsContainer: "#story-image-preview",
        previewTemplate: previewTemplate,
        paramName: "StoryImages",
        method: 'POST',
        url: "/Users/VolunteerStory/UploadPhotos",
        autoProcessQueue: false,
        maxFilesize: 4, // MB
        maxFiles: 20,
        acceptedFiles: ".jpeg,.jpg,.png,.gif",
        addRemoveLinks: false,
        parallelUploads: 30,
        uploadMultiple: true,
        init: function () {
            if(storyId != 0){
                $.ajax({
                    method: 'GET',
                    url: '/Users/VolunteerStory/GetUploadedImages',
                    data: { storyId: storyId },
                    success: function (response) {
                        toBeSubmitted = false;
                        let uploadedImages = response;
                        uploadedImages.forEach(uploadedImage => {
                            StoryImagesDropzone.displayExistingFile(uploadedImage, `/data/Images/Story/${uploadedImage.name}`);
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
                //storyImages.files = images;
                //$("#StoryImages").prop("files", images);
                //console.log($("#StoryImages").prop("files"));
                //$("#StoryImages").val(images.join(','));
            });

            this.on("removedfile", function (file) {
                var index = images.indexOf(file);
                if (index > -1) {
                    images.splice(index, 1);
                }
                console.log(file.name);
                if(storyId != 0){
                    $.ajax({
                        method: 'POST',
                        url: '/Users/VolunteerStory/RemoveUploadedImage',
                        data: { storyId : storyId, fileName: file.name },
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
                console.log(file);
                hideBtnLoader(submitTypeBtn);
            });

            this.on("success", function (file, response) {
                console.log(response);
                uniqueStoryImagesNames = response;
                console.log(uniqueStoryImagesNames);
            });

            this.on("complete", function (file) {
                if (this.getQueuedFiles().length === 0 && this.getUploadingFiles().length === 0 && toBeSubmitted) {
                    const storyForm = document.querySelector('#share-story-form');
                    const storyFormData = new FormData(storyForm);
                    console.log(storyFormData);
                    //uniqueStoryImagesNames.forEach(uniqueName => storyFormData.append('StoryImagesUniqueNames', uniqueName));
                    //storyFormData.set('StoryImagesUniqueNames', StoryImagesUniqueNames);
                    $("#StoryImagesUniqueNames").val(uniqueStoryImagesNames);
                    console.log(storyFormData);
                    setTimeout(function () {
                        storyForm.submit();
                    }, 1000);
                }
            });
        }
    });

    $("#remove-story-btn").click(function (e) {
        e.preventDefault();
        $("#confirm-action").val($(this).val());
    })

    $("#confirm-action").on('click', function (e) {
        let form = $('<form>', {
            method: 'POST',
            action: '/Users/VolunteerStory/RemoveUserStory'
        });

        let storyIdInput = $('<input>', {
            type: 'hidden',
            name: 'StoryId',
            value: $(this).val()
        })

        form.append(storyIdInput);
        console.log(form);
        $("main").append(form);
        form.submit();
    })

    $("#story-form-saveBtn").click(function (e) {
        $("#StoryAction").val("draft");
        submitTypeBtn = $("#story-form-saveBtn");
    })

    $("#story-form-submitBtn").click(function (e) {
        $("#StoryAction").val("pending");
        submitTypeBtn = $("#story-form-submitBtn");
    })

    $("#share-story-form").submit(function (event) {
        event.preventDefault();
        //event.stopPropagation();
        if ($(this).valid()) {
            showBtnLoader(submitTypeBtn);
            if (StoryImagesDropzone.getQueuedFiles().length === 0 && StoryImagesDropzone.getUploadingFiles().length === 0) {
                $(this).off("submit").submit();
            } else {
                toBeSubmitted = true;
                StoryImagesDropzone.processQueue();
            }
        }
    });

});
