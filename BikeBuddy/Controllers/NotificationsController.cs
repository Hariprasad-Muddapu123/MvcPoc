using Microsoft.AspNetCore.Mvc;

namespace BikeBuddy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationsController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications(string userId)
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync(userId);
            return Ok(notifications);
        }

        //[HttpPost("mark-as-read")]
        //public async Task<IActionResult> MarkAsRead(int notificationId)
        //{
        //    await _notificationRepository.MarkAsReadAsync(notificationId);
        //    return Ok();
        //}

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(notificationId);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true; // Mark the notification as read
            await _notificationRepository.UpdateNotificationAsync(notification);

            return Ok();
        }
    }

}
