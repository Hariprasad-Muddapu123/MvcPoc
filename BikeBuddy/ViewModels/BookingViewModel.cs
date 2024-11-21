using BikeBuddy.Models;

namespace BikeBuddy.ViewModels
{
    public class BookingViewModel
    {
        public String Amount {  get; set; }
        public String Gst {  get; set; }

        public String RentedHours {  get; set; }

        public String TotalBill {  get; set; }

        public Bike BikeDetails { get; set; }
    }
}
