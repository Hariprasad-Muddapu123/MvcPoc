
    document.addEventListener("DOMContentLoaded", async function () {
            // Check if the user is logged in
    const userId = "@User.FindFirstValue(ClaimTypes.NameIdentifier)"; // Get logged-in user's ID

    if (userId) {
                // Initialize SignalR connection
                const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub") // Replace with your NotificationHub URL
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
        // Display the notification (example using alert)
        alert(message);

    // Alternatively, you can dynamically display the notification on the UI
    const notificationContainer = document.createElement("div");
    notificationContainer.classList.add("alert", "alert-info", "mt-2");
    notificationContainer.textContent = message;
    document.body.appendChild(notificationContainer);
                    });
                } catch (err) {
        console.error("Error connecting to SignalR:", err);
                }

                // Handle connection errors or disconnects
                connection.onclose(async () => {
        console.log("SignalR connection lost. Reconnecting...");
    try {
        await connection.start();
    console.log("SignalR connection reestablished.");
    await connection.invoke("JoinGroup", userId);
                    } catch (err) {
        console.error("Error reconnecting to SignalR:", err);
                    }
                });
            }
        });
