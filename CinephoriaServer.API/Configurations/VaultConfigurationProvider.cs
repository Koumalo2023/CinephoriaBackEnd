using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using CinephoriaServer.API.Services;

namespace CinephoriaServer.API.Configurations
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, string> _vaultMappings;

        public VaultConfigurationProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Mappings des chemins de configuration vers les chemins Vault
            _vaultMappings = new Dictionary<string, string>
            {
                ["JWT:Secret"] = "secret/cinephoria/jwt:secret",
                ["JWT:ValidIssuer"] = "secret/cinephoria/jwt:issuer",
                ["JWT:ValidAudience"] = "secret/cinephoria/jwt:audience",
                ["ConnectionStrings:PostgreSQL"] = "secret/cinephoria/database/postgres:connection_string",
                ["MongoDbSettings:ConnectionString"] = "secret/cinephoria/database/mongodb:connection_string",
                ["MongoDbSettings:DatabaseName"] = "secret/cinephoria/database/mongodb:database_name",
                ["SmtpSettings:Username"] = "secret/cinephoria/smtp:username",
                ["SmtpSettings:Password"] = "secret/cinephoria/smtp:password",
                ["SmtpSettings:Server"] = "secret/cinephoria/smtp:server",
                ["SmtpSettings:Port"] = "secret/cinephoria/smtp:port",
                ["TMDbSettings:ApiKey"] = "secret/cinephoria/tmdb:api_key"
            };
        }

        public override void Load()
        {
            using var scope = _serviceProvider.CreateScope();
            var vaultService = scope.ServiceProvider.GetRequiredService<IVaultService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<VaultConfigurationProvider>>();
            
            logger.LogInformation("Chargement de la configuration depuis Vault...");
            
            try
            {
                var data = new Dictionary<string, string>();
                
                // Charger chaque secret depuis Vault
                foreach (var mapping in _vaultMappings)
                {
                    var vaultPath = mapping.Value.Split(':')[0];
                    var vaultKey = mapping.Value.Split(':')[1];
                    
                    var secretValue = vaultService.GetSecretAsync(vaultPath, vaultKey).GetAwaiter().GetResult();
                    
                    if (!string.IsNullOrEmpty(secretValue))
                    {
                        data[mapping.Key] = secretValue;
                        logger.LogDebug("Secret chargé depuis Vault: {Key} -> {VaultPath}/{VaultKey}",
                            mapping.Key, vaultPath, vaultKey);
                    }
                    else
                    {
                        logger.LogWarning("Secret non trouvé dans Vault: {VaultPath}/{VaultKey}",
                            vaultPath, vaultKey);
                    }
                }
                
                Data = data;
                logger.LogInformation("Configuration Vault chargée avec {Count} secrets", data.Count);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, "Erreur lors du chargement de la configuration Vault");
                // En cas d'erreur, on laisse les valeurs par défaut des appsettings
            }
        }
    }

    public class VaultConfigurationSource : IConfigurationSource
    {
        private readonly System.IServiceProvider _serviceProvider;

        public VaultConfigurationSource(System.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VaultConfigurationProvider(_serviceProvider);
        }
    }

    public static class VaultConfigurationExtensions
    {
        public static IConfigurationBuilder AddVaultConfiguration(
            this IConfigurationBuilder builder, 
            System.IServiceProvider serviceProvider)
        {
            return builder.Add(new VaultConfigurationSource(serviceProvider));
        }
    }
}