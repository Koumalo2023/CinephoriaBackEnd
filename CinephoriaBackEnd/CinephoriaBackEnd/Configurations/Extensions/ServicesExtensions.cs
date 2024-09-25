using CinephoriaBackEnd.Repository;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaBackEnd.Configurations
{
    public static class ServicesExtensions
    {
        public static void AddDbServiceInjection(this IServiceCollection services)
        {
            // Pour accéder au contexte HTTP si nécessaire
            services.AddHttpContextAccessor();

            // Ajouter la gestion des utilisateurs et des connexions pour utilisateurs
            //services.AddTransient<UserManager<AppUser>>();
            //services.AddTransient<SignInManager<AppUser>>();

            // Injection du UoW (Unit of Work) pour Entity Framework
            services.AddTransient<IUnitOfWorkPostgres, UnitOfWorkPostgres>();
            services.AddTransient<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();

            // Injection des répository pour maintenir la testabilité et la maintenabilité du code
            services.AddTransient<IIncidentRepository, IncidentRepository>();
            services.AddTransient<IShowtimeRepository, ShowtimeRepository>();
            services.AddTransient<IMovieRepository, MovieRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
        }
    }

}
