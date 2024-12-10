using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name is required.")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public IEnumerable<AuthenticationScheme>? Schemes { get; set; }
    }
}
