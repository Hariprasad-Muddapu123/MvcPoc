namespace BikeBuddy.Repositories
{
    public interface IBikeRepository
    {
        int GetTotalBikes();
        int GetApprovedBikes();
        int GetRejectedBikes();
        int GetPendingBikes();
    }   
}
