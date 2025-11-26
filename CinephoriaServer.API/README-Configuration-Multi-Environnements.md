# Configuration Multi-Environnements - Cinephoria Backend

## 📋 Vue d'ensemble

Ce document décrit la configuration multi-environnements mise en place pour l'API Cinephoria. Le système utilise les fichiers `appsettings.*.json` standard d'ASP.NET Core avec une extension pour l'environnement AWS.

## 🏗️ Structure des fichiers de configuration

### Fichiers disponibles

1. **`appsettings.json`** - Configuration de base (valeurs par défaut)
2. **`appsettings.Development.json`** - Environnement de développement local
3. **`appsettings.Staging.json`** - Environnement de pré-production
4. **`appsettings.Production.json`** - Environnement de production
5. **`appsettings.AWS.json`** - Configuration spécifique AWS

## 🔧 Configuration par environnement

### Développement (Development)
- **Logging** : Niveau Debug détaillé
- **Base de données** : PostgreSQL local + MongoDB local
- **CORS** : Origines locales autorisées
- **Swagger** : Activé
- **HTTPS** : Localhost avec certificat de développement

### Staging
- **Logging** : Niveau Information
- **Base de données** : Serveurs de staging
- **CORS** : Domaines de staging
- **Swagger** : Activé (pour les tests)
- **HTTPS** : Production-like

### Production
- **Logging** : Niveau Warning/Error uniquement
- **Base de données** : Serveurs de production
- **CORS** : Domaines de production
- **Swagger** : Désactivé
- **HTTPS** : Obligatoire
- **Rate Limiting** : Activé

### AWS
- **Logging** : Niveau Information avec CloudWatch
- **Base de données** : RDS PostgreSQL + MongoDB Atlas
- **Email** : Amazon SES
- **Stockage** : Amazon S3 pour les images
- **Secrets** : AWS Secrets Manager

## 🚀 Utilisation

### Variables d'environnement

Tous les secrets et valeurs sensibles doivent être configurés via des variables d'environnement :

```bash
# Exemple pour le développement
setx JWT__Secret "votre_secret_jwt"
setx ConnectionStrings__PostgreSQL "votre_connection_string"
setx TMDbSettings__ApiKey "votre_api_key_tmdb"
```

### Démarrage par environnement

```bash
# Développement (par défaut)
dotnet run

# Staging
dotnet run --environment Staging

# Production  
dotnet run --environment Production

# AWS
dotnet run --environment AWS
```

### Dans les conteneurs Docker

```dockerfile
# Définir l'environnement
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
```

## 🔒 Sécurité

### Bonnes pratiques

1. **Ne jamais commiter les secrets** dans les fichiers de configuration
2. **Utiliser les variables d'environnement** pour les valeurs sensibles
3. **Valider les configurations** au démarrage de l'application
4. **Utiliser Azure Key Vault / AWS Secrets Manager** en production

### Variables à remplacer

Dans chaque fichier `appsettings.*.json`, recherchez les chaînes contenant `REPLACE_IN_ENV_VARIABLES` et remplacez-les par des variables d'environnement :

- `JWT:Secret`
- `SmtpSettings:Password` 
- `ConnectionStrings:PostgreSQL`
- `TMDbSettings:ApiKey`
- Toutes les valeurs AWS

## 🧪 Tests de configuration

### Vérification au démarrage

L'application valide les configurations essentielles au démarrage :

```csharp
// Dans Program.cs
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'PostgreSQL' not found.");
}
```

### Health Checks

Des endpoints de santé sont disponibles pour vérifier la connectivité aux bases de données :

- `/health` - Statut général
- `/health/database` - Statut PostgreSQL
- `/health/mongodb` - Statut MongoDB

## 📁 Structure des configurations

### Sections communes à tous les environnements

- `Logging` - Configuration des logs
- `AllowedHosts` - Sécurité des hôtes
- `JWT` - Configuration d'authentification
- `ConnectionStrings` - Chaînes de connexion BDD
- `MongoDbSettings` - Configuration MongoDB
- `TMDbSettings` - API The Movie Database
- `CorsSettings` - Politique CORS
- `FeatureFlags` - Fonctionnalités conditionnelles

### Sections spécifiques

- `AWS` - Configuration cloud AWS (uniquement dans appsettings.AWS.json)
- `HealthChecks` - Configuration des vérifications de santé
- `Security` - Paramètres de sécurité avancés

## 🔄 Ordre de chargement

Les configurations sont chargées dans cet ordre (les dernières écrasent les premières) :

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. `appsettings.AWS.json` (si spécifié)
4. Variables d'environnement
5. User Secrets (développement uniquement)

## 🛠️ Résolution de problèmes

### Logs de configuration

Activez les logs détaillés pour voir quelle configuration est chargée :

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.Extensions.Configuration": "Debug"
    }
  }
}
```

### Validation des variables d'environnement

Utilisez la commande suivante pour lister les variables chargées :

```bash
dotnet run --configuration Debug --verbosity detailed
```

### Erreurs courantes

1. **Connection string manquante** : Vérifiez les variables d'environnement
2. **Secrets non chargés** : Vérifiez User Secrets en développement
3. **CORS bloqué** : Vérifiez les origines autorisées
4. **Certificats HTTPS** : Vérifiez les certificats en production