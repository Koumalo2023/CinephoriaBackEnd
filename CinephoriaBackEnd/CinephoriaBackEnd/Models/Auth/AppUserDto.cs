namespace CinephoriaBackEnd.Models
{
    public class AppUserDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Rôle de l'utilisateur dans l'application.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Date de création du compte utilisateur.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date de la dernière mise à jour des informations utilisateur.
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

}
