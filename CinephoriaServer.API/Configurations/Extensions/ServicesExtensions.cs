using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Repository;
using CinephoriaServer.API.Services;
using CinephoriaServer.API.Services.FavoriteMovie;
using CinephoriaServer.API.Services.Notification;
using CinephoriaServer.API.Services.Settings; 
using DnsClient;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.API.Configurations
{
    public static class ServicesExtensions
    {
        public static void AddDbServiceInjection(this IServiceCollection services)
        {
            // Pour accéder au contexte HTTP si nécessaire
            services.AddHttpContextAccessor();

            // Ajouter la gestion des utilisateurs et des connexions pour utilisateurs
            services.AddTransient<UserManager<AppUser>>();
            services.AddTransient<SignInManager<AppUser>>();

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<EmailService>();
            services.AddTransient<QRCodeService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IReservationReminderService, ReservationReminderService>();
            services.AddTransient<ReservationReminderService>();
            services.AddTransient<ReservationExpirationService>();
            services.AddTransient<IShowtimeStatusService, ShowtimeStatusService>();
            services.AddTransient<ShowtimeStatusService>();


            // injection des Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<ISeatService, SeatService>();
            services.AddTransient<IIncidentService, IncidentService>();
            services.AddTransient<ITheaterService, TheaterService>();
            services.AddTransient<ICinemaService, CinemaService>();
            services.AddTransient<IMovieService, MovieService>(); 
            services.AddTransient<IMovieRatingService, MovieRatingService>();
            services.AddTransient<IShowtimeService, ShowtimeService>();
            services.AddTransient<IReservationService, ReservationService>();
            services.AddTransient<IAdminDashboardService, AdminDashboardService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IFavoriteMovieService, FavoriteMovieService>();
            services.AddTransient<IEmployeeFavoriteService, EmployeeFavoriteService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<INotificationService, NotificationService>();


            // Injection du UoW (Unit of Work) pour Entity Framework
            services.AddTransient<IUnitOfWorkPostgres, UnitOfWorkPostgres>();
            services.AddTransient<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();

            // MongoDB Repositories
            

            // PostgreSQL Repositories
            services.AddTransient<IAdminDashboardRepository, AdminDashboardRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IEmployeeFavoriteRepository, EmployeeFavoriteRepository>();
            services.AddTransient<IUserMovieHistoryRepository, UserMovieHistoryRepository>();
            services.AddTransient<ISettingsRepository, SettingsRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();



        }
    }

}
