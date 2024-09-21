$(document).ready(function () {
    $("#notification-settings-btn").on('click', (e) => toggleNotificationSettings());
    $("#back-to-notifications-btn, .notification-settings-cancel-btn").on('click', (e) => {
        toggleNotificationSettings();
        getNotificationSetting();
    });
    getUserNotifications();
    getNotificationSetting();
});

var _userNotificationsList = [];

function toggleNotificationSettings() {
    $(`#notification-settings-btn,
            #notifications-list-wrapper,
            #back-to-notifications-btn,
            #notification-settings-wrapper,
            .notification-heading`).toggleClass("d-none");
    $(".clear-all-notification-btn").toggleClass("invisible");
}

function getUserNotifications() {
    $.ajax({
        url: "/Users/Notification/GetNotifications",
        type: "GET",
        datatype: "json",
        success: function (userNotificationsList) {
            _userNotificationsList = userNotificationsList;
            unreadNotificationCountBadge(_userNotificationsList);
            listUserNotifications(userNotificationsList);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:" + error);
        }
    })
}

function unreadNotificationCountBadge(userNotificationsList) {
    let unreadNotificationCounts = userNotificationsList.filter(notification => notification.isRead == 0).length;

    // Notification count badge
    if (unreadNotificationCounts > 0) {
        $(".notification-count-badge").removeClass('d-none');
        $(".notification-count-badge").text(unreadNotificationCounts);
    } else {
        $(".notification-count-badge").addClass('d-none');
        $(".notification-count-badge").text(unreadNotificationCounts);
    }
}

function listUserNotifications(userNotificationsList) {
    if (userNotificationsList.length > 0) {
        let isOlderLiAppended = false;
        const liOlderNotificationEl = $(`<li class="list-group-item bg-light">
                                        <h5 class="dropdown-header text-center">Older</h5>
                                   </li>`);
        const liNotificationTemplate = `<li class="list-group-item">
                                        <a class="dropdown-item notification-link">
                                            <span class="notification-icon"></span>
                                            <p class="text-start notification-message"></p>
                                            <button type="button" class="btn p-0 rounded-circle notification-unread"></button>
                                            <span class="notification-read d-none">
                                                <i class="bi bi-check-circle-fill"></i>
                                            </span>
                                        </a>
                                    </li>`;
        // Listing Each notification
        userNotificationsList.forEach(notification => {
            let liNotificationEl = $(liNotificationTemplate);
            liNotificationEl.find(".notification-link").attr('id', `notification-${notification.userNotificationId}`);
            liNotificationEl.find(".notification-link").attr('href', notification.notificationLink);
            liNotificationEl.find(".notification-icon").html(getNotificationIcon(notification.notificationFor));
            liNotificationEl.find(`#notification-${notification.userNotificationId} .notification-message`).text(notification.notificationMessage);
            if (notification.isRead == 0)
                liNotificationEl.find(".notification-unread").val(notification.userNotificationId);
            else
                liNotificationEl.find('.notification-unread, .notification-read').toggleClass('d-none');
            if (!isOlderLiAppended && isOlderNotification(notification.createdAt)) {
                isOlderLiAppended = true;
                $("#notifications-list").append(liOlderNotificationEl);
            }
            $("#notifications-list").append(liNotificationEl);
        })
    } else {
        const liNoNotificationsEl = $(`<li class="list-group-item">
                                        <h5 class="dropdown-header text-center">No Notifications!</h5>
                                   </li>`);
        $("#notifications-list").append(liNoNotificationsEl);
    }

}

function isOlderNotification(notification_date) {
    notification_date = new Date(notification_date);
    let yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    console.log(notification_date)
    console.log(yesterday)
    console.log(notification_date < yesterday ? true : false)
    return notification_date < yesterday ? true : false
}

