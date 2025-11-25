namespace CinephoriaServer.API.Configurations
{
    public static class EnumConfig
    {

        public enum UserRole
        {
            User,
            Employee,
            Admin
        }

        public enum ReservationStatus
        {
            Pending,        // Réservation en attente de paiement
            Confirmed,      // Réservation confirmée et payée
            Expired,        // Réservation expirée (non payée dans le délai)
            Cancelled,      // Réservation annulée par l'utilisateur
            NoShow,         // Réservation non honorée (utilisateur absent)
            Completed       // Réservation honorée (utilisateur présent)
        }

        public enum ProjectionQuality
        {
            FourDX,
            ThreeD,
            IMAX,
            FourK,
            Standard2D,
            DolbyCinema
        }


        public enum IncidentStatus
        {
            Pending,
            InProgress,
            Resolved
        }


        public enum MovieGenre
        {

            Action,
            Aventure,
            Comedie,
            Animation,
            Crime,
            Documentaire,
            Fantastique,
            Guerre,
            Horreur,
            Western,
            Romance,
            Familiale,
            Thriller,
            Mystere,

        }
        

        
    public enum NotificationType
    {
        ReservationCreated,
        ReservationCanceled,
        UserRegistered,
        SystemAlert,
        SecurityIncident,
        PaymentProcessed,
        MovieAdded,
        ShowtimeUpdated
    }

    public enum NotificationSeverity
    {
        Info,
        Warning,
        Error,
        Success,
        Critical
    }

    public enum MinimumAge
    {
        All = -1,        // pour "Tous âges"
        Public = 0,      // "Tous publics" Champs par défaut
        Ten = 10,        // "+10 ans"
        Twelve = 12,     // "+12 ans"
        Sixteen = 16,    // "+16 ans"
        Eighteen = 18    // "+18 ans"
    }

    public enum ShowtimeStatus
    {
        Upcoming,      // Séances à venir (StartTime > DateTime.Now)
        Ongoing,       // Séances en cours (StartTime <= DateTime.Now && EndTime > DateTime.Now)
        Completed,     // Séances terminées (EndTime <= DateTime.Now)
        Cancelled      // Séances annulées manuellement
    }
    }
}
