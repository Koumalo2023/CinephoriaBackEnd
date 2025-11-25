namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class LoginResponseDto
    {
        /// <summary>
        /// Jeton JWT pour l'authentification.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Profil de l'utilisateur (UserProfileDto ou EmployeeProfileDto).
        /// </summary>
        public object Profile { get; set; }
    }
}
