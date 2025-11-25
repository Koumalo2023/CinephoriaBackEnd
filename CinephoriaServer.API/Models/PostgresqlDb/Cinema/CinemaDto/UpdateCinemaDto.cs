namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class UpdateCinemaDto
    {
        /// <summary>
        /// Identifiant unique du cinéma à mettre à jour.
        /// </summary>
        public int CinemaId { get; set; }

        /// <summary>
        /// Nouveau nom du cinéma.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nouvelle adresse du cinéma.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Nouveau numéro de téléphone du cinéma.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Nouvelle ville où se trouve le cinéma.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Nouveau pays où se trouve le cinéma.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Nouveaux horaires d'ouverture du cinéma.
        /// </summary>
        public string OpeningHours { get; set; }
    }
}
