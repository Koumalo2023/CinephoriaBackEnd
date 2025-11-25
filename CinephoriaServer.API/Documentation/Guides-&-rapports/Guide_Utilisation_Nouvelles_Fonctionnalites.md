# Guide d'Utilisation des Nouvelles Fonctionnalités

## 📋 Table des Matières
- [Workflows Administrateur](#workflows-administrateur)
- [Workflows Utilisateur](#workflows-utilisateur)
- [Endpoints et DTOs](#endpoints-et-dtos)

---

## 🛠️ Workflows Administrateur

### Workflow 1: Gestion des Statuts des Séances

**Objectif** : Surveiller et gérer les statuts automatiques des séances

#### Endpoints Utilisés :
- `GET /api/showtime/status` - Récupérer toutes les séances avec statuts
- `GET /api/showtime/stats` - Obtenir les statistiques des séances
- `POST /api/showtime/update-status` - Forcer la mise à jour des statuts
- `PUT /api/showtime/{id}/status` - Mettre à jour manuellement un statut

#### DTOs Utilisés :
```csharp
// ShowtimeStatusDto
public class ShowtimeStatusDto
{
    public int ShowtimeId { get; set; }
    public string MovieTitle { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TheaterName { get; set; }
    public EnumConfig.ShowtimeStatus Status { get; set; }
    public string StatusDescription { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
}

// ShowtimeStatusStatsDto
public class ShowtimeStatusStatsDto
{
    public int TotalShowtimes { get; set; }
    public int UpcomingCount { get; set; }
    public int OngoingCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public Dictionary<string, int> StatusDistribution { get; set; }
}

// ShowtimeStatusUpdateDto
public class ShowtimeStatusUpdateDto
{
    public EnumConfig.ShowtimeStatus Status { get; set; }
}
```

#### Workflow :
1. **Surveillance quotidienne** : Appeler `/api/showtime/stats` pour voir la répartition
2. **Vérification des séances** : Utiliser `/api/showtime/status` pour lister toutes les séances
3. **Mise à jour automatique** : Exécuter `/api/showtime/update-status` pour synchroniser les statuts
4. **Correction manuelle** : Si nécessaire, utiliser `/api/showtime/{id}/status` pour forcer un statut

---

### Workflow 2: Gestion des Rappels de Réservation

**Objectif** : Superviser et déclencher les rappels automatiques

#### Endpoints Utilisés :
- `POST /api/reservation/send-payment-reminders` - Envoyer les rappels de paiement
- `POST /api/reservation/send-showtime-reminders` - Envoyer les rappels de séance
- `POST /api/reservation/send-expiration-warnings` - Envoyer les avertissements d'expiration
- `GET /api/reservation/reminder-stats` - Obtenir les statistiques des rappels
- `GET /api/reservation/pending-payment-reminders` - Voir les rappels en attente
- `GET /api/reservation/pending-showtime-reminders` - Voir les rappels de séance en attente

#### DTOs Utilisés :
```csharp
// ReminderResultDto
public class ReminderResultDto
{
    public string Message { get; set; }
    public int Count { get; set; }
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; }
}

// ReminderStatsDto
public class ReminderStatsDto
{
    public int TotalReservations { get; set; }
    public int PendingPaymentReminders { get; set; }
    public int PendingShowtimeReminders { get; set; }
    public int ExpiringReservations { get; set; }
    public int PaymentRemindersSentToday { get; set; }
    public int ShowtimeRemindersSentToday { get; set; }
}

// ReservationReminderDto
public class ReservationReminderDto
{
    public int ReservationId { get; set; }
    public string UserEmail { get; set; }
    public string MovieTitle { get; set; }
    public DateTime ShowtimeDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastReminderSent { get; set; }
    public string Status { get; set; }
}
```

#### Workflow :
1. **Surveillance matinale** : Vérifier `/api/reservation/reminder-stats`
2. **Rappels automatiques** : Les services de fond s'exécutent automatiquement
3. **Intervention manuelle** : Si nécessaire, déclencher manuellement les rappels
4. **Suivi des envois** : Consulter les listes de rappels en attente

---

### Workflow 3: Analyse des Films avec Séances

**Objectif** : Analyser la programmation des films

#### Endpoints Utilisés :
- `GET /api/showtime/movies-with-showtimes` - Films ayant au moins une séance
- `GET /api/showtime/recent-movies` - Films ajoutés le dernier mercredi

#### DTOs Utilisés :
```csharp
// Retourne une liste d'objets anonymes avec :
public class MovieWithShowtimesDto
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public int ShowtimeCount { get; set; }
    public DateTime? NextShowtime { get; set; }
    public DateTime? LastShowtime { get; set; }
    public int TotalSeatsAvailable { get; set; }
    public int TotalSeatsBooked { get; set; }
}
```

#### Workflow :
1. **Analyse hebdomadaire** : Vérifier les films avec séances
2. **Nouveautés** : Consulter les films récemment ajoutés
3. **Optimisation** : Ajuster la programmation basée sur les données

---

### Workflow 4: Configuration et Test du Système Email

**Objectif** : Vérifier et tester la configuration email

#### Endpoints Utilisés :
- `GET /api/reservation/email-stats` - Statistiques d'email
- `POST /api/reservation/test-email-config` - Test de configuration

#### DTOs Utilisés :
```csharp
// EmailStatsDto
public class EmailStatsDto
{
    public int TotalEmailsSent { get; set; }
    public int SuccessfulEmails { get; set; }
    public int FailedEmails { get; set; }
    public DateTime LastEmailSent { get; set; }
    public Dictionary<string, int> EmailTypes { get; set; }
}

// TestEmailResultDto
public class TestEmailResultDto
{
    public bool IsConfigured { get; set; }
    public bool TestEmailSent { get; set; }
    public string Message { get; set; }
    public string? Error { get; set; }
    public DateTime Timestamp { get; set; }
}
```

#### Workflow :
1. **Vérification quotidienne** : Consulter `/api/reservation/email-stats`
2. **Test de configuration** : Exécuter `/api/reservation/test-email-config`
3. **Dépannage** : En cas d'erreur, vérifier la configuration SMTP

---

## 👤 Workflows Utilisateur

### Workflow 1: Réservation Complète

**Objectif** : Réserver des places pour une séance

#### Endpoints Utilisés :
- `GET /api/reservation/movie/{movieId}/sessions` - Voir les séances d'un film
- `GET /api/reservation/showtime/{showtimeId}/seats` - Voir les sièges disponibles
- `POST /api/reservation/create` - Créer la réservation

#### DTOs Utilisés :
```csharp
// CreateReservationDto
public class CreateReservationDto
{
    public int ShowtimeId { get; set; }
    public string AppUserId { get; set; }
    public List<int> SeatIds { get; set; }
    public decimal TotalPrice { get; set; }
}
```

#### Workflow :
1. **Recherche** : Consulter les séances disponibles pour un film
2. **Sélection** : Choisir une séance et voir les sièges disponibles
3. **Réservation** : Sélectionner les sièges et confirmer
4. **Confirmation** : Recevoir un email de confirmation avec QR Code

---

### Workflow 2: Gestion des Réservations Personnelles

**Objectif** : Gérer ses propres réservations

#### Endpoints Utilisés :
- `GET /api/reservation/user/{AppUserId}` - Mes réservations
- `DELETE /api/reservation/cancel/{reservationId}` - Annuler une réservation

#### DTOs Utilisés :
```csharp
// UserReservationDto
public class UserReservationDto
{
    public int ReservationId { get; set; }
    public string MovieTitle { get; set; }
    public DateTime ShowtimeDate { get; set; }
    public string TheaterName { get; set; }
    public List<string> SeatNumbers { get; set; }
    public decimal TotalPrice { get; set; }
    public EnumConfig.ReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaymentDueDate { get; set; }
    public string QRCodeData { get; set; }
}
```

#### Workflow :
1. **Consultation** : Voir toutes mes réservations
2. **Annulation** : Annuler une réservation si nécessaire (avant expiration)
3. **Rappels** : Recevoir automatiquement des rappels de paiement et de séance

---

### Workflow 3: Consultation des Séances à Venir

**Objectif** : Voir les séances programmées

#### Endpoints Utilisés :
- `GET /api/showtime/upcoming` - Séances à venir (24h)
- `GET /api/showtime/ongoing` - Séances en cours

#### DTOs Utilisés :
```csharp
// ShowtimeStatusDto (même que pour l'admin)
public class ShowtimeStatusDto
{
    public int ShowtimeId { get; set; }
    public string MovieTitle { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TheaterName { get; set; }
    public EnumConfig.ShowtimeStatus Status { get; set; }
    public string StatusDescription { get; set; }
    public int AvailableSeats { get; set; }
    public int TotalSeats { get; set; }
}
```

#### Workflow :
1. **Planning** : Consulter les séances à venir pour planifier
2. **Disponibilité** : Voir les places disponibles en temps réel
3. **Décision** : Choisir la séance qui convient

---

### Workflow 4: Validation QR Code à l'Entrée

**Objectif** : Valider son billet à l'entrée de la salle

#### Endpoints Utilisés :
- `POST /api/reservation/validate` - Valider le QR Code

#### DTOs Utilisés :
```csharp
// Le endpoint attend simplement le string du QR Code
// Retourne un message de succès ou d'échec
```

#### Workflow :
1. **Présentation** : Montrer le QR Code sur son téléphone
2. **Scan** : Le personnel scanne le QR Code
3. **Validation** : Le système valide et marque la réservation comme utilisée
4. **Accès** : Accès autorisé à la salle

---

## 🔧 Endpoints et DTOs Complets

### ShowtimeController Endpoints

| Méthode | Endpoint | Rôle | Autorisation |
|---------|----------|------|-------------|
| GET | `/api/showtime/status` | Toutes les séances avec statuts | User |
| GET | `/api/showtime/status/{status}` | Séances par statut | User |
| GET | `/api/showtime/upcoming` | Séances à venir (24h) | User |
| GET | `/api/showtime/ongoing` | Séances en cours | User |
| GET | `/api/showtime/stats` | Statistiques des séances | Admin/Manager |
| PUT | `/api/showtime/{id}/status` | Mettre à jour statut | Admin/Manager |
| POST | `/api/showtime/update-status` | Forcer mise à jour statuts | Admin/Manager |
| GET | `/api/showtime/movies-with-showtimes` | Films avec séances | User |
| GET | `/api/showtime/recent-movies` | Films récents avec séances | User |

### ReservationController Endpoints

| Méthode | Endpoint | Rôle | Autorisation |
|---------|----------|------|-------------|
| POST | `/api/reservation/send-payment-reminders` | Rappels paiement | Admin/Manager |
| POST | `/api/reservation/send-showtime-reminders` | Rappels séance | Admin/Manager |
| POST | `/api/reservation/send-expiration-warnings` | Avertissements expiration | Admin/Manager |
| GET | `/api/reservation/reminder-stats` | Stats rappels | Admin/Manager |
| GET | `/api/reservation/email-stats` | Stats email | Admin/Manager |
| POST | `/api/reservation/test-email-config` | Test configuration email | Admin/Manager |
| GET | `/api/reservation/pending-payment-reminders` | Rappels paiement en attente | Admin/Manager |
| GET | `/api/reservation/pending-showtime-reminders` | Rappels séance en attente | Admin/Manager |

## ⚙️ Configuration Requise

### Services de Fond
Les services suivants doivent être configurés dans `Program.cs` :

```csharp
// Services de statuts et rappels
builder.Services.AddScoped<IShowtimeStatusService, ShowtimeStatusService>();
builder.Services.AddScoped<IReservationReminderService, ReservationReminderService>();
builder.Services.AddScoped<IReservationExpirationService, ReservationExpirationService>();

// Services de fond
builder.Services.AddHostedService<ShowtimeStatusBackgroundService>();
builder.Services.AddHostedService<ReservationExpirationBackgroundService>();
builder.Services.AddHostedService<ReservationReminderBackgroundService>();
```

### Configuration Email
Assurez-vous que la configuration SMTP est définie dans `appsettings.json` :

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "Port": 587,
    "Username": "noreply@cinephoria.com",
    "Password": "your-password",
    "FromEmail": "noreply@cinephoria.com",
    "FromName": "Cinephoria"
  }
}
```

---

## 📞 Support et Dépannage

### Problèmes Courants

1. **Statuts non mis à jour** : Vérifier que `ShowtimeStatusBackgroundService` est actif
2. **Rappels non envoyés** : Tester la configuration email avec `/api/reservation/test-email-config`
3. **Expirations non traitées** : Vérifier `ReservationExpirationBackgroundService`

### Logs
Consultez les logs dans `logs/CinephoriaLog{date}.txt` pour le débogage.

Ce guide couvre l'ensemble des nouvelles fonctionnalités et leur utilisation pratique au quotidien.