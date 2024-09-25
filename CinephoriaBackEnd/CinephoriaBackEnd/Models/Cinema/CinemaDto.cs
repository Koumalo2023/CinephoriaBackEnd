namespace CinephoriaBackEnd.Models
{
    public class CinemaDto
    {
        /// <summary>
        /// Identifiant unique du cinéma.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom du cinéma.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adresse du cinéma.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Numéro de téléphone du cinéma.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ville où se trouve le cinéma.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Pays où se trouve le cinéma.
        /// </summary>
        public string Country { get; set; }
    }

}
