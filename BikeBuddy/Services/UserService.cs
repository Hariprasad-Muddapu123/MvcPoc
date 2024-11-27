using BikeBuddy.Models;
using BikeBuddy.Repositories;
using BikeBuddy.ViewModels;
using System.IO;
using System.Threading.Tasks;

namespace BikeBuddy.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBikeRepository _bikeRepository;

        public UserService(IUserRepository userRepository, IBikeRepository bikeRepository)
        {
            _userRepository = userRepository;
            _bikeRepository = bikeRepository;
        }

        public async Task<User> GetCurrentUserAsync(string userName)
        {
            return await _userRepository.GetUserByNameAsync(userName);
        }

        public async Task<ProfileViewModel> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new ProfileViewModel
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Address = user.Address,
                IsAadhaarUploaded = user.IsAadhaarUploaded,
                IsDrivingLicenseUploaded = user.IsDrivingLicenseUploaded,
                AadhaarUploadStatus = user.IsAadhaarUploaded ? "Aadhaar Uploaded Successfully" : "Aadhaar Not Uploaded",
                DrivingLicenseUploadStatus = user.IsDrivingLicenseUploaded ? "Driving License Uploaded Successfully" : "Driving License Not Uploaded",
                KYCStatus = user.KycStatus,
                ProfileImageBytes = user.ProfileImage,
                AadhaarImageBytes = user.AadhaarImage,
                DrivingLicenseImageBytes = user.DrivingLicenseImage
            };
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, ProfileViewModel model)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) return false;
            user.UserName = string.IsNullOrWhiteSpace(model.UserName) ? user.UserName : model.UserName;
            user.Email = string.IsNullOrWhiteSpace(model.Email) ? user.Email : model.Email;
            user.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? user.PhoneNumber : model.PhoneNumber;
            user.Address = string.IsNullOrWhiteSpace(model.Address) ? user.Address : model.Address;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.ProfileImage.CopyToAsync(memoryStream);
                    user.ProfileImage = memoryStream.ToArray();
                }
            }

            if (model.AadhaarFile != null && model.AadhaarFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.AadhaarFile.CopyToAsync(memoryStream);
                    user.AadhaarImage = memoryStream.ToArray();
                    user.IsAadhaarUploaded = true;
                }
            }

            if (model.DrivingLicenseFile != null && model.DrivingLicenseFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.DrivingLicenseFile.CopyToAsync(memoryStream);
                    user.DrivingLicenseImage = memoryStream.ToArray();
                    user.IsDrivingLicenseUploaded = true;
                }
            }
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}

