using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.Models
{
    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
    }
}
