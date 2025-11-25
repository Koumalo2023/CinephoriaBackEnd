# Analyse de la Gestion des Séances - Problèmes et Solutions

## Problèmes Identifiés

### 1. **Séances Passées qui Continuent de S'Afficher**

**Problème :** Les séances dont les dates sont passées continuent d'être affichées dans les résultats des méthodes :
- [`GetMovieSessions(int movieId)`](CinephoriaServer.API/Controllers/ReservationController.cs:37) dans `ReservationController`
- [`GetMoviesByCinemaId(int cinemaId)`](CinephoriaServer.API/Controllers/MovieController.cs:420) dans `MovieController`

**Analyse :**
- Dans [`MovieRepository.GetMovieSessionsAsync(int movieId)`](CinephoriaServer.API/Repository/Modules/MovieRepository.cs:159) : **AUCUN FILTRE** sur la date des séances
- Dans [`ReservationRepository.GetMovieSessionsAsync(int movieId)`](CinephoriaServer.API/Repository/Modules/ReservationRepository.cs:133) : **AUCUN FILTRE** sur la date des séances
- Dans [`MovieRepository.GetMoviesByCinemaIdAsync(int cinemaId)`](CinephoriaServer.API/Repository/Modules/MovieRepository.cs:324) : **AUCUN FILTRE** sur la date des séances

### 2. **Incohérence dans le Filtrage des Séances**

**Problème :** Certaines méthodes filtrent les séances futures, d'autres non :
- [`ShowtimeRepository.GetUpcomingShowtimesAsync()`](CinephoriaServer.API/Repository/Modules/ShowtimeRepository.cs:119) : **FILTRE** `s.StartTime >= today`
- [`MovieRepository.GetAllMoviesWithShowtimeAsync()`](CinephoriaServer.API/Repository/Modules/MovieRepository.cs:337) : **FILTRE** `s.StartTime >= DateTime.UtcNow`
- Mais les méthodes principales de récupération des séances n'ont **AUCUN FILTRE**

## Solutions Proposées

### 1. **Correction des Méthodes de Récupération des Séances**

#### Solution pour `MovieRepository.GetMovieSessionsAsync`
```csharp
public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
{
    return await _context.Set<Showtime>()
        .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
        .OrderBy(s => s.StartTime)
        .ToListAsync();
}
```

#### Solution pour `ReservationRepository.GetMovieSessionsAsync`
```csharp
public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
{
    return await _context.Set<Showtime>()
        .Include(s => s.Movie)
        .Include(s => s.Theater)
        .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
        .OrderBy(s => s.StartTime)
        .ToListAsync();
}
```

#### Solution pour `MovieRepository.GetMoviesByCinemaIdAsync`
```csharp
public async Task<List<Movie>> GetMoviesByCinemaIdAsync(int cinemaId)
{
    return await _context.Set<Showtime>()
        .Where(s => s.CinemaId == cinemaId && s.StartTime >= DateTime.UtcNow)
        .Select(s => s.Movie)
        .Distinct()
        .ToListAsync();
}
```

### 2. **Amélioration de la Gestion des Films Récents**

#### Problème dans `MovieRepository.GetRecentMoviesAsync()`
La méthode filtre par date de création du film mais **n'inclut pas de filtre sur les séances futures**.

**Solution :**
```csharp
public async Task<List<Movie>> GetRecentMoviesAsync()
{
    DateTime lastWednesday = GetLastWednesday();
    
    return await _context.Set<Movie>()
        .Include(m => m.Showtimes)
        .Where(m => m.CreatedAt >= lastWednesday && 
                   m.Showtimes.Any(s => s.StartTime >= DateTime.UtcNow))
        .OrderByDescending(m => m.CreatedAt)
        .Take(20)
        .ToListAsync();
}
```

### 3. **Création d'une Méthode Unifiée pour les Séances Futures**

#### Nouvelle Méthode dans `IShowtimeRepository`
```csharp
Task<List<Showtime>> GetUpcomingMovieSessionsAsync(int movieId);
```

#### Implémentation dans `ShowtimeRepository`
```csharp
public async Task<List<Showtime>> GetUpcomingMovieSessionsAsync(int movieId)
{
    return await _context.Set<Showtime>()
        .Include(s => s.Movie)
        .Include(s => s.Cinema)
        .Include(s => s.Theater)
        .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
        .OrderBy(s => s.StartTime)
        .ToListAsync();
}
```

## Implémentation des Corrections

### 1. **Correction de MovieRepository**

<apply_diff>
<path>CinephoriaServer.API/Repository/Modules/MovieRepository.cs</path>
<diff>
<<<<<<< SEARCH
:start_line:159
-------
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }
=======
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
>>>>>>> REPLACE
</diff>
</apply_diff>

