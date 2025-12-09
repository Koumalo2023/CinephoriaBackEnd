
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto;
using Microsoft.AspNetCore.Identity;

namespace CinephoriaServer.API.Services
{
    public interface IAuthService
    {
        // Gestion des utilisateurs

        /// <summary>
        /// Met à jour l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de la nouvelle image de profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateProfileImageAsync(string AppUserId, string imageUrl);

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image de profil à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RemoveProfileImageAsync(string AppUserId, string imageUrl);

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RegisterUserAsync(RegisterUserDto registerUserDto);

        /// <summary>
        /// Enregistre un nouvel employé ou administrateur.
        /// </summary>
        /// <param name="createEmployeeDto">Les données de l'employé ou de l'administrateur à enregistrer.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuel.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> RegisterEmployeeOrAdminAsync(CreateEmployeeDto createEmployeeDto, string currentUserRole);

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> ConfirmEmailAsync(string AppUserId, string token);


        /// <summary>
        /// Permet à un utilisateur de se connecter en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations de connexion de l'utilisateur.</param>
        /// <returns>Un jeton JWT et le profil sont retournés si la connexion est réussie, ou un message d'erreur.</returns>
        Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto);

        /// <summary>
        /// Récupère la liste de tous les utilisateurs enregistrés.
        /// </summary>
        /// <returns>Une liste d'utilisateurs avec leurs informations.</returns>
        Task<List<AppUserDto>> GetAllUsersAsync();

        /// <summary>
        /// Récupère la liste des utilisateurs avec filtrage, pagination et tri.
        /// </summary>
        /// <param name="role">Filtre par rôle (optionnel).</param>
        /// <param name="page">Numéro de page (défaut 1).</param>
        /// <param name="pageSize">Taille de la page (défaut 10).</param>
        /// <param name="sortBy">Champ de tri (optionnel).</param>
        /// <param name="sortOrder">Ordre de tri ("asc" ou "desc", défaut "asc").</param>
        /// <returns>Tuple contenant la liste des utilisateurs et le nombre total.</returns>
        Task<(List<AppUserDto> Users, int TotalCount)> GetUsersFilteredAsync(string? role = null, int page = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = "asc");

        /// <summary>
        /// Récupère un utilisateur spécifique par son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les informations de l'utilisateur.</returns>
        Task<AppUserDto> GetUserByIdAsync(string AppUserId);

        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateUserProfileAsync(string AppUserId, UpdateAppUserDto updateAppUserDto);

        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> UpdateEmployeeProfileAsync(string appUserId, UpdateEmployeeDto updateEmployeeDto);

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        Task<UserProfileDto> GetUserProfileAsync(string AppUserId);

        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les réservations de l'utilisateur.</returns>
        Task<List<ReservationDto>> GetUserOrdersAsync(string AppUserId);

        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé.</returns>
        Task<EmployeeProfileDto> GetEmployeeProfileAsync(string appUserId);


        /// Gestion des mot de passe(Demande de changement & Réinitialisation)
        /// <summary>
        /// Demande de réinitialisation de mot de passe pour un utilisateur normal.
        /// </summary>
        /// <param name="request">Les informations de demande de réinitialisation (e-mail).</param>
        /// <returns>Un message indiquant si la demande a été traitée avec succès.</returns>
        Task<string> ForgotPasswordAsync(RequestPasswordResetDto request);

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="resetPasswordDto">Les informations de réinitialisation (token et nouveau mot de passe).</param>
        /// <returns>Un message indiquant si la réinitialisation a réussi.</returns>
        Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// Valide un jeton de réinitialisation de mot de passe.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de réinitialisation.</param>
        /// <returns>Un message indiquant si le jeton est valide.</returns>
        Task<string> ValidateResetTokenAsync(string AppUserId, string token);

        /// <summary>
        /// Force la réinitialisation du mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        Task<string> ForcePasswordResetAsync(string AppUserId);

        /// <summary>
        /// Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        Task<string> ForceEmployeePasswordChangeAsync(string AppUserId);
        
        /// <summary>
        /// Force un employé à changer son mot de passe (par exemple, si le mot de passe temporaire a expiré).
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'employé.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        Task<string> ChangeEmployeePasswordAsync(ChangeEmployeePasswordDto changePasswordDto);

        /// <summary>
        /// Permet à un utilisateur connecté de changer son mot de passe.
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur connecté.</param>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        Task<string> ChangeUserPasswordAsync(string userId, ChangeUserPasswordDto changePasswordDto);

        /// <summary>
        /// Supprime un utilisateur en fonction de son rôle et des autorisations.
        /// </summary>
        /// <param name="currentUserId">L'ID de l'utilisateur actuellement connecté.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuellement connecté.</param>
        /// <param name="targetUserId">L'ID de l'utilisateur à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> DeleteUserAsync(string currentUserId, string currentUserRole, string targetUserId);

        Task SendContactByEmail(string username, string title, string description, string fromEmail);

        /// <summary>
        /// Renvoie l'email de confirmation à un utilisateur.
        /// </summary>
        /// <param name="email">L'adresse email de l'utilisateur.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        Task<string> ResendConfirmationEmailAsync(string email);
    }
}
