function fetchNotifications() {
    const userId = "@User.Identity.GetUserId()"; // Replace with the correct method to retrieve the logged-in user ID

    $.ajax({
        url: `/api/Notifications/unread?userId=${userId}`,
        type: "GET",
        success: function (notifications) {
            if (notifications.length > 0) {
                const notificationContainer = $("#notificationContainer");
                notificationContainer.empty(); // Clear previous notifications to avoid duplication

                notifications.forEach(notification => {
                    // Append each notification to the container
                    notificationContainer.append(`
                        <div class="notification" id="notification-${notification.id}">
                            <p>${notification.message}</p>
                            <button onclick="markAsRead(${notification.id})">Mark as Read</button>
                        </div>
                    `);
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching notifications:", error);
        }
    });
}

function markAsRead(notificationId) {
    $.ajax({
        url: `/api/Notifications/mark-as-read?notificationId=${notificationId}`,
        type: "POST",
        success: function () {
            $(`#notification-${notificationId}`).remove(); // Remove the notification from UI
            console.log("Notification marked as read.");
        },
        error: function (xhr, status, error) {
            console.error("Error marking notification as read:", error);
        }
    });
}


// Fetch notifications immediately when the page loads
$(document).ready(fetchNotifications);

// Periodically fetch notifications every 30 seconds
setInterval(fetchNotifications, 30000);
