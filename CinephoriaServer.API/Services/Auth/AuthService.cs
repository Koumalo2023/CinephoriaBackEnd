using AutoMapper;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models.PostgresqlDb;
using CinephoriaServer.API.Models.PostgresqlDb.Auth.AppUserDto;
using CinephoriaServer.API.Repository; 
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static CinephoriaServer.API.Configurations.EnumConfig;

namespace CinephoriaServer.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWorkPostgres _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IImageService _imageService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, IEmailService emailService, IMapper mapper, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IImageService imageService, ILogger<AuthService> logger, IUnitOfWorkPostgres unitOfWork, IRoleService roleService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _imageService = imageService;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _roleService = roleService;
        }

        /// <summary>
        /// Met à jour l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de la nouvelle image de profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateProfileImageAsync(string AppUserId, string imageUrl)
        {
            // Récupérer l'utilisateur existant
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Mettre à jour l'URL de l'image de profil
            user.ProfilePictureUrl = imageUrl;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour de l'image de profil.";
            }

            _logger.LogInformation("Image de profil mise à jour avec succès pour l'utilisateur avec l'ID {AppUserId}.", AppUserId);
            return "Image de profil mise à jour avec succès.";
        }

        /// <summary>
        /// Supprime l'image de profil d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="imageUrl">L'URL de l'image de profil à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RemoveProfileImageAsync(string AppUserId, string imageUrl)
        {
            // Récupérer l'utilisateur existant
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Supprimer l'image du stockage
            var imageDeleted = await _imageService.DeleteImageAsync(imageUrl);
            if (!imageDeleted)
            {
                return "L'image de profil n'a pas pu être supprimée du stockage.";
            }

            // Supprimer l'URL de l'image de profil de l'utilisateur
            user.ProfilePictureUrl = null;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la suppression de l'image de profil.";
            }

            _logger.LogInformation("Image de profil supprimée avec succès pour l'utilisateur avec l'ID {AppUserId}.", AppUserId);
            return "Image de profil supprimée avec succès.";
        }

        /// <summary>
        /// Enregistre un nouvel utilisateur.
        /// </summary>
        /// <param name="registerUserDto">Les données de l'utilisateur à enregistrer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // Validation de base
            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return "Les mots de passe sont différents.";
            }

            // Vérifie si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(registerUserDto.Email);
            if (existingUser != null)
            {
                return "L'utilisateur existe déjà.";
            }

            // Création de l'utilisateur
            var newUser = new AppUser
            {
                UserName = registerUserDto.Email,
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                HasApprovedTermsOfUse = false
            };

            // Créer l'utilisateur
            var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                return "Erreur lors de la création de l'utilisateur: " + string.Join(" ", createUserResult.Errors.Select(e => e.Description));
            }

            // Assigner le rôle à l'utilisateur en utilisant RoleService
            var roleResult = await _roleService.AssignRoleToUserAsync(newUser, EnumConfig.UserRole.User);
            if (!roleResult.Contains("succès"))
            {
                // Si l'assignation du rôle échoue, supprimer l'utilisateur
                await _userManager.DeleteAsync(newUser);
                return roleResult;
            }

            // Envoyer l'email de confirmation
            await SendConfirmationEmailAsync(newUser);

            return "Utilisateur créé avec succès. Un email de confirmation a été envoyé.";
        }

        /// <summary>
        /// Enregistre un nouvel employé ou administrateur.
        /// </summary>
        /// <param name="createEmployeeDto">Les données de l'employé ou de l'administrateur à enregistrer.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuel.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> RegisterEmployeeOrAdminAsync(CreateEmployeeDto createEmployeeDto, string currentUserRole)
        {

            // Vérifie si l'utilisateur existe déjà
            var existingUser = await _userManager.FindByEmailAsync(createEmployeeDto.Email);
            if (existingUser != null)
            {
                return "L'utilisateur existe déjà.";
            }

            // Vérifie que l'utilisateur actuel est un administrateur
            if (currentUserRole != UserRole.Admin.ToString())
            {
                return "Seul un administrateur peut créer un autre administrateur ou un employé.";
            }

            // Vérifie que l'adresse email se termine par "@cinephoria.com"
            if (!createEmployeeDto.Email.EndsWith("@cinephoria.com"))
            {
                return "Les comptes administrateurs et employés doivent utiliser une adresse email se terminant par '@cinephoria.com'.";
            }

            // Validation supplémentaire pour les employés
            if (createEmployeeDto.Role == UserRole.Employee)
            {
                if (string.IsNullOrEmpty(createEmployeeDto.PhoneNumber) ||
                    !createEmployeeDto.HiredDate.HasValue ||
                    string.IsNullOrEmpty(createEmployeeDto.Position))
                {
                    return "Le numéro de téléphone, la date d'embauche et le poste de l'employé sont obligatoires.";
                }
            }

            // Générer un mot de passe temporaire
            var temporaryPassword = GenerateTemporaryPassword();

            // Création de l'utilisateur
            var newUser = new AppUser
            {
                UserName = createEmployeeDto.Email,
                Email = createEmployeeDto.Email,
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                PhoneNumber = createEmployeeDto.PhoneNumber,
                HiredDate = createEmployeeDto.HiredDate,
                Position = createEmployeeDto.Position,
                ProfilePictureUrl = createEmployeeDto.ProfilePictureUrl,
                Role = createEmployeeDto.Role
            };

            // Créer l'utilisateur avec le mot de passe temporaire
            var createUserResult = await _userManager.CreateAsync(newUser, temporaryPassword);
            if (!createUserResult.Succeeded)
            {
                return "Erreur lors de la création de l'utilisateur "; 
                
            }

            // Assigner le rôle "Employee" à l'utilisateur
            await _userManager.AddToRoleAsync(newUser, UserRole.Employee.ToString());

            // Envoyer l'e-mail avec le mot de passe temporaire et le lien pour changer le mot de passe
            await SendEmployeePasswordResetEmailAsync(newUser, temporaryPassword);

            return "Compte employé créé avec succès. Un e-mail a été envoyé avec un mot de passe temporaire.";
        }

        /// <summary>
        /// Connecte un utilisateur en vérifiant ses informations d'identification.
        /// </summary>
        /// <param name="loginUserDto">Les informations d'identification de l'utilisateur.</param>
        /// <returns>Un jeton JWT en cas de succès, ou un message d'erreur.</returns>
        public async Task<LoginResponseDto> LoginAsync(LoginUserDto loginUserDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null)
            {
                return new LoginResponseDto { Token = null, Profile = "Utilisateur non trouvé." };
            }

            // Vérifier si le mot de passe est correct
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!isPasswordValid)
            {
                return new LoginResponseDto { Token = null, Profile = "Mot de passe incorrect." };
            }

            // Vérifier si l'email est confirmé
            if (!user.EmailConfirmed)
            {
                return new LoginResponseDto { Token = null, Profile = "Veuillez confirmer votre adresse email avant de vous connecter." };
            }

            // Récupérer le rôle de l'utilisateur
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            // Générer le token JWT
            var token = GenerateJwtToken(user);

            // Créer le profil en fonction du rôle
            object profile = null;
            if (role == "User")
            {
                var userProfile = _mapper.Map<UserProfileDto>(user);
                userProfile.Role = role; // Ajouter le rôle au profil utilisateur
                profile = userProfile;
            }
            else if (role == "Employee" || role == "Admin")
            {
                var employeeProfile = _mapper.Map<EmployeeProfileDto>(user);
                employeeProfile.Role = role; // Ajouter le rôle au profil employé
                profile = employeeProfile;
            }

            // Retourner la réponse avec le token et le profil
            return new LoginResponseDto
            {
                Token = token,
                Profile = profile
            };
        }

        // Gestion des mot de passe(Demande de changement & Réinitialisation)

        /// <summary>
        /// Demande de réinitialisation de mot de passe pour un utilisateur normal.
        /// </summary>
        /// <param name="request">Les informations de demande de réinitialisation (e-mail).</param>
        /// <returns>Un message indiquant si la demande a été traitée avec succès.</returns>
        public async Task<string> ForgotPasswordAsync(RequestPasswordResetDto request)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Ne pas révéler que l'utilisateur n'existe pas pour des raisons de sécurité
                return "Si l'e-mail existe, un lien de réinitialisation sera envoyé.";
            }

            // Générer un jeton de réinitialisation de mot de passe
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Envoyer l'e-mail de réinitialisation
            await SendResetPasswordEmailAsync(user, resetToken);

            return "Un lien de réinitialisation a été envoyé à votre adresse e-mail.";
        }

        /// <summary>
        /// Réinitialise le mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="resetPasswordDto">Les informations de réinitialisation (token et nouveau mot de passe).</param>
        /// <returns>Un message indiquant si la réinitialisation a réussi.</returns>
        public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Réinitialiser le mot de passe
            var resetResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!resetResult.Succeeded)
            {
                return "Erreur lors de la réinitialisation du mot de passe ";
            }

            return "Votre mot de passe a été réinitialisé avec succès.";
        }

        /// <summary>
        /// Valide un jeton de réinitialisation de mot de passe.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de réinitialisation.</param>
        /// <returns>Un message indiquant si le jeton est valide.</returns>
        public async Task<string> ValidateResetTokenAsync(string AppUserId, string token)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si le jeton est valide
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            if (!isValidToken)
            {
                return "Le jeton de réinitialisation est invalide ou a expiré.";
            }

            return "Le jeton de réinitialisation est valide.";
        }

        /// <summary>
        /// Force la réinitialisation du mot de passe d'un utilisateur normal.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        public async Task<string> ForcePasswordResetAsync(string AppUserId)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Générer un nouveau jeton de réinitialisation
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Envoyer l'e-mail de réinitialisation
            await SendResetPasswordEmailAsync(user, resetToken);

            return "Un nouveau lien de réinitialisation a été envoyé à votre adresse e-mail.";
        }

        /// <summary>
        /// Permet à un employé de changer son mot de passe après avoir utilisé un mot de passe temporaire.
        /// </summary>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        public async Task<string> ChangeEmployeePasswordAsync(ChangeEmployeePasswordDto changePasswordDto)
        {
            // Vérifier si les nouveaux mots de passe correspondent
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return "Les mots de passe ne correspondent pas.";
            }

            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(changePasswordDto.AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si l'ancien mot de passe (temporaire) est correct
            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.OldPassword);
            if (!isOldPasswordValid)
            {
                return "L'ancien mot de passe est incorrect.";
            }

            user.EmailConfirmed = true;

            // Changer le mot de passe
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                return $"Erreur lors du changement de mot de passe : {errors}";
            }

            return "Votre mot de passe a été changé avec succès.";
        }

        /// <summary>
        /// Permet à un utilisateur connecté de changer son mot de passe.
        /// </summary>
        /// <param name="userId">L'ID de l'utilisateur connecté.</param>
        /// <param name="changePasswordDto">Les informations de changement de mot de passe.</param>
        /// <returns>Un message indiquant si le changement de mot de passe a réussi.</returns>
        public async Task<string> ChangeUserPasswordAsync(string userId, ChangeUserPasswordDto changePasswordDto)
        {
            // Vérifier si les nouveaux mots de passe correspondent
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return "Les mots de passe ne correspondent pas.";
            }

            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Vérifier si l'ancien mot de passe est correct
            var isOldPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.OldPassword);
            if (!isOldPasswordValid)
            {
                return "L'ancien mot de passe est incorrect.";
            }

            // Changer le mot de passe
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                return $"Erreur lors du changement de mot de passe : {errors}";
            }

            return "Votre mot de passe a été changé avec succès.";
        }

        /// <summary>
        /// Force un employé à changer son mot de passe (par exemple, si le mot de passe temporaire a expiré).
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'employé.</param>
        /// <returns>Un message indiquant si la réinitialisation forcée a réussi.</returns>
        public async Task<string> ForceEmployeePasswordChangeAsync(string AppUserId)
        {
            // Vérifier si l'utilisateur existe
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Générer un nouveau mot de passe temporaire
            var temporaryPassword = GenerateTemporaryPassword();

            // Réinitialiser le mot de passe de l'utilisateur avec le mot de passe temporaire
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, temporaryPassword);
            if (!resetResult.Succeeded)
            {
                return "Erreur lors de la réinitialisation du mot de passe ";
            }

            // Envoyer l'e-mail avec le nouveau mot de passe temporaire
            await SendEmployeePasswordResetEmailAsync(user, temporaryPassword);

            return "Un nouveau mot de passe temporaire a été envoyé à l'employé.";
        }


        /// <summary>
        /// Récupère la liste de tous les utilisateurs.
        /// </summary>
        /// <returns>Une liste d'utilisateurs ou un message d'erreur.</returns>
        public async Task<List<AppUserDto>> GetAllUsersAsync()
        {
            // Récupérer tous les utilisateurs
            var users = _userManager.Users.ToList();

            // Mapper les utilisateurs vers AppUserDto
            var userDtos = _mapper.Map<List<AppUserDto>>(users);

            return userDtos;
            }
    
            /// <summary>
            /// Récupère la liste des utilisateurs avec filtrage, pagination et tri.
            /// </summary>
            /// <param name="role">Filtre par rôle (optionnel).</param>
            /// <param name="page">Numéro de page (défaut 1).</param>
            /// <param name="pageSize">Taille de la page (défaut 10).</param>
            /// <param name="sortBy">Champ de tri (optionnel).</param>
            /// <param name="sortOrder">Ordre de tri ("asc" ou "desc", défaut "asc").</param>
            /// <returns>Tuple contenant la liste des utilisateurs et le nombre total.</returns>
            public async Task<(List<AppUserDto> Users, int TotalCount)> GetUsersFilteredAsync(string? role = null, int page = 1, int pageSize = 10, string? sortBy = null, string? sortOrder = "asc")
            {
                // Appeler le repository pour obtenir les utilisateurs filtrés et paginés
                var (users, totalCount) = await _unitOfWork.Users.GetUsersFilteredAsync(role, page, pageSize, sortBy, sortOrder);
    
                // Mapper les utilisateurs vers AppUserDto
                var userDtos = _mapper.Map<List<AppUserDto>>(users);
    
                return (userDtos, totalCount);
            }
    
    
            /// <summary>
            /// Récupère les détails d'un utilisateur spécifique par son identifiant.
            /// </summary>
            /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
            /// <returns>Les détails de l'utilisateur ou un message d'erreur.</returns>
            public async Task<AppUserDto> GetUserByIdAsync(string AppUserId)
        {
            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper l'utilisateur vers AppUserDto
            var userDto = _mapper.Map<AppUserDto>(user);

            return userDto;
        }


        /// <summary>
        /// Met à jour le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="updateAppUserDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateUserProfileAsync(string AppUserId, UpdateAppUserDto updateAppUserDto)
        {
            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Mettre à jour les informations de l'utilisateur
            user.FirstName = updateAppUserDto.FirstName;
            user.LastName = updateAppUserDto.LastName;
            user.Email = updateAppUserDto.Email;
            user.UserName = updateAppUserDto.UserName;
            user.PhoneNumber = updateAppUserDto.PhoneNumber;
            user.ProfilePictureUrl = updateAppUserDto.ProfilePictureUrl;

            // Mettre à jour l'utilisateur dans la base de données
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour du profil : " + string.Join(", ", updateResult.Errors.Select(e => e.Description));
            }

            return "Profil utilisateur mis à jour avec succès.";
        }

        /// <summary>
        /// Récupère le profil d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Le profil de l'utilisateur ou un message d'erreur.</returns>
        public async Task<UserProfileDto> GetUserProfileAsync(string AppUserId)
        {
            // Récupérer le profil utilisateur via le repository
            var user = await _unitOfWork.Users.GetUserProfileAsync(AppUserId);
            if (user == null)
            {
                return null;
            }

            // Mapper l'utilisateur vers UserProfileDto
            var userDto = _mapper.Map<UserProfileDto>(user);

            return userDto;
        }


        /// <summary>
        /// Récupère les réservations d'un utilisateur spécifique.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <returns>Les réservations de l'utilisateur ou un message d'erreur.</returns>
        public async Task<List<ReservationDto>> GetUserOrdersAsync(string AppUserId)
        {
            // Récupérer les réservations de l'utilisateur via le repository
            var reservations = await _unitOfWork.Users.GetUserOrdersAsync(AppUserId);
            if (reservations == null || !reservations.Any())
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper les réservations vers ReservationDto
            var reservationDtos = _mapper.Map<List<ReservationDto>>(reservations);

            return reservationDtos;
        }


        /// <summary>
        /// Récupère le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <returns>Le profil de l'employé ou un message d'erreur.</returns>
        public async Task<EmployeeProfileDto> GetEmployeeProfileAsync(string appUserId)
        {
            // Récupérer le profil de l'employé via le repository
            var employee = await _unitOfWork.Users.GetEmployeeProfileAsync(appUserId);
            if (employee == null)
            {
                return null; // Ou retourner un message d'erreur si nécessaire
            }

            // Mapper l'employé vers EmployeeProfileDto
            var employeeProfileDto = _mapper.Map<EmployeeProfileDto>(employee);

            return employeeProfileDto;
        }


        /// <summary>
        /// Met à jour le profil d'un employé spécifique.
        /// </summary>
        /// <param name="appUserId">L'identifiant de l'employé.</param>
        /// <param name="updateEmployeeDto">Les nouvelles données du profil.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> UpdateEmployeeProfileAsync(string appUserId, UpdateEmployeeDto updateEmployeeDto)
        {
            // Récupérer l'employé par son ID
            var employee = await _userManager.FindByIdAsync(appUserId);
            if (employee == null)
            {
                return "Employé non trouvé.";
            }

            // Mettre à jour les informations de l'employé
            employee.FirstName = updateEmployeeDto.FirstName;
            employee.LastName = updateEmployeeDto.LastName;
            employee.Email = updateEmployeeDto.Email;
            employee.PhoneNumber = updateEmployeeDto.PhoneNumber;
            employee.Position = updateEmployeeDto.Position;
            employee.ProfilePictureUrl = updateEmployeeDto.ProfilePictureUrl;

            // Mettre à jour l'employé dans la base de données
            var updateResult = await _userManager.UpdateAsync(employee);
            if (!updateResult.Succeeded)
            {
                return "Erreur lors de la mise à jour du profil : " + string.Join(", ", updateResult.Errors.Select(e => e.Description));
            }

            return "Profil employé mis à jour avec succès.";
        }

        /// <summary>
        /// Confirme l'adresse email d'un utilisateur.
        /// </summary>
        /// <param name="AppUserId">L'identifiant de l'utilisateur.</param>
        /// <param name="token">Le jeton de confirmation.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> ConfirmEmailAsync(string AppUserId, string token)
        {
            if (string.IsNullOrEmpty(AppUserId))
            {
                return "L'ID de l'utilisateur ne peut pas être vide.";
            }

            if (string.IsNullOrEmpty(token))
            {
                return "Le token de confirmation ne peut pas être vide.";
            }

            // Récupérer l'utilisateur par son ID
            var user = await _userManager.FindByIdAsync(AppUserId);
            if (user == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Confirmer l'email avec le token
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return "Erreur lors de la confirmation de l'email : " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return "Email confirmé avec succès.";
        }

        private string GenerateJwtToken(AppUser user)
        {
            // Récupérer les rôles de l'utilisateur
            var roles = _userManager.GetRolesAsync(user).Result;

            // Créer les claims (informations sur l'utilisateur)
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
        };

            // Ajouter les rôles de l'utilisateur aux claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Récupérer la clé secrète depuis la configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            // Créer les informations d'identification pour le token
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Configurer le token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:LifeSpanInDays"])),
                signingCredentials: creds
            );

            // Générer le token sous forme de chaîne
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateTemporaryPassword()
        {
            const string uppercase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijkmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*?_-";

            // Combinaison de tous les caractères valides
            const string validChars = uppercase + lowercase + digits + symbols;

            // Génération cryptographiquement sécurisée
            var randomBytes = new byte[12]; // 12 caractères
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            var password = new StringBuilder();
            foreach (byte b in randomBytes)
            {
                password.Append(validChars[b % validChars.Length]);
            }

            // Garantir au moins un caractère de chaque catégorie
            password[0] = uppercase[new Random().Next(uppercase.Length)];
            password[1] = lowercase[new Random().Next(lowercase.Length)];
            password[2] = digits[new Random().Next(digits.Length)];
            password[3] = symbols[new Random().Next(symbols.Length)];

            return password.ToString();
        }

        private async Task SendConfirmationEmailAsync(AppUser user)
        {
            // Générer le token de confirmation d'email
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encoder le token pour qu'il soit sûr pour les URLs
            var encodedToken = WebUtility.UrlEncode(emailConfirmationToken);

            // Vérifier que l'URL de base est correctement configurée
            var appBaseUrl = _configuration["AppBaseUrl"];
            if (string.IsNullOrEmpty(appBaseUrl))
            {
                _logger.LogWarning("AppBaseUrl n'est pas configuré dans appsettings.json");
                appBaseUrl = "https://localhost:7000"; // Valeur par défaut pour le développement
            }

            // Créer le lien de confirmation avec le bon format
            var confirmationLink = $"{appBaseUrl}/api/auth/confirm-email?AppUserId={user.Id}&token={encodedToken}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "ConfirmationEmail.html");
            
            if (!File.Exists(emailTemplatePath))
            {
                _logger.LogError("Template d'email de confirmation non trouvé: {EmailTemplatePath}", emailTemplatePath);
                return;
            }

            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate.Replace("{{ConfirmationLink}}", confirmationLink);

            // Envoyer l'email de confirmation
            var emailSubject = "Confirmez votre adresse email";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody, true);
            
            _logger.LogInformation("Email de confirmation envoyé à {Email} avec le lien: {ConfirmationLink}", user.Email, confirmationLink);
        }

        private async Task SendResetPasswordEmailAsync(AppUser user, string resetToken)
        {
            // Créer le lien de réinitialisation
            var resetPasswordLink = $"{_configuration["AppBaseUrl"]}/reset-password?AppUserId={user.Id}&token={WebUtility.UrlEncode(resetToken)}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "ResetPasswordEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate.Replace("{{ResetPasswordLink}}", resetPasswordLink);

            // Envoyer l'email de réinitialisation
            var emailSubject = "Réinitialisation de votre mot de passe";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody, true);
        }

        private async Task SendEmployeePasswordResetEmailAsync(AppUser user, string temporaryPassword)
        {
            // Créer le lien de changement de mot de passe
            var changePasswordLink = $"{_configuration["AppBaseUrl"]}/change-password?AppUserId={user.Id}";

            // Charger le template HTML
            var emailTemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "Configurations/Templates", "EmployeePasswordResetEmail.html");
            var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);

            // Remplacer les placeholders dans le template
            var emailBody = emailTemplate
                .Replace("{{ChangePasswordLink}}", changePasswordLink)
                .Replace("{{TemporaryPassword}}", temporaryPassword);

            // Envoyer l'email de changement de mot de passe
            var emailSubject = "Changement de mot de passe requis";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody, true);
        }

        private void ValidateUserRegistration(dynamic registerDto)
        {
            // Vérifie si les mots de passe correspondent
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new BadRequestException("Les mots de passe sont différents.");
            }

            // Vérifie la validité du rôle
            if (!Enum.IsDefined(typeof(UserRole), registerDto.Role))
            {
                throw new BadRequestException($"Le rôle spécifié '{registerDto.Role}' n'est pas valide.");
            }

            // Si aucun rôle n'est spécifié, définir le rôle par défaut (User)
            if (registerDto.Role == null)
            {
                registerDto.Role = UserRole.User;
            }
        }


        /// <summary>
        /// Renvoie l'email de confirmation à un utilisateur.
        /// </summary>
        /// <param name="email">L'adresse email de l'utilisateur.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return "Aucun utilisateur trouvé avec cette adresse email.";
            }

            if (user.EmailConfirmed)
            {
                return "Votre adresse email est déjà confirmée.";
            }

            try
            {
                await SendConfirmationEmailAsync(user);
                return "Un nouvel email de confirmation a été envoyé.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du renvoi de l'email de confirmation à {Email}", email);
                return "Une erreur s'est produite lors de l'envoi de l'email de confirmation.";
            }
        }

        public async Task SendContactByEmail(string username, string title, string description, string fromEmail)
        {
            // Adresse email générique de Cinéphoria
            string cinephoriaEmail = "contact@cinephoria.com";

            // Construire le corps de l'email
            string emailBody = $@"
                <h2>Nouveau message de contact</h2>
                <p><strong>Nom d'utilisateur:</strong> {(string.IsNullOrEmpty(username) ? "Non spécifié" : username)}</p>
                <p><strong>Email:</strong> {fromEmail}</p>
                <p><strong>Titre de la demande:</strong> {title}</p>
                <p><strong>Description:</strong></p>
                <p>{description}</p>
            ";

            // Envoyer l'email via EmailService
            await _emailService.SendEmailAsync(cinephoriaEmail, title, emailBody, true);
        }

        /// <summary>
        /// Supprime un utilisateur en fonction de son rôle et des autorisations.
        /// </summary>
        /// <param name="currentUserId">L'ID de l'utilisateur actuellement connecté.</param>
        /// <param name="currentUserRole">Le rôle de l'utilisateur actuellement connecté.</param>
        /// <param name="targetUserId">L'ID de l'utilisateur à supprimer.</param>
        /// <returns>Un message indiquant si l'opération a réussi.</returns>
        public async Task<string> DeleteUserAsync(string currentUserId, string currentUserRole, string targetUserId)
        {
            // Vérifier si l'utilisateur cible existe
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            if (targetUser == null)
            {
                return "Utilisateur non trouvé.";
            }

            // Récupérer le rôle de l'utilisateur cible
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);
            var targetUserRole = targetUserRoles.FirstOrDefault();

            // Vérifier les autorisations en fonction des rôles
        if (currentUserRole == UserRole.Admin.ToString())
        {
            // Un administrateur peut supprimer n'importe quel utilisateur sauf lui-même
            if (currentUserId == targetUserId)
            {
                return "Un administrateur ne peut pas supprimer son propre compte.";
            }

            // Un administrateur peut supprimer des utilisateurs, employés ou d'autres administrateurs
            if (targetUserRole == UserRole.User.ToString() || targetUserRole == UserRole.Employee.ToString() || targetUserRole == UserRole.Admin.ToString())
            {
                var result = await _userManager.DeleteAsync(targetUser);
                if (!result.Succeeded)
                {
                    return $"Erreur lors de la suppression de l'utilisateur: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }
                return "Utilisateur supprimé avec succès.";
            }
            else
            {
                return "Rôle d'utilisateur non reconnu.";
            }
        }
            else if (currentUserRole == UserRole.User.ToString())
            {
                // Un utilisateur ne peut supprimer que son propre compte
                if (currentUserId != targetUserId)
                {
                    return "Vous n'êtes pas autorisé à supprimer ce compte.";
                }

                var result = await _userManager.DeleteAsync(targetUser);
                if (!result.Succeeded)
                {
                    return $"Erreur lors de la suppression de votre compte: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                }
                return "Votre compte a été supprimé avec succès.";
            }
            else
            {
                return "Vous n'avez pas les autorisations nécessaires pour effectuer cette action.";
            }
        }
    }
}
