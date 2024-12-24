using BikeBuddy.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BikeBuddy.Controllers
{
    public class WishlistController : Controller
    {
        private readonly WishlistService _wishlistService;

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpPost]
        public async Task<IActionResult> ToggleWishlist(int bikeId)
        {
            if (bikeId <= 0)
                return BadRequest(new { message = "Invalid bike ID." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var isInWishlist = await _wishlistService.ToggleWishlistAsync(userId, bikeId);
            return Json(new { isInWishlist });
            //return RedirectToAction("Getwishlist");
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromWishlist(int bikeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await _wishlistService.RemoveFromWishlistAsync(userId, bikeId);
            return RedirectToAction("Getwishlist");
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var wishlist = await _wishlistService.GetWishlistAsync(userId);
            
            return View(wishlist);
        }
    }

}