function getNotificationIcon(notificationFor) {
    let notificationIcon;
    switch (notificationFor) {
        case "New Mission":
            notificationIcon = `<i class="bi bi-plus-circle lg-icon"></i>`;
            break;
        case "Mission Approved":
            notificationIcon = `<i class="bi bi-check2-circle lg-icon"></i>`
            break;
        case "Mission Rejected":
            notificationIcon = `<i class="bi bi-x-circle lg-icon"></i>`;
            break;
        case "Story Approved":
            notificationIcon = `<i class="bi bi-journal-check lg-icon"></i>`;
            break;
        case "Story Rejected":
            notificationIcon = `<i class="bi bi-journal-x lg-icon"></i>`;
            break;
        case "Mission Recommendation":
        case "Story Recommendation":
            notificationIcon = `<i class="bi bi-people lg-icon"></i>`;
            break;
        default:
            notificationIcon = `<i class="bi bi-question-circle lg-icon"></i>`;
            break;
    }
    return notificationIcon;
}

function getNotificationSetting() {
    $.ajax({
        url: "/Users/Notification/GetUserNotificationSetting",
        type: "GET",
        datatype: "json",
        success: function (response) {
            console.log(response);
            const notificationSettingForm = $('notification-settings-form').find("input[type=checkbox]");
            notificationSettingCheck(response.recommendedMissions, $("#RecommendedMissions"));
            notificationSettingCheck(response.volunteeringHours, $("#VolunteeringHours"));
            notificationSettingCheck(response.volunteeringGoals, $("#VolunteeringGoals"));
            notificationSettingCheck(response.userComments, $("#UserComments"));
            notificationSettingCheck(response.userStories, $("#UserStories"));
            notificationSettingCheck(response.newMissions, $("#NewMissions"));
            notificationSettingCheck(response.newMessages, $("#NewMessages"));
            notificationSettingCheck(response.recommendedStory, $("#RecommendedStory"));
            notificationSettingCheck(response.notificationByEmail, $("#NotificationByEmail"));
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:" + error);
        }
    })
}

function notificationSettingCheck(currentSetting, settingInputEl) {
    if (currentSetting == 1) {
        console.log("hits")
        settingInputEl.prop('checked', true);
    } else {
        settingInputEl.prop('checked', false);
    }
}

$("#notifications-list").on('click', '.notification-unread', function (e) {

    let unreadBtn = $(this);
    let userNotificationId = $(this).val();

    $.ajax({
        url: '/Users/Notification/MarkAsRead',
        type: 'GET',
        data: { userNotificationId },
        success: function (response) {
            _userNotificationsList = _userNotificationsList.map(notification => {
                if (notification.userNotificationId == userNotificationId)
                    notification.isRead = 1;
                return notification;
            });
            unreadBtn.addClass('d-none');
            unreadBtn.siblings(".notification-read").removeClass('d-none');
            unreadNotificationCountBadge(_userNotificationsList);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:" + error);
        }
    })

    return false;
})

$(".clear-all-notification-btn").on('click', function () {
    $.ajax({
        url: 'Users/Notification/ClearAllNotifications',
        type: 'GET',
        success: function (userNotificationsList) {
            console.log(userNotificationsList);
            $("#notifications-list").empty();
            _userNotificationsList = userNotificationsList;
            unreadNotificationCountBadge(userNotificationsList);
            listUserNotifications(userNotificationsList);
        },
        error: function (xhr, status, error) {
            console.log("Ajax error:" + error);
        }
    })
})

$("#notification-settings-form").on('submit', function (e) {
    e.preventDefault();
    const notificationSettingsFormData = $(this).serialize();
    console.log(notificationSettingsFormData);
    $.ajax({
        url: '/Users/Notification/SaveNotificationSettings',
        type: 'POST',
        data: notificationSettingsFormData,
        success: function (response) {
            console.log(response);
            getNotificationSetting();
            toggleNotificationSettings();
        },
        error: function (xhr, status, error) {
            console.log("Ajax Error : " + error);
        }
    })
})

