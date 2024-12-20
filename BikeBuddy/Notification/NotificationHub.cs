namespace BikeBuddy.Notification
{
    using Microsoft.AspNetCore.SignalR;
    public class NotificationHub : Hub
    {
        public async Task JoinChat(string userId)
        {
            // Add user to the group
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"User {Context.UserIdentifier} joined group {userId}");
        }

        public async Task SendMessage(string userId, string message)
        {
            var senderId = Context.UserIdentifier;  

            // Broadcast the message to the relevant group
            await Clients.Group(userId).SendAsync("ReceiveMessage", message);
        }
    }
}
