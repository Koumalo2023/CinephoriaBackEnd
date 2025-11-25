using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.API.Services
{
    public interface IRoleService
    {
        /// <summary>
        /// Crée les rôles par défaut pour les utilisateurs (Admin, Employee, User).
        /// </summary>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> CreateDefaultRolesAsync();
        Task<bool> RoleExistsAsync(string roleName);
        Task<string> AssignRoleToUserAsync(AppUser user, EnumConfig.UserRole role);
    }

    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Crée les rôles par défaut pour les utilisateurs (Admin, Employee, User).
        /// </summary>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> CreateDefaultRolesAsync()
        {
            var rolesToCreate = Enum.GetNames(typeof(EnumConfig.UserRole));

            foreach (var roleName in rolesToCreate)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        return $"Erreur lors de la création du rôle {roleName}.";
                    }
                }
            }

            return "Les rôles ont été créés avec succès.";
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<string> AssignRoleToUserAsync(AppUser user, EnumConfig.UserRole role)
        {
            var roleName = role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return $"Le rôle {roleName} n'existe pas.";
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return $"Erreur lors de l'assignation du rôle {roleName}.";
            }

            return $"Le rôle {roleName} a été assigné avec succès.";
        }
    }
}