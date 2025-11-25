using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto;
using CinephoriaServer.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IImageService _imageService;
        private readonly IRoleService _roleService;

        public AuthController(IAuthService authService, IImageService imageService, IRoleService roleService)
        {
            _authService = authService;
            _imageService = imageService;
            _roleService = roleService;
        }

        /// <summary>
        /// Télécharge une image de profil pour un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="file">Le fichier image à télécharger.</param>
        /// <returns>L'URL de l'image téléchargée ou un message d'erreur.</returns>
        [HttpPost("upload-user-profile/{AppUserId}")]
        public async Task<IActionResult> UploadUserProfile(string AppUserId, [FromForm, Required] IFormFile file)
        {
            try
            {
                string folder = "users";
                var imageUrl = await _imageService.UploadImageAsync(file, folder);
                if (imageUrl == null)
                {
                    return BadRequest(new { Message = "Erreur lors du téléchargement de l'image." });
                }

                var result = await _authService.UpdateProfileImageAsync(AppUserId, imageUrl);
                return Ok(new { Message = result, Url = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors du téléchargement de l'image de profil." });
            }
        }

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpDelete("delete-user-profile-image/{AppUserId}")]
        public async Task<IActionResult> DeleteUserProfileImage(string AppUserId, [FromQuery] string imageUrl)
        {
            try
            {
                var result = await _authService.RemoveProfileImageAsync(AppUserId, imageUrl);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la suppression de l'image de profil." });
            }
        }

        /// <summary>
        /// Crée les rôles par défaut pour les utilisateurs (Admin, Employee, User).
        /// </summary>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpPost("create-default-roles")]
        public async Task<IActionResult> CreateDefaultRoles()
        {
            try
            {
                var result = await _roleService.CreateDefaultRolesAsync();
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la création des rôles par défaut." });
            }
        }

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var result = await _authService.RegisterUserAsync(registerUserDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de l'enregistrement de l'utilisateur." });
            }
        }


        /// <summary>
        /// Crée un compte employé ou administrateur avec un mot de passe temporaire.
        /// </summary>
        /// <param name="createEmployeeDto">Les informations de création ou de l'administrateur du compte employé.</param>
        /// <returns>Un message indiquant si la création a réussi.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var result = await _authService.RegisterEmployeeOrAdminAsync(createEmployeeDto, currentUserRole);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de l'enregistrement de l'employé ou de l'administrateur." });
            }
        }

        /// <summary>
        /// Connecte un utilisateur en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations de connexion de l'utilisateur.</param>
        /// <returns>
        /// Un jeton JWT et le profil de l'utilisateur si la connexion est réussie,
        /// ou un message d'erreur en cas d'échec.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                // Appeler le service d'authentification pour se connecter
                var loginResponse = await _authService.LoginAsync(loginUserDto);

                // Si le token est null, cela signifie qu'une erreur s'est produite (utilisateur non trouvé, mot de passe incorrect, etc.)
                if (loginResponse.Token == null)
                {
                    return BadRequest(new { Message = loginResponse.Profile });
                }

                // Retourner le token et le profil encapsulés dans LoginResponseDto
                return Ok(loginResponse);
            }
            catch (ApiException ex)
            {
                // Gérer les erreurs spécifiques (par exemple, utilisateur non trouvé, mot de passe incorrect, etc.)
                return StatusCode(ex.StatusCode, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Une erreur inattendue s'est produite lors de la connexion." });
            }
        }

        /// <summary>
        /// Récupère la liste de tous les utilisateurs enregistrés.
        /// </summary>
        /// <returns>Une liste d'utilisateurs avec leurs informations.</returns>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération des utilisateurs." });
            }
        }

        /// <summary>
        /// Récupère un utilisateur spécifique par son identifiant.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les informations de l'utilisateur.</returns>
        [HttpGet("users/{AppUserId}")]
        public async Task<IActionResult> GetUserById(string AppUserId)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(AppUserId);
                if (user == null)
                {
                    return NotFound(new { Message = "Utilisateur non trouvé." });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération de l'utilisateur." });
            }
        }

        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé.</returns>
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("employee-profile/{appUserId}")]
        public async Task<IActionResult> GetEmployeeProfile(string appUserId)
        {
            try
            {
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                if (currentUserRole != UserRole.Employee.ToString() && currentUserRole != UserRole.Admin.ToString())
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à accéder à ce profil." });
                }

                var employeeProfile = await _authService.GetEmployeeProfileAsync(appUserId);
                if (employeeProfile == null)
                {
                    return NotFound(new { Message = "Employé non trouvé." });
                }
                return Ok(employeeProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération du profil de l'employé." });
            }
        }

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur.</returns>
        [Authorize]
        [HttpGet("user-profile/{AppUserId}")]
        public async Task<IActionResult> GetUserProfile(string AppUserId)
        {
            try
            {
                var currentAppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentAppUserId != AppUserId)
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à accéder à ce profil." });
                }

                var userProfile = await _authService.GetUserProfileAsync(AppUserId);
                if (userProfile == null)
                {
                    return NotFound(new { Message = "Utilisateur non trouvé." });
                }
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la récupération du profil utilisateur." });
            }
        }

        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [Authorize]
        [HttpPut("update-profile/{AppUserId}")]
        public async Task<IActionResult> UpdateUserProfile(string AppUserId, [FromBody] UpdateAppUserDto updateAppUserDto)
        {
            try
            {
                var currentAppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentAppUserId != AppUserId)
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à mettre à jour ce profil." });
                }

                var result = await _authService.UpdateUserProfileAsync(AppUserId, updateAppUserDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour du profil utilisateur." });
            }
        }

        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [Authorize(Roles = "Employee,Admin")]
        [HttpPut("update-employee-profile/{appUserId}")]
        public async Task<IActionResult> UpdateEmployeeProfile(string appUserId, [FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var currentAppUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentAppUserId != appUserId && !User.IsInRole("Admin"))
                {
                    return StatusCode(403, new { Message = "Vous n'êtes pas autorisé à mettre à jour ce profil." });
                }

                var result = await _authService.UpdateEmployeeProfileAsync(appUserId, updateEmployeeDto);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la mise à jour du profil employé." });
            }
        }

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string AppUserId, string token)
        {
            try
            {
                var result = await _authService.ConfirmEmailAsync(AppUserId, token);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la confirmation de l'email." });
            }
        }

        /// <summary>
        /// Renvoie l'email de confirmation à un utilisateur.
        /// </summary>
        /// <param name="request">La demande de renvoi d'email de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationRequest request)
        {
            try
            {
                var result = await _authService.ResendConfirmationEmailAsync(request.Email);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors du renvoi de l'email de confirmation." });
            }
        }


        // Gestion de changement de mot-de-passe

        /// <summary>
        /// Demande de réinitialisation de mot de passe pour un utilisateur normal.
        /// </summary>
        /// <param name="request">Les informations de demande de réinitialisation (e-mail).</param>
        /// <returns>Un message indiquant si la demande a été traitée avec succès.</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] RequestPasswordResetDto request)
        {
            try
            {
                var result = await _authService.ForgotPasswordAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {Message = "Une erreur s'est produite lors de la demande de réinitialisation de mot de passe " });
            }
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="resetPasswordDto">Les informations de réinitialisation (token et nouveau mot de passe).</param>
        /// <returns>Un message indiquant si la réinitialisation a réussi.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var result = await _authService.ResetPasswordAsync(resetPasswordDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation du mot de passe" });
            }
        }

        /// <summary>
        /// Valide un jeton de réinitialisation de mot de passe.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de réinitialisation.</param>
        /// <returns>Un message indiquant si le jeton est valide.</returns>
        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string AppUserId, [FromQuery] string token)
        {
            try
            {
                var result = await _authService.ValidateResetTokenAsync(AppUserId, token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la validation du jeton" });
                
            }
        }

        /// <summary>
        /// Force la réinitialisation du mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        [HttpPost("force-password-reset")]
        public async Task<IActionResult> ForcePasswordReset([FromQuery] string AppUserId)
        {
            try
            {
                var result = await _authService.ForcePasswordResetAsync(AppUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation forcée du mot de passe" });
            }
        }


        /// <summary>
        /// Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un résultat HTTP contenant un message de réussite ou d'échec.</returns>
        [HttpPost("change-employee-password")]
        public async Task<IActionResult> ChangeEmployeePassword([FromBody] ChangeEmployeePasswordDto changePasswordDto)
        {
            try
            {
                // Vérifier la validité du modèle
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }

                // Appeler le service pour changer le mot de passe
                var result = await _authService.ChangeEmployeePasswordAsync(changePasswordDto);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors du changement de mot de passe." });
            }
        }

        /// <summary>
        /// Permet à un utilisateur connecté de changer son mot de passe.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un résultat HTTP contenant un message de réussite ou d'échec.</returns>
        [HttpPost("change-password")]
        [Authorize] // Assurez-vous que seul un utilisateur authentifié peut accéder à cette route
        public async Task<IActionResult> ChangeUserPassword([FromBody] ChangeUserPasswordDto changePasswordDto)
        {
            try
            {
                // Vérifier la validité du modèle
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }

                // Récupérer l'ID de l'utilisateur connecté depuis le contexte d'authentification
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Appeler le service pour changer le mot de passe
                var result = await _authService.ChangeUserPasswordAsync(userId, changePasswordDto);

                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors du changement de mot de passe." });
            }
        }

        /// <summary>
        /// Force un employé à changer son mot de passe (par exemple, si le mot de passe temporaire a expiré).
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'employé.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        [HttpPost("force-password-change")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForceEmployeePasswordChange([FromQuery] string AppUserId)
        {
            try
            {
                var result = await _authService.ForceEmployeePasswordChangeAsync(AppUserId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de la réinitialisation forcée du mot de passe : " });
            }
        }

        /// <summary>
        /// Permet à un utilisateur d'envoyer un message de contact par email.
        /// </summary>
        /// <param name="request">Les informations du message de contact.</param>
        /// <returns>Un résultat HTTP contenant un message de succès ou d'échec.</returns>
        [HttpPost("send-contact")]
        public async Task<IActionResult> SendContactEmail([FromBody] ContactRequest request)
        {
            try
            {
                // Vérifier la validité du modèle
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new { Errors = errors });
                }

                // Vérifier si les champs obligatoires sont bien remplis
                if (string.IsNullOrEmpty(request.Email) ||
                    string.IsNullOrEmpty(request.Title) ||
                    string.IsNullOrEmpty(request.Description))
                {
                    return BadRequest(new { Message = "L'email, le titre et la description sont obligatoires." });
                }

                // Envoyer l'email via le service approprié
                
                await _authService.SendContactByEmail(request.Username, request.Title, request.Description, request.Email);

                return Ok(new { Message = "Votre message a été envoyé avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur s'est produite lors de l'envoi du message." });
            }
        }

        /// <summary>
        /// Supprime un utilisateur en fonction des autorisations de l'utilisateur connecté.
        /// </summary>
        /// <param name="targetUserId">L'identifiant de l'utilisateur à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        [HttpDelete("users/{targetUserId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string targetUserId)
        {
            try
            {
                // Récupérer l'ID et le rôle de l'utilisateur actuellement connecté
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(currentUserRole))
                {
                    return Unauthorized(new { Message = "Utilisateur non authentifié." });
                }

                // Appeler le service pour supprimer l'utilisateur
                var result = await _authService.DeleteUserAsync(currentUserId, currentUserRole, targetUserId);
                
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Une erreur inattendue s'est produite lors de la suppression de l'utilisateur." });
            }
        }

    }

}

