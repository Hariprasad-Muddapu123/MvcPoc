using BikeBuddy.Notification;
using BikeBuddy.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace BikeBuddy.Services
{
    public class BikeAvailabilityService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
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
                await Notification();
                await Task.Delay(_interval, stoppingToken);
            }
        }
        private async Task UpdateBikeAvailability()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var bikeRepository = scope.ServiceProvider.GetRequiredService<IBikeRepository>();
                var rideRepository = scope.ServiceProvider.GetRequiredService<IRideRepository>();
                var bikes = await bikeRepository.GetAllAsync(); // Get all bikes
                var ongoingRides = await rideRepository.GetAllOngoingRidesAsync();

                foreach (var bike in bikes)
                {
                    if (bike.AvailableUpto <= DateTime.Now && bike.Available == true)
                    {
                        bike.Available = false;
                        
                    }
                    var bikeInOngoingRide = ongoingRides.Any(ride =>
                        ride.BikeId == bike.BikeId &&
                        DateTime.TryParse(ride.PickupDateTime, out var pickupDateTime) &&
                        DateTime.TryParse(ride.DropoffDateTime, out var dropoffDateTime) &&
                        pickupDateTime <= DateTime.Now &&
                        dropoffDateTime >= DateTime.Now);

                    if (bikeInOngoingRide)
                    {
                        bike.Available = false;
                    }
                    else
                    {
                        bike.Available = true;
                    }
                    await bikeRepository.UpdateAsync(bike);
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
                    DateTime.TryParse(ride.DropoffDateTime, out var dropoffDateTime);
                    if (dropoffDateTime<= DateTime.Now && ride.RentalStatus == RentStatus.Ongoing)
                    {
                        ride.RentalStatus = RentStatus.Completed;
                        await rideRepository.UpdateAsync(ride);
                    }
                }
            }
        }

        private async Task Notification()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var rideRepository = scope.ServiceProvider.GetRequiredService<IRideRepository>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                var ongoingRides = await rideRepository.GetAllRidesAsync();
                var currentTime = DateTime.Now;


                foreach (var ride in ongoingRides)
                {
                    DateTime.TryParse(ride.PickupDateTime, out var pickupDateTime);
                    DateTime.TryParse(ride.DropoffDateTime, out var dropoffDateTime);
                    var pickupTimeDifference = pickupDateTime - currentTime;

                    if (pickupTimeDifference.TotalMinutes <=30 && pickupTimeDifference.TotalMinutes >= 29)
                    {
                        await hubContext.Clients.User(ride.UserId.ToString())
                            .SendAsync("ReceiveNotification", $"Your ride for the bike '{ride.RideId}' is scheduled to begin in 30 minutes. Please be prepared.");

                        Console.WriteLine($"Notification sent for ride: {ride.RideId} to user: {ride.UserId}");
                    }

                    TimeSpan dropOffTimeDifference = dropoffDateTime - DateTime.Now;
                    if (dropOffTimeDifference.TotalMinutes <= 10 && dropOffTimeDifference.TotalMinutes > 9)
                    {
                        await hubContext.Clients.User(ride.UserId.ToString())
                            .SendAsync("ReceiveNotification", $"Your ride for the bike '{ride.RideId}' is scheduled to end in 10 minutes. Please be ready to submit the bike.");

                        Console.WriteLine($"Notification sent for bike submission for ride: {ride.RideId} to user: {ride.UserId}");
                    }

                }
            }
        }


    }
}