### 2. **Correction de ReservationRepository**

<apply_diff>
<path>CinephoriaServer.API/Repository/Modules/ReservationRepository.cs</path>
<diff>
<<<<<<< SEARCH
:start_line:133
-------
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.MovieId == movieId)
                .ToListAsync();
        }
=======
        public async Task<List<Showtime>> GetMovieSessionsAsync(int movieId)
        {
            return await _context.Set<Showtime>()
                .Include(s => s.Movie)
                .Include(s => s.Theater)
                .Where(s => s.MovieId == movieId && s.StartTime >= DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
>>>>>>> REPLACE
</diff>
</apply_diff>

### 3. **Correction de GetMoviesByCinemaIdAsync**

<apply_diff>
<path>CinephoriaServer.API/Repository/Modules/MovieRepository.cs</path>
<diff>
<<<<<<< SEARCH
:start_line:324
-------
        public async Task<List<Movie>> GetMoviesByCinemaIdAsync(int cinemaId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.CinemaId == cinemaId)
                .Select(s => s.Movie)
                .Distinct()
                .ToListAsync();
        }
=======
        public async Task<List<Movie>> GetMoviesByCinemaIdAsync(int cinemaId)
        {
            return await _context.Set<Showtime>()
                .Where(s => s.CinemaId == cinemaId && s.StartTime >= DateTime.UtcNow)
                .Select(s => s.Movie)
                .Distinct()
                .ToListAsync();
        }
>>>>>>> REPLACE
</diff>
</apply_diff>

### 4. **Amélioration de GetRecentMoviesAsync**

<apply_diff>
<path>CinephoriaServer.API/Repository/Modules/MovieRepository.cs</path>
<diff>
<<<<<<< SEARCH
:start_line:119
-------
        public async Task<List<Movie>> GetRecentMoviesAsync()
        {
            // Calculer la date du dernier mercredi
            DateTime lastWednesday = GetLastWednesday();

            // Récupérer les films ajoutés depuis le dernier mercredi avec au moins une séance
            var recentMovies = await _context.Set<Movie>()
                .Include(m => m.Showtimes)
                .Where(m => m.CreatedAt >= lastWednesday && m.Showtimes.Any())
                .OrderByDescending(m => m.CreatedAt)
                .Take(20)
                .ToListAsync();

            return recentMovies;
        }
=======
        public async Task<List<Movie>> GetRecentMoviesAsync()
        {
            // Calculer la date du dernier mercredi
            DateTime lastWednesday = GetLastWednesday();

            // Récupérer les films ajoutés depuis le dernier mercredi avec au moins une séance future
            var recentMovies = await _context.Set<Movie>()
                .Include(m => m.Showtimes)
                .Where(m => m.CreatedAt >= lastWednesday && 
                           m.Showtimes.Any(s => s.StartTime >= DateTime.UtcNow))
                .OrderByDescending(m => m.CreatedAt)
                .Take(20)
                .ToListAsync();

            return recentMovies;
        }
>>>>>>> REPLACE
</diff>
</apply_diff>

## Améliorations Supplémentaires

### 1. **Gestion des Statuts des Séances**

**Problème :** Les séances passées ne sont pas automatiquement marquées comme "Terminées"

**Solution :** Utiliser le système de statuts existant dans `ShowtimeStatus` :
- `Scheduled` → Séance programmée
- `Ongoing` → Séance en cours
- `Completed` → Séance terminée
- `Cancelled` → Séance annulée

### 2. **Archivage Automatique des Séances Passées**

**Proposition :** Créer un service d'arrière-plan qui :
- Marque automatiquement les séances passées comme "Completed"
- Archive les séances terminées après un certain délai
- Nettoie les réservations expirées

### 3. **Amélioration des Performances**

**Proposition :** Ajouter des index sur :
- `Showtime.StartTime` pour les requêtes de filtrage temporel
- `Showtime.MovieId` pour les jointures avec les films
- `Showtime.CinemaId` pour les requêtes par cinéma

## Conclusion

Les corrections proposées résoudront le problème principal des séances passées qui continuent de s'afficher. L'approche consiste à :

1. **Ajouter systématiquement des filtres temporels** dans toutes les méthodes de récupération des séances
2. **Utiliser `DateTime.UtcNow`** pour la cohérence temporelle
3. **Ordonner les résultats** par date de séance croissante
4. **Maintenir la cohérence** entre toutes les méthodes de l'application

Ces modifications garantiront que seules les séances futures sont présentées aux utilisateurs, améliorant ainsi l'expérience utilisateur et la pertinence des résultats.