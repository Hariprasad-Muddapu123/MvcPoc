using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.ViewModels
{
	public class VerifyOtpViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Otp { get; set; }

		public TimeSpan ExpirationTime {  get; set; }
	}
}
