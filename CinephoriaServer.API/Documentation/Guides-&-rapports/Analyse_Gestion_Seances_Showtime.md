
# Analyse de la Gestion des Séances (Showtime) - Cinephoria

## Table des Matières
1. [État Actuel de la Gestion des Séances](#état-actuel-de-la-gestion-des-séances)
2. [Fonctionnalités Existantes](#fonctionnalités-existantes)
3. [Lacunes Identifiées](#lacunes-identifiées)
4. [Propositions d'Amélioration](#propositions-damélioration)
5. [Plan d'Implémentation Détaillé](#plan-dimplémentation-détaillé)
6. [Recommandations Finales](#recommandations-finales)

## État Actuel de la Gestion des Séances

### Structure Actuelle

#### Entité Showtime
- **Gestion complète** des séances avec film, salle, cinéma
- **Horaires** : Début et fin de séance
- **Qualité de projection** : 4DX, 3D, IMAX, 4K, Standard2D, DolbyCinema
- **Système de prix** : Calcul automatique basé sur la qualité + ajustements + promotions

#### Système de Réservation
- **Intégration complète** avec gestion des sièges
- **QR codes** pour validation
- **Gestion des annulations** avec libération des sièges
- **Notifications** pour nouvelles réservations et annulations

#### Gestion des Films
- **Films avec séances** programmées
- **Système de notation** et avis utilisateurs
- **Historique de consultation** par utilisateur
- **Import depuis TMDb** (The Movie Database)

#### Architecture API
- **Contrôleurs dédiés** : [`ShowtimeController`](CinephoriaServer.API/Controllers/ShowtimeController.cs), [`ReservationController`], [`MovieController`]
- **Services spécialisés** : [`ShowtimeService`](CinephoriaServer.API/Services/Showtime/ShowtimeService.cs), [`ReservationService`](CinephoriaServer.API/Services/Reservation/ReservationService.cs), [`MovieService`](CinephoriaServer.API/Services/Movie/MovieService.cs)
- **Repository pattern** avec Unit of Work

## Fonctionnalités Existantes

### Création et Gestion des Séances

#### Création de Séances
```csharp
// Dans ShowtimeService.cs
public async Task<string> CreateShowtimeAsync(CreateShowtimeDto createShowtimeDto)
{
    // Calcul automatique du prix basé sur la qualité
    var basePrice = CalculateBasePrice(createShowtimeDto.Quality);
    var finalPrice = basePrice + createShowtimeDto.PriceAdjustment;
    
    if (createShowtimeDto.IsPromotion)
    {
        finalPrice *= 0.9m; // 10% de réduction
    }
}
```

**Fonctionnalités :**
- Calcul automatique des prix selon la qualité
- Ajustements de prix personnalisés
- Système de promotions (10% de réduction)
- Validation des données d'entrée

#### Mise à Jour des Séances
- **Recalcul automatique** des prix si qualité ou horaire modifié
- **Gestion des dépendances** avec les réservations existantes
- **Journalisation** des modifications

#### Suppression de Séances
- **Cascade automatique** des réservations associées
- **Libération des sièges** réservés
- **Notifications** aux utilisateurs concernés

### Films Récents et Séances

#### Films du Dernier Mercredi
```csharp
// Dans MovieRepository.cs - Ligne 119
public async Task<List<Movie>> GetRecentMoviesAsync()
{
    DateTime lastWednesday = GetLastWednesday();
    
    return await _context.Set<Movie>()
        .Include(m => m.Showtimes)
        .Where(m => m.CreatedAt >= lastWednesday && m.Showtimes.Any())
        .OrderByDescending(m => m.CreatedAt)
        .Take(20)
        .ToListAsync();
}
```

**Caractéristiques :**
- **Période** : Depuis le dernier mercredi
- **Filtrage** : Uniquement les films avec au moins une séance
- **Limitation** : 20 films maximum
- **Tri** : Par date de création décroissante

#### Films avec Séances Programmées
```csharp
// Dans MovieRepository.cs - Ligne 337
public async Task<List<Movie>> GetAllMoviesWithShowtimeAsync()
{
    return await _context.Set<Movie>()
        .Include(m => m.Showtimes)
        .Where(m => m.Showtimes.Any(s => s.StartTime >= DateTime.UtcNow))
        .OrderBy(m => m.Title)
        .ToListAsync();
}
```

**Fonctionnalités :**
- **Filtrage** : Séances futures uniquement
- **Tri** : Par titre alphabétique
- **Inclusion** : Données des séances associées

### Système de Réservation

#### Processus de Réservation
1. **Vérification disponibilité** des sièges
2. **Blocage temporaire** des sièges sélectionnés
3. **Calcul du prix total** basé sur la séance et le nombre de sièges
4. **Création de la réservation** avec statut "Confirmé"
5. **Génération du QR code**
6. **Notification** à l'utilisateur

#### Validation des Séances
```csharp
// Dans ReservationService.cs - Ligne 245
public async Task<bool> ValidatedSession(string qrCodeData)
{
    // Décodage du QR code
    // Vérification de la validité de la séance
    // Marquage de la réservation comme validée
}
```

**Sécurité :**
- **Format QR code** : ReservationId:123;ShowtimeId:456;AppUserId:789
- **Vérification** : Séance non commencée
- **Validation unique** : Empêche la réutilisation

## Lacunes Identifiées

### 1. Gestion des Dates des Séances

#### Problèmes Actuels
- **Aucune distinction** entre séances passées, en cours et futures
- **Pas d'archivage** des séances terminées
- **Statistiques manquantes** sur les performances passées
- **Nettoyage manuel** nécessaire pour les anciennes séances

#### Impact
- **Expérience utilisateur** dégradée avec séances passées affichées
- **Performance** dégradée avec accumulation de données
- **Analyse impossible** des tendances et performances

### 2. Films du Dernier Mercredi

#### Limitations
- **Rigidité** : Période fixe (dernier mercredi seulement)
- **Limite arbitraire** : 20 films maximum
- **Pas de personnalisation** : Pas de filtrage par cinéma ou genre
- **Pas de pagination** : Impossible de naviguer dans les résultats

#### Impact sur l'Expérience Utilisateur
- **Manque de flexibilité** pour les utilisateurs
- **Expérience limitée** sur les grands catalogues
- **Personnalisation impossible** selon les préférences

### 3. Système de Réservation

#### Déficiences
- **Pas d'expiration** : Réservations en attente indéfiniment
- **Pas de confirmation** : Aucun email de confirmation
- **Pas de rappels** : Aucun rappel avant la séance
- **Statuts limités** : Seulement Pending, Confirmed, Cancelled

#### Impact Opérationnel
- **Occupation inefficace** des salles
- **Taux d'annulation** élevé
- **Expérience utilisateur** incomplète
- **Perte de revenus** potentielle

## Propositions d'Amélioration

### 1. Amélioration de la Gestion des Dates

#### Nouvelle Structure de Statuts
```csharp
// À ajouter dans EnumConfig.cs
public enum ShowtimeStatus
{
    Upcoming,      // Séances à venir (StartTime > DateTime.Now)
    Ongoing,       // Séances en cours (StartTime <= DateTime.Now && EndTime > DateTime.Now)
    Completed,     // Séances terminées (EndTime <= DateTime.Now)
    Cancelled      // Séances annulées manuellement
}
```

#### Modifications de l'Entité Showtime
```csharp
// À ajouter dans Showtime.cs
public ShowtimeStatus Status { get; set; } = ShowtimeStatus.Upcoming;
public DateTime? ActualStartTime { get; set; } // Heure réelle de début
public DateTime? ActualEndTime { get; set; }   // Heure réelle de fin
public int OccupancyRate { get; set; }         // Taux d'occupation (%)
```

#### Nouveaux Services
- **Service de mise à jour automatique** des statuts
- **Service d'archivage** des séances anciennes
- **Service de statistiques** sur les performances

### 2. Amélioration des Films Récents

#### Extension de GetRecentMoviesAsync()
```csharp
public async Task<List<Movie>> GetRecentMoviesAdvancedAsync(
    DateTime? sinceDate = null,
    int? cinemaId = null,
    MovieGenre? genre = null,
    int limit = 20,
    int page = 1)
{
    // Implémentation avec filtrage avancé et pagination
}
```

#### Nouveau DTO pour le Filtrage
```csharp
public class RecentMoviesFilterDto
{
    public DateTime? SinceDate { get; set; }
    public int? CinemaId { get; set; }
    public MovieGenre? Genre { get; set; }
    public int Limit { get; set; } = 20;
    public int Page { get; set; } = 1;
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
```

#### Fonctionnalités Avancées
- **Période personnalisable** : Au-delà du dernier mercredi
- **Filtrage multi-critères** : Cinéma, genre, date
- **Pagination complète** : Navigation dans les résultats
- **Tri personnalisable** : Date, titre, popularité

### 3. Amélioration du Système de Réservation

#### Nouveaux Statuts de Réservation
```csharp
// À étendre dans EnumConfig.cs
public enum ReservationStatus
{
    Pending,       // En attente de confirmation (15 minutes max)
    Confirmed,     // Confirmée et payée
    Expired,       // Expirée (non confirmée dans le délai)
    Cancelled,     // Annulée par l'utilisateur
    Used,          // Utilisée (QR code scanné)
    NoShow         // Non présenté à la séance
}
```

#### Système d'Expiration Automatique
```csharp
public class ReservationExpirationService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Rechercher les réservations en attente depuis plus de 15 minutes
            // Les marquer comme expirées
            // Libérer les sièges associés
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

#### Système de Rappels
```csharp
public class ReservationReminderService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Rechercher les réservations confirmées pour demain
            // Envoyer des rappels par email
            // Rechercher les réservations confirmées pour dans 1 heure
            // Envoyer des rappels par notification push
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### 4. Gestion Avancée des Séances

#### Nouveaux Endpoints API
```csharp
// Séances par période
[HttpGet("upcoming")]
Task<List<ShowtimeDto>> GetUpcomingShowtimesAsync();

[HttpGet("today")]
Task<List<ShowtimeDto>> GetTodayShowtimesAsync();

[HttpGet("week")]
Task<List<ShowtimeDto>> GetThisWeekShowtimesAsync();

// Séances populaires
[HttpGet("popular")]
Task<List<ShowtimeDto>> GetPopularShowtimesAsync(int limit = 10);

// Statistiques
[HttpGet("stats/{showtimeId}")]
Task<ShowtimeStatsDto> GetShowtimeStatsAsync(int showtimeId);
```

#### Optimisations de Performance

##### Cache des Séances Fréquentes
```csharp
public class CachedShowtimeService
{
    private readonly IMemoryCache _cache;
    
    public async Task<List<ShowtimeDto>> GetCachedUpcomingShowtimesAsync()
    {
        return await _cache.GetOrCreateAsync("upcoming_showtimes", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _showtimeService.GetUpcomingShowtimesAsync();
        });
    }
}
```

##### Indexation Base de Données
```sql
-- Index pour les requêtes fréquentes
CREATE INDEX IX_Showtime_StartTime_Status ON Showtimes(StartTime, Status);
CREATE INDEX IX_Showtime_MovieId_StartTime ON Showtimes(MovieId, StartTime);
CREATE INDEX IX_Showtime_CinemaId_StartTime ON Showtimes(CinemaId, StartTime);
```

## Plan d'Implémentation Détaillé

### Phase 1 : Gestion des Dates (2 semaines)

#### Semaine 1 : Structure de Base
1. **Ajouter ShowtimeStatus** dans [`EnumConfig.cs`](CinephoriaServer.API/Configurations/EnumConfig.cs)
2. **Mettre à jour l'entité Showtime** avec nouveaux champs
3. **Créer la migration** de base de données
4. **Mettre à jour ShowtimeProfile** pour le mapping

#### Semaine 2 : Services et API
1. **Implémenter ShowtimeStatusService** pour mise à jour automatique
2. **Créer les nouveaux endpoints** API
3. **Mettre à jour les repositories** existants
4. **Tests unitaires** et d'intégration

### Phase 2 : Amélioration des Films Récents (1 semaine)

#### Tâches
1. **Étendre IMovieRepository** avec nouvelle méthode
2. **Implémenter GetRecentMoviesAdvancedAsync**
3. **Créer RecentMoviesFilterDto**
4. **Mettre à jour MovieController**
5. **Tests** avec différents scénarios de filtrage

### Phase 3 : Système de Réservation Avancé (3 semaines)

#### Semaine 1 : Statuts et Expiration
1. **Étendre ReservationStatus** dans EnumConfig
2. **Mettre à jour l'entité Reservation**
3. **Implémenter ReservationExpirationService**
4. **Tests** d'expiration automatique

#### Semaine 2 : Rappels et Notifications
1. **Implémenter ReservationReminderService**
2. **Créer les templates d'email**
3. **Intégrer le système de notifications**
4. **Tests** d'envoi de rappels

#### Semaine 3 : Optimisations et Polissage
1. **Mettre à jour l'UI** pour les nouveaux statuts
2. **Implémenter les rapports** de performance
3. **Tests de charge** et performance
4. **Documentation** API mise à jour

### Phase 4 : Optimisations (1 semaine)

#### Tâches
1. **Implémenter le cache** avec IMemoryCache
2. **Optimiser les requêtes** avec projection
3. **Ajouter les index** en base de données
4. **Tests de performance** complets
5. **Surveillance** et métriques

## Recommandations Finales

### Priorité Haute (Impact Immédiat)

#### 1. Gestion des Statuts de Séances
**Impact** : Améliore l'expérience utilisateur et les performances
**Effort** : 2 semaines
**ROI** : Élevé - Réduction de la confusion utilisateur

#### 2. Expiration des Réservations
**Impact** : Optimise l'occupation des salles et réduit les pertes
**Effort** : 1 semaine
**ROI** : Très élevé - Augmentation directe des revenus

#### 3. Filtrage Avancé des Films Récents
**Impact** : Personnalisation de l'expérience utilisateur
**Effort** : 1 semaine
**ROI** : Élevé - Augmentation de l'engagement

### Priorité Moyenne (Impact à Moyen Terme)

#### 1. Système de Rappels
**Impact** : Réduction des annulations et des "no-shows"
**Effort** : 2 semaines
**ROI** : Moyen - Amélioration de la satisfaction

#### 2. Statistiques des Séances
**Impact** : Aide à la décision pour la programmation
**Effort** : 1 semaine
**ROI** : Moyen - Optimisation à long terme

#### 3. Cache des Séances
**Impact** : Amélioration des performances
**Effort** : 1 semaine
**ROI** : Moyen - Meilleure expérience utilisateur

### Priorité Basse (Maintenance et Optimisation)

#### 1. Archivage Automatique
**Impact** : Maintenance et performance
**Effort** : 1 semaine
**ROI** : Faible - Impact indirect

#### 2. Nettoyage des Anciennes Séances
**Impact** : Performance base de données
**Effort** : 1 semaine
**ROI** : Faible - Maintenance préventive

### Métriques de Suivi

#### Avant/Après Implémentation
- **Taux d'occupation des salles** : Cible +30%
- **Taux