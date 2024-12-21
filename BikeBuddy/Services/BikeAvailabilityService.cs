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
            og}
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
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Get access to the database


                var ongoingRides = await rideRepository.GetAllRidesAsync();
                var currentTime = DateTime.Now;


                foreach (var ride in ongoingRides)
                {
                    DateTime.TryParse(ride.PickupDateTime, out var pickupDateTime);
                    DateTime.TryParse(ride.DropoffDateTime, out var dropoffDateTime);
                    var pickupTimeDifference = pickupDateTime - currentTime;

                    if (pickupTimeDifference.TotalMinutes <=30) //&& pickupTimeDifference.TotalMinutes >= 29)
                    {
                        if (NotificationHub.IsUserOnline(ride.UserId.ToString()))
                        {
                            await hubContext.Clients.User(ride.UserId.ToString())
                                .SendAsync("ReceiveNotification", $"Your ride for bike '{ride.RideId}' starts in 30 minutes.");
                        }
                        else
                        {
                            await StoreNotification(ride.UserId.ToString(), $"Your ride for bike '{ride.RideId}' starts in 30 minutes.");
                        }
                    }
                    

                    TimeSpan dropOffTimeDifference = dropoffDateTime - DateTime.Now;
                    if (dropOffTimeDifference.TotalMinutes <= 10000)// && dropOffTimeDifference.TotalMinutes > 9)
                    {
                        if (NotificationHub.IsUserOnline(ride.UserId.ToString()))
                        {
                            await hubContext.Clients.User(ride.UserId.ToString())
                                .SendAsync("ReceiveNotification", $"Your ride for bike '{ride.RideId}' ends in 10 minutes.");
                        }
                        else
                        {
                            await StoreNotification(ride.UserId.ToString(), $"Your ride for bike '{ride.RideId}' ends in 10 minutes.");
                        }
                    }

                }
            }
        }

        // Store notification in the database if the user is offline
        private async Task StoreNotification(string userId, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>(); // Replace ApplicationUser with your User class name

            // Retrieve the user's email using the UserManager
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }

            var notification = new Models.Notification
            {
                UserId = userId,
                UserEmail = user.Email, // Use the email from the user object
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            dbContext.Notifications.Add(notification);
            await dbContext.SaveChangesAsync();
        }


    }
}


