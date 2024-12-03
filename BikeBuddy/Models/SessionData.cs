namespace BikeBuddy.Models
{
    public class SessionData
    {
        public int BikeId { get; set; }
        public string PickupDateTime { get; set; }
        public string DropoffDateTime { get; set; }
        public string TotalPrice { get; set; }
        public string RentedHours { get; set; }
        public string Gst { get; set; }
        public string TotalBill { get; set; }
    }
}
