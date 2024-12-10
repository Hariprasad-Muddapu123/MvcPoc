using System.ComponentModel.DataAnnotations;

namespace BikeBuddy.ViewModels
{
    public class CancelRideViewModel
    {

        public Ride Ride {get ; set;}

        [Required(ErrorMessage = "Please provide a reason for cancellation.")]
        public string Reason { get; set; }
    }
}
