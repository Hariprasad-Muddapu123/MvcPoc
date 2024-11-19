using System.ComponentModel.DataAnnotations.Schema;
using BikeBuddy.ViewModels;
using Microsoft.AspNetCore.Identity;


namespace BikeBuddy.Models
{
    public class User : IdentityUser
    {

        public byte[]? ProfileImage { get; set; }
        public byte[]? AadhaarImage { get; set; }
        public byte[]? DrivingLicenseImage { get; set; }

        public KycStatus KycStatus { get; set; }

        public DateOnly RegistrationDate { get; set; }
        
        public bool IsAadhaarUploaded { get; set; }
        public bool IsDrivingLicenseUploaded { get; set; }
        public String? Address {  get; set; }

        public ICollection<Bike> Bikes { get; set; }
    }
}
