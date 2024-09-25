using CinephoriaBackEnd.Repository;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaBackEnd.Configurations
{
    public static class ServicesExtensions
    {
        public static void AddDbServiceInjection(this IServiceCollection services)
        {
            services.AddHttpContextAccessor(); // Pour accéder au contexte HTTP si nécessaire

            // Ajouter la gestion des utilisateurs et des connexions pour MenapUser
            //services.AddTransient<UserManager<MenapUser>>();
            //services.AddTransient<SignInManager<MenapUser>>();

            // Injection du UoW (Unit of Work) pour Entity Framework
            services.AddTransient<IUnitOfWorkPostgres, UnitOfWorkPostgres>();
            services.AddTransient<IUnitOfWorkMongoDb, UnitOfWorkMongoDb>();

            // Vous pouvez ajouter des services supplémentaires ici
        }
    }

}
