namespace BikeBuddy.Services
{
    public class BikeAvailabilityService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(100);
        public BikeAvailabilityService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateBikeAvailability();
                await UpdateRideStatus();
                await Task.Delay(_interval, stoppingToken);
            }
        }
        private async Task UpdateBikeAvailability()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bikeRepository = scope.ServiceProvider.GetRequiredService<IBikeRepository>();
                var bikes = await bikeRepository.GetAllAsync(); // Get all bikes

                foreach (var bike in bikes)
                {
                    if (bike.AvailableUpto <= DateTime.Now && bike.Available == true)
                    {
                        bike.Available = false;
                        await bikeRepository.UpdateAsync(bike);
                    }
                }
            }
        }

        private async Task UpdateRideStatus()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var rideRepository = scope.ServiceProvider.GetRequiredService<IRideRepository>();
                var rides = await rideRepository.GetAllAsync();

                foreach (var ride in rides)
                {
                    if (Convert.ToDateTime(ride.DropoffDateTime) <= DateTime.Now && ride.RentalStatus == RentStatus.Ongoing)
                    {
                        ride.RentalStatus = RentStatus.Completed;
                        await rideRepository.UpdateAsync(ride);
                    }
                }
            }
        }
    }
}
