namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class AddHandicapSeatDto
    {
        /// <summary>
        /// Identifiant de la salle de cinéma.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Numéro du siège à ajouter.
        /// </summary>
        public string SeatNumber { get; set; }
    }
}
