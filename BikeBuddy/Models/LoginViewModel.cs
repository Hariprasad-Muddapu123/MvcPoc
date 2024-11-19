using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.Models
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

        //[Display(Name = "Remember Me")]
        //public bool RememberMe { get; set; }
    }
}
