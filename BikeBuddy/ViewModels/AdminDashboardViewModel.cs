namespace BikeBuddy.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalBikes { get; set; }
        public int ApprovedBikes { get; set; }
        public int RejectedBikes { get; set; }

        public int PendingBikes { get; set; }

        public int TotalUsers { get; set; }
        public int KycUsers { get; set; }
        public int NonKycUsers { get; set; }
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Bike> Bikes { get; set; } 



    }
}
