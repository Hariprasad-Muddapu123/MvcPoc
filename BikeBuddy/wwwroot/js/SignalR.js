document.addEventListener("DOMContentLoaded", async function () {
    // Get the logged-in user's ID
    const userId = "@User.FindFirstValue(ClaimTypes.NameIdentifier)";

    if (userId) {
        // Initialize SignalR connection
        const connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();

        try {
            // Start the connection
            await connection.start();
            console.log("SignalR connection established.");

            // Join the user to their group
            await connection.invoke("JoinChat", userId);
            console.log(`User joined group: ${userId}`);

            // Handle real-time notifications
            connection.on("ReceiveNotification", function (message) {
                // Dynamically display the notification in an overlay
                const overlay = document.getElementById("notification-overlay") || createNotificationOverlay();

                // Create a new notification element
                const notification = document.createElement("div");
                notification.classList.add("notification");
                notification.textContent = message;

                // Add the notification to the overlay
                overlay.appendChild(notification);

                // Automatically remove the notification after 5 seconds
                setTimeout(() => {
                    notification.remove();
                }, 5000);
            });
        } catch (err) {
            console.error("Error connecting to SignalR:", err);
        }

        // Handle reconnection logic
        connection.onclose(async () => {
            console.log("SignalR connection lost. Reconnecting...");
            try {
                await connection.start();
                console.log("SignalR connection reestablished.");
                await connection.invoke("JoinChat", userId);
            } catch (err) {
                console.error("Error reconnecting to SignalR:", err);
            }
        });
    }

    // Function to create and return the notification overlay container
    function createNotificationOverlay() {
        const overlay = document.createElement("div");
        overlay.id = "notification-overlay";
        document.body.appendChild(overlay);
        return overlay;
    }
});