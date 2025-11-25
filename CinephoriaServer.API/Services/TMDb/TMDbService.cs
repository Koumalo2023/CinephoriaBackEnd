using System.Text.Json;
using CinephoriaServer.API.Configurations;
using CinephoriaServer.API.Models;
using Microsoft.Extensions.Options;

namespace CinephoriaServer.API.Services
{
    public class TMDbService : ITMDbService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TMDbService> _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://api.themoviedb.org/3";

        public TMDbService(HttpClient httpClient, ILogger<TMDbService> logger, IOptions<TMDbSettings> settings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = settings.Value.ApiKey;
        }

        public async Task<TMDbSearchResult> SearchMoviesAsync(string query, int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&page={page}&language=fr-FR";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la recherche TMDb : {StatusCode} - {Reason}", response.StatusCode, response.ReasonPhrase);
                    throw new ApiException($"Erreur TMDb: {response.StatusCode}", (int)response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TMDbSearchResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Recherche TMDb réussie : {Query} - {Count} résultats", query, result?.Results?.Count ?? 0);
                return result ?? new TMDbSearchResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche TMDb pour : {Query}", query);
                throw new ApiException("Erreur lors de la recherche TMDb", 500);
            }
        }

        public async Task<TMDbMovieDetails> GetMovieDetailsAsync(int tmdbId)
        {
            try
            {
                var url = $"{_baseUrl}/movie/{tmdbId}?api_key={_apiKey}&append_to_response=credits&language=fr-FR";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la récupération des détails TMDb pour l'ID {TmdbId} : {StatusCode}", tmdbId, response.StatusCode);
                    throw new ApiException($"Film TMDb non trouvé", 404);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TMDbMovieDetails>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Détails TMDb récupérés pour l'ID {TmdbId} : {Title}", tmdbId, result?.Title);
                return result ?? throw new ApiException("Données TMDb invalides", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des détails TMDb pour l'ID {TmdbId}", tmdbId);
                throw new ApiException("Erreur lors de la récupération des détails du film", 500);
            }
        }

        public async Task<TMDbSearchResult> GetPopularMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&page={page}&language=fr-FR";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la récupération des films populaires TMDb : {StatusCode}", response.StatusCode);
                    throw new ApiException($"Erreur TMDb: {response.StatusCode}", (int)response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TMDbSearchResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Films populaires TMDb récupérés - {Count} résultats", result?.Results?.Count ?? 0);
                return result ?? new TMDbSearchResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films populaires TMDb");
                throw new ApiException("Erreur lors de la récupération des films populaires", 500);
            }
        }

        public async Task<TMDbSearchResult> GetUpcomingMoviesAsync(int page = 1)
        {
            try
            {
                var url = $"{_baseUrl}/movie/upcoming?api_key={_apiKey}&page={page}&language=fr-FR";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la récupération des films à venir TMDb : {StatusCode}", response.StatusCode);
                    throw new ApiException($"Erreur TMDb: {response.StatusCode}", (int)response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TMDbSearchResult>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Films à venir TMDb récupérés - {Count} résultats", result?.Results?.Count ?? 0);
                return result ?? new TMDbSearchResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des films à venir TMDb");
                throw new ApiException("Erreur lors de la récupération des films à venir", 500);
            }
        }

        public async Task<TMDbVideoResponse> GetMovieVideosAsync(int tmdbId)
        {
            try
            {
                var url = $"{_baseUrl}/movie/{tmdbId}/videos?api_key={_apiKey}&language=fr-FR";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de la récupération des vidéos TMDb pour l'ID {TmdbId} : {StatusCode}", tmdbId, response.StatusCode);
                    throw new ApiException($"Vidéos TMDb non trouvées", 404);
                }

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TMDbVideoResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _logger.LogInformation("Vidéos TMDb récupérées pour l'ID {TmdbId} - {Count} résultats", tmdbId, result?.Results?.Count ?? 0);
                return result ?? new TMDbVideoResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des vidéos TMDb pour l'ID {TmdbId}", tmdbId);
                throw new ApiException("Erreur lors de la récupération des vidéos du film", 500);
            }
        }
    }
}