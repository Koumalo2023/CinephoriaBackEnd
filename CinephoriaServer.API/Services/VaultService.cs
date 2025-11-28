using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;
using VaultSharp.V1.AuthMethods;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinephoriaServer.API.Services
{
    public interface IVaultService
    {
        Task<string> GetSecretAsync(string path, string key);
        Task<Dictionary<string, object>> GetSecretsAsync(string path);
        Task<bool> IsVaultAvailableAsync();
    }

    public class VaultService : IVaultService
    {
        private readonly IVaultClient _vaultClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<VaultService> _logger;
        private readonly string _vaultAddress;
        private readonly string _vaultToken;

        public VaultService(IConfiguration configuration, IMemoryCache cache, ILogger<VaultService> logger)
        {
            _vaultAddress = configuration["Vault:Address"] ?? "http://vault:8200";
            _vaultToken = configuration["Vault:Token"] ?? "cinephoria-vault-root-token";
            _cache = cache;
            _logger = logger;

            // Configuration du client Vault
            var vaultClientSettings = new VaultClientSettings(_vaultAddress, new TokenAuthMethodInfo(_vaultToken))
            {
                Namespace = "",
                ContinueAsyncTasksOnCapturedContext = false
            };

            _vaultClient = new VaultClient(vaultClientSettings);
        }

        public async Task<string> GetSecretAsync(string path, string key)
        {
            try
            {
                var cacheKey = $"{path}:{key}";
                
                // Vérifier le cache d'abord
                if (_cache.TryGetValue(cacheKey, out string cachedSecret))
                {
                    _logger.LogDebug("Secret récupéré depuis le cache: {Path}/{Key}", path, key);
                    return cachedSecret;
                }

                _logger.LogInformation("Récupération du secret depuis Vault: {Path}/{Key}", path, key);
                
                // Récupérer le secret depuis Vault
                Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                    path: path,
                    mountPoint: "secret");

                if (secret?.Data?.Data != null && secret.Data.Data.ContainsKey(key))
                {
                    var secretValue = secret.Data.Data[key].ToString();
                    
                    // Mettre en cache pour 5 minutes
                    _cache.Set(cacheKey, secretValue, TimeSpan.FromMinutes(5));
                    
                    _logger.LogDebug("Secret récupéré avec succès: {Path}/{Key}", path, key);
                    return secretValue;
                }

                _logger.LogWarning("Secret non trouvé: {Path}/{Key}", path, key);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du secret: {Path}/{Key}", path, key);
                throw new VaultException($"Erreur Vault: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, object>> GetSecretsAsync(string path)
        {
            try
            {
                var cacheKey = $"secrets:{path}";
                
                // Vérifier le cache d'abord
                if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cachedSecrets))
                {
                    _logger.LogDebug("Secrets récupérés depuis le cache: {Path}", path);
                    return cachedSecrets;
                }

                _logger.LogInformation("Récupération des secrets depuis Vault: {Path}", path);
                
                // Récupérer tous les secrets du chemin
                Secret<SecretData> secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
                    path: path,
                    mountPoint: "secret");

                if (secret?.Data?.Data != null)
                {
                    var secrets = secret.Data.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    
                    // Mettre en cache pour 5 minutes
                    _cache.Set(cacheKey, secrets, TimeSpan.FromMinutes(5));
                    
                    _logger.LogDebug("Secrets récupérés avec succès: {Path} ({Count} secrets)", path, secrets.Count);
                    return secrets;
                }

                _logger.LogWarning("Aucun secret trouvé: {Path}", path);
                return new Dictionary<string, object>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des secrets: {Path}", path);
                throw new VaultException($"Erreur Vault: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsVaultAvailableAsync()
        {
            try
            {
                var status = await _vaultClient.V1.System.GetHealthStatusAsync();
                _logger.LogInformation("Vault status: {Initialized}, {Sealed}, {Standby}", status.Initialized, status.Sealed, status.Standby);
                return status.Initialized && !status.Sealed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vault n'est pas disponible");
                return false;
            }
        }

        public async Task<Dictionary<string, string>> GetConfigurationAsync()
        {
            var configuration = new Dictionary<string, string>();
            
            try
            {
                // Récupérer tous les secrets de configuration
                var jwtSecrets = await GetSecretsAsync("cinephoria/jwt");
                var dbSecrets = await GetSecretsAsync("cinephoria/database/postgres");
                var mongoSecrets = await GetSecretsAsync("cinephoria/database/mongodb");
                var smtpSecrets = await GetSecretsAsync("cinephoria/smtp");
                var tmdbSecrets = await GetSecretsAsync("cinephoria/tmdb");
                
                // Mapper vers la structure de configuration
                if (jwtSecrets != null)
                {
                    configuration["JWT:Secret"] = jwtSecrets.GetValueOrDefault("secret")?.ToString();
                    configuration["JWT:ValidIssuer"] = jwtSecrets.GetValueOrDefault("issuer")?.ToString();
                    configuration["JWT:ValidAudience"] = jwtSecrets.GetValueOrDefault("audience")?.ToString();
                }
                
                if (dbSecrets != null)
                {
                    configuration["ConnectionStrings:PostgreSQL"] = dbSecrets.GetValueOrDefault("connection_string")?.ToString();
                }
                
                if (mongoSecrets != null)
                {
                    configuration["MongoDbSettings:ConnectionString"] = mongoSecrets.GetValueOrDefault("connection_string")?.ToString();
                    configuration["MongoDbSettings:DatabaseName"] = mongoSecrets.GetValueOrDefault("database_name")?.ToString();
                }
                
                if (smtpSecrets != null)
                {
                    configuration["SmtpSettings:Server"] = smtpSecrets.GetValueOrDefault("server")?.ToString();
                    configuration["SmtpSettings:Port"] = smtpSecrets.GetValueOrDefault("port")?.ToString();
                    configuration["SmtpSettings:Username"] = smtpSecrets.GetValueOrDefault("username")?.ToString();
                    configuration["SmtpSettings:Password"] = smtpSecrets.GetValueOrDefault("password")?.ToString();
                }
                
                if (tmdbSecrets != null)
                {
                    configuration["TMDbSettings:ApiKey"] = tmdbSecrets.GetValueOrDefault("api_key")?.ToString();
                }
                
                _logger.LogInformation("Configuration Vault chargée avec {Count} éléments", configuration.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement de la configuration Vault");
            }
            
            return configuration;
        }
    }

    public class VaultException : Exception
    {
        public VaultException(string message) : base(message) { }
        public VaultException(string message, Exception innerException) : base(message, innerException) { }
    }
}