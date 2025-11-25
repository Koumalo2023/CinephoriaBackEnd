
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.API.Configurations
{
    public class SeedAdmin
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Vérifiez et créez les rôles s'ils n'existent pas
            string[] roles = { "Admin", "Employee", "User" };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Vérifiez si l'administrateur existe déjà
            if (await userManager.FindByNameAsync("admin@cinephoria.com") == null)
            {
                // Créez l'administrateur
                var adminUser = new AppUser
                {
                    UserName = "admin@cinephoria.com",
                    Email = "admin@cinephoria.com",
                    FirstName = "Admin",
                    LastName = "Cinephoria",
                    Position = "Directeur",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    HiredDate = DateTime.UtcNow,
                    PhoneNumber = "062598631457",
                    EmailConfirmed = true,
                    Role = EnumConfig.UserRole.Admin
                };

                // Créez l'utilisateur avec un mot de passe par défaut
                var result = await userManager.CreateAsync(adminUser, "Admin-Cine-123");

                // Si la création réussit, assignez le rôle Admin
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Vérifiez si l'employé existe déjà
            if (await userManager.FindByNameAsync("employee@cinephoria.com") == null)
            {
                // Créez l'employé
                var employeeUser = new AppUser
                {
                    UserName = "employee@cinephoria.com",
                    Email = "employee@cinephoria.com",
                    FirstName = "Employé",
                    LastName = "Cinephoria",
                    Position = "Vendeur",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    HiredDate = DateTime.UtcNow,
                    PhoneNumber = "062598631458",
                    EmailConfirmed = true,
                    Role = EnumConfig.UserRole.Employee
                };

                // Créez l'utilisateur avec le mot de passe spécifié
                var result = await userManager.CreateAsync(employeeUser, "Employee-Cine-123");

                // Si la création réussit, assignez le rôle Employee
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser, "Employee");
                }
            }

            // Vérifiez si l'utilisateur existe déjà
            if (await userManager.FindByNameAsync("user@exemple.com") == null)
            {
                // Créez l'utilisateur
                var user = new AppUser
                {
                    UserName = "user@exemple.com",
                    Email = "user@exemple.com",
                    FirstName = "Utilisateur",
                    LastName = "Standard",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "062598631459",
                    EmailConfirmed = true,
                    HasApprovedTermsOfUse = true,
                    Role = EnumConfig.UserRole.User
                };

                // Créez l'utilisateur avec le mot de passe spécifié
                var result = await userManager.CreateAsync(user, "User-Cine-123");

                // Si la création réussit, assignez le rôle User
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }

}
