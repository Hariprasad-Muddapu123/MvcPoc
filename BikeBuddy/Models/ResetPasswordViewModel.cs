using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
