namespace BikeBuddy.ViewModels
{
    public class UserDetailsViewModel
    {
        public String UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public byte[] AadhaarImage { get; set; }
        public byte[] DrivingLicenseImage { get; set; }
        public KycStatus KycStatus { get; set; }
    }
}
