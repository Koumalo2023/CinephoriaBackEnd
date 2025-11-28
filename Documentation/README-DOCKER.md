# Configuration Docker - Cinephoria

Ce document décrit la configuration Docker pour l'application Cinephoria, incluant les services backend, frontend, et bases de données.

## 🏗️ Architecture des Services

### Services Déployés

| Service | Port | Description | Health Check |
|---------|------|-------------|--------------|
| PostgreSQL | 5432 | Base de données principale | `pg_isready` |
| MongoDB | 27017 | Base de données NoSQL | TCP check |
| Backend API | 5000 | API .NET Core | `/health/live`, `/health/ready` |
| Frontend Angular | 4200 | Application web | `/health` |
| pgAdmin | 5050 | Interface d'administration PostgreSQL | - |

## 🚀 Démarrage Rapide

### Prérequis
- Docker Desktop 20.10+
- Docker Compose 2.0+

### Commandes de Base

```bash
# Build des images
docker compose build

# Démarrage des services
docker compose up -d

# Arrêt des services
docker compose down

# Voir les logs
docker compose logs -f

# Vérifier l'état des services
docker compose ps

# Tester les health checks
.\scripts\test-health-checks.ps1
```

## 🔧 Configuration des Services

### Backend (.NET Core)
- **Image**: `mcr.microsoft.com/dotnet/aspnet:8.0`
- **Port**: 5000
- **Variables d'environnement**:
  - `ConnectionStrings__DefaultConnection`: PostgreSQL
  - `ConnectionStrings__MongoDB`: MongoDB
  - `ASPNETCORE_ENVIRONMENT`: Development/Production

### Frontend (Angular)
- **Build**: Node.js 18
- **Runtime**: Nginx Alpine
- **Port**: 4200
- **Health Check**: `/health` (retourne HTTP 200)

### PostgreSQL
- **Image**: `postgres:15`
- **Port**: 5432
- **Base de données**: `cinephoria`
- **Utilisateur**: `cinephoria_user`
- **Mot de passe**: `cinephoria_password`

### MongoDB
- **Image**: `mongo:6`
- **Port**: 27017
- **Base de données**: `cinephoria`

## 🛠️ Développement Local

### Variables d'Environnement

Créez un fichier `.env` à la racine du projet :

```env
# PostgreSQL
POSTGRES_DB=cinephoria
POSTGRES_USER=cinephoria_user
POSTGRES_PASSWORD=cinephoria_password

# Backend
ASPNETCORE_ENVIRONMENT=Development
```

### Commandes Utiles

```bash
# Accéder à un conteneur
docker compose exec backend bash

# Voir les logs d'un service spécifique
docker compose logs backend -f

# Redémarrer un service
docker compose restart backend

# Rebuild une image spécifique
docker compose build backend

# Nettoyer les conteneurs et volumes
docker compose down -v
```

## 📊 Health Checks

### Backend Health Endpoints

- **Live**: `GET http://localhost:5000/health/live`
  - Vérifie que l'application est démarrée
- **Ready**: `GET http://localhost:5000/health/ready`
  - Vérifie les connexions aux bases de données

### Frontend Health Endpoint

- **Health**: `GET http://localhost:4200/health`
  - Vérifie que Nginx sert correctement

### Script de Test Automatisé

Utilisez le script PowerShell pour tester tous les health checks :

```powershell
.\scripts\test-health-checks.ps1
```

## 🔒 Sécurité

### Configuration Sécurisée

- **Utilisateurs non-root**: Les conteneurs backend et frontend s'exécutent avec des utilisateurs non-privilégiés
- **Health checks**: Monitoring intégré de l'état des services
- **Nginx**: Configuration sécurisée avec en-têtes CSP
- **Volumes nommés**: Isolation des données

### Bonnes Pratiques

1. **Ne jamais exposer les mots de passe en clair**
2. **Utiliser des secrets Docker pour la production**
3. **Mettre à jour régulièrement les images de base**
4. **Scanner les images pour les vulnérabilités**

## 🐛 Dépannage

### Problèmes Courants

#### Ports déjà utilisés
```bash
# Vérifier les ports utilisés
netstat -ano | findstr :5000

# Changer les ports dans docker-compose.yml
ports:
  - "5001:5000"
```

#### Connexion aux bases de données
```bash
# Vérifier que les conteneurs DB sont démarrés
docker compose ps

# Tester la connexion PostgreSQL
docker compose exec postgres psql -U cinephoria_user -d cinephoria
```

#### Build échoue
```bash
# Nettoyer le cache Docker
docker system prune -f

# Rebuild sans cache
docker compose build --no-cache
```

### Logs Détaillés

```bash
# Logs complets
docker compose logs

# Logs d'un service spécifique
docker compose logs backend

# Logs en temps réel
docker compose logs -f
```

## 📈 Monitoring

### Métriques de Performance

- **Mémoire**: `docker stats`
- **CPU**: `docker stats`
- **Logs**: `docker compose logs`
- **Health**: Scripts de test automatiques

### Alertes

Configurez des alertes basées sur :
- Échec des health checks
- Consommation mémoire excessive
- Erreurs dans les logs d'application