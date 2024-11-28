namespace BikeBuddy.Middleware
{
    using BikeBuddy.Models;
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class RoleBasedRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleBasedRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                var userRoles = context.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                var currentPath = context.Request.Path.Value?.ToLower();
                if (userRoles.Contains("Admin") && currentPath == "/admin/index")
                {
                    context.Response.Redirect("/Admin/Index");
                    return;
                }
                else if (userRoles.Contains("User") && currentPath == "/home/index")
                {
                    context.Response.Redirect("/Home/Index");
                    return;
                }
            }
            await _next(context);
        }
    }

}
