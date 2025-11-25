namespace CinephoriaServer.API.Models.PostgresqlDb
{
    public class RemoveHandicapSeatDto
    {
        /// <summary>
        /// Identifiant de la salle de cinéma.
        /// </summary>
        public int TheaterId { get; set; }

        /// <summary>
        /// Numéro du siège à supprimer.
        /// </summary>
        public string SeatNumber { get; set; }
    }
}
