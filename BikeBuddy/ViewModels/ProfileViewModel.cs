using BikeBuddy.Models;

namespace BikeBuddy.ViewModels
{
    public class ProfileViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public IFormFile? ProfileImage { get; set; }
        public byte[]? ProfileImageBytes { get; set; }
        public string ImageBase64 { get; set; }
        public IFormFile? AadhaarFile { get; set; }
        public byte[]? AadhaarImageBytes { get; set; }

        public IFormFile? DrivingLicenseFile { get; set; }
        public byte[]? DrivingLicenseImageBytes { get; set; }

        public string? AadhaarUploadStatus { get; set; }
        public string? DrivingLicenseUploadStatus { get; set; }
        public bool? IsAadhaarUploaded { get; set; }
        public bool? IsDrivingLicenseUploaded { get; set; }
        public KycStatus? KYCStatus { get; set; }
    }

}
