document.addEventListener("DOMContentLoaded", async function () {
    const userId = '@User.FindFirstValue(ClaimTypes.NameIdentifier)';

    if (userId) {
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        try {
            await connection.start();
            console.log("SignalR connection established.");

            await connection.invoke("JoinChat", userId);
            console.log(`User joined group: ${userId}`);

            // Fetch unread notifications
            await connection.invoke("FetchNotifications", userId);

            // Listen for new notifications
            connection.on("ReceiveNotification", function (message) {
                displayNotification(message);
            });
        } catch (err) {
            console.error("Error connecting to SignalR:", err);
        }

        connection.onclose(async () => {
            console.log("Reconnecting to SignalR...");
            await connection.start();
            await connection.invoke("JoinChat", userId);
        });
    }

    function displayNotification(message) {
        const overlay = document.getElementById("notification-overlay");

        const notification = document.createElement("div");
        notification.style.background = "rgba(0, 0, 0, 0.8)";
        notification.style.color = "white";
        notification.style.padding = "10px";
        notification.style.marginBottom = "10px";
        notification.style.borderRadius = "5px";
        notification.textContent = message;

        overlay.appendChild(notification);

        setTimeout(() => notification.remove(), 5000);
    }
});
