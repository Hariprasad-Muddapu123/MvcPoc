using Microsoft.AspNetCore.Mvc.Filters;

namespace BikeBuddy.Filters
{
    public class BlockedUserFilter : IActionFilter
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public BlockedUserFilter(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = _userManager.GetUserId(context.HttpContext.User);
            if (!string.IsNullOrEmpty(userId))
            {
                var isBlocked = _userService.IsUserBlockedAsync(userId).Result;
                if (isBlocked)
                {
                    context.Result = new RedirectToActionResult("BlockedAccount", "Home", new { message = "Your account is blocked, please contact the admin." });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution
        }
    }

}
