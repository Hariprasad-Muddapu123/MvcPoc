namespace BikeBuddy.Services
{
    public class RideNotificationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RideNotificationService> _logger;

        public RideNotificationService(IServiceProvider serviceProvider, ILogger<RideNotificationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                    var currentTime = DateTime.UtcNow;

                    // Fetch ongoing rides in a single query
                    var upcomingOngoingRides = await dbContext.Rides
                        .Include(r => r.User)
                        .Include(r => r.Bike)
                        .ToListAsync(stoppingToken);

                    foreach (var ride in upcomingOngoingRides)
                    {
                        DateTime.TryParse(ride.PickupDateTime, out var pickupDateTime);
                        DateTime.TryParse(ride.DropoffDateTime, out var dropoffDateTime);

                        var timeDifference = dropoffDateTime - currentTime;

                        
                        if (timeDifference == TimeSpan.FromMinutes(10))
                        {
                            // 10 minutes before ride completion
                            await notificationRepository.AddNotificationAsync(new Notification
                            {
                                UserId = ride.UserId,
                                UserEmail=ride.User.Email,
                                Message = $"Your ride for the bike '{ride.Bike.BikeModel}' is about to end in 10 minutes. Please prepare to return the bike.",
                                CreatedAt = DateTime.UtcNow
                            });

                            _logger.LogInformation($"Notification sent for upcoming ride end in 10 minutes: {ride.RideId}");
                        }
                        else if ((pickupDateTime - currentTime).TotalMinutes ==30)
                        {
                            // Time to collect the bike
                            await notificationRepository.AddNotificationAsync(new Notification
                            {
                                UserId = ride.UserId,
                                UserEmail=ride.User.Email,
                                Message = $"Your bike '{ride.Bike.BikeModel}' is ready for collection. Please collect it within 30 minutes.",
                                CreatedAt = DateTime.UtcNow
                            });

                            _logger.LogInformation($"Notification sent for upcoming pickup: {ride.RideId}");
                        }
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Check every 10 minutes
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in RideNotificationService: {ex}");
                }
            }
        }
    }

}
