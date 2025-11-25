# Guide des Migrations EF Core - Cinéphoria

## 📋 Vue d'ensemble

Ce guide explique comment utiliser Entity Framework Core pour gérer les migrations de base de données pendant le développement du projet Cinéphoria.

## 🚀 Commandes EF Core Essentielles

### Installation des Outils (si nécessaire)
```bash
dotnet tool install --global dotnet-ef
```

### Créer une Nouvelle Migration
```bash
cd CinephoriaServer.API
dotnet ef migrations add NomDeLaMigration
```

### Appliquer les Migrations
```bash
# Mettre à jour la base de données
dotnet ef database update

# Appliquer une migration spécifique
dotnet ef database update NomDeLaMigration
```

### Gérer les Migrations
```bash
# Lister les migrations
dotnet ef migrations list

# Supprimer la dernière migration (si non appliquée)
dotnet ef migrations remove

# Générer un script SQL
dotnet ef migrations script --output migration.sql
```

## 🔧 Workflow de Développement

### 1. Modifier les Modèles
```csharp
// Dans Models/PostgresqlDb/Movie/Movie.cs
public class Movie : BaseEntity
{
    // Ajouter une nouvelle propriété
    public string? NouvellePropriete { get; set; }
}
```

### 2. Créer une Migration
```bash
dotnet ef migrations add AjoutNouvelleProprieteMovie
```

### 3. Vérifier la Migration Générée
```bash
# Vérifier le fichier généré dans Migrations/
cat Migrations/20251018000000_AjoutNouvelleProprieteMovie.cs
```

### 4. Appliquer la Migration
```bash
dotnet ef database update
```

### 5. Tester l'Application
```bash
dotnet run
```

## 📁 Structure des Migrations

### Fichiers Générés
```
Migrations/
├── 20251016113708_InitialCreate.cs          # Migration initiale
├── 20251016113708_InitialCreate.Designer.cs # Métadonnées
├── 20251018041211_AddMovieColumnsAndSettings.cs # Nouvelle migration
└── CinephoriaDbContextModelSnapshot.cs      # État actuel du modèle
```

### Exemple de Migration
```csharp
public partial class AjoutNouvelleProprieteMovie : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "NouvellePropriete",
            table: "Movies",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "NouvellePropriete",
            table: "Movies");
    }
}
```

## 🔄 Gestion des Environnements

### Configuration des Chaînes de Connexion

**Development** (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=CinephoriaDevDB;Username=devUser;Password=***;"
  }
}
```

**Production** (`appsettings.Production.json`):
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=prod-server;Database=CinephoriaProdDB;Username=prodUser;Password=***;"
  }
}
```

### Variables d'Environnement
```bash
# Définir la variable d'environnement
export ASPNETCORE_ENVIRONMENT=Development
# ou
export ASPNETCORE_ENVIRONMENT=Production
```

## 🛠️ Scripts Utiles

### Script de Déploiement avec EF Core
```bash
#!/bin/bash
# deploy_with_ef.sh

echo "Déploiement avec EF Core Migrations..."

# Vérifier l'environnement
if [ "$ASPNETCORE_ENVIRONMENT" = "Production" ]; then
    echo "⚠️  Environnement Production détecté"
    read -p "Confirmer le déploiement en production? (y/n): " -n 1 -r
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Appliquer les migrations
echo "Application des migrations..."
dotnet ef database update

echo "✅ Déploiement terminé"
```

### Script de Sauvegarde et Migration
```bash
#!/bin/bash
# backup_and_migrate.sh

# Sauvegarde
BACKUP_FILE="backup_$(date +%Y%m%d_%H%M%S).sql"
echo "Création de la sauvegarde: $BACKUP_FILE"
pg_dump -h localhost -U postgres -d cinephoria_db -f $BACKUP_FILE

# Migration
echo "Application des migrations..."
dotnet ef database update

echo "✅ Sauvegarde et migration terminées"
```

## 📊 Bonnes Pratiques

### 1. Nommage des Migrations
- Utiliser des noms descriptifs : `AddUserPreferences`, `UpdateMovieSchema`
- Inclure la date dans le nom du fichier (automatique)
- Éviter les noms génériques comme `Migration1`, `Migration2`

### 2. Gestion des Données de Référence
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Ajouter des données de référence
    migrationBuilder.InsertData(
        table: "Settings",
        columns: new[] { "Key", "Value", "Description" },
        values: new object[,]
        {
            { "TMDB_API_KEY", "", "Clé API TMDB" },
            { "MAX_RESERVATIONS", "10", "Réservations max par utilisateur" }
        });
}
```

### 3. Vérifications de Sécurité
```csharp
// Toujours vérifier si une colonne existe avant de l'ajouter
migrationBuilder.Sql(@"
    DO $$ 
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM information_schema.columns 
                      WHERE table_name = 'Movies' AND column_name = 'NouvelleColonne') THEN
            ALTER TABLE ""Movies"" ADD COLUMN ""NouvelleColonne"" text;
        END IF;
    END $$;
");
```

## 🔍 Dépannage

### Problèmes Courants

**Erreur de Connexion :**
```bash
# Vérifier la chaîne de connexion
cat appsettings.Development.json

# Tester la connexion
psql -h localhost -U postgres -d cinephoria_db -c "SELECT 1;"
```

**Migration en Conflit :**
```bash
# Supprimer la dernière migration
dotnet ef migrations remove

# Recréer la migration
dotnet ef migrations add NomCorrige
```

**Base Non Synchronisée :**
```bash
# Réinitialiser la base (ATTENTION : supprime les données)
dotnet ef database drop --force
dotnet ef database update
```

### Commandes de Diagnostic
```bash
# Vérifier l'état des migrations
dotnet ef migrations list

# Vérifier le contexte
dotnet ef dbcontext info

# Générer un script de toutes les migrations
dotnet ef migrations script --idempotent --output full_migration.sql
```

## 📝 Exemple Complet

### Scénario : Ajouter un Champ de Préférences Utilisateur

1. **Modifier le modèle** :
```csharp
// Dans AppUser.cs
public class AppUser : IdentityUser
{
    public string? Preferences { get; set; } // Nouveau champ
}
```

2. **Créer la migration** :
```bash
dotnet ef migrations add AddUserPreferences
```

3. **Vérifier le fichier généré** :
```csharp
// Le fichier devrait contenir :
migrationBuilder.AddColumn<string>(
    name: "Preferences",
    table: "AspNetUsers",
    type: "text",
    nullable: true);
```

4. **Appliquer** :
```bash
dotnet ef database update
```

5. **Tester** :
```bash
dotnet run
```

## 🎯 Intégration avec le Développement

### Dans Visual Studio
- **Package Manager Console** :
```powershell
Add-Migration NomDeLaMigration
Update-Database
```

### Dans CI/CD
```yaml
# .github/workflows/deploy.yml
jobs:
  deploy:
    steps:
      - name: Apply EF Core Migrations
        run: |
          cd CinephoriaServer.API
          dotnet ef database update
```

---

**Dernière Mise à Jour** : 18/10/2024  
**Version EF Core** : 8.0.10  
**Base de Données** : PostgreSQL 15+

Pour plus d'informations, consulter la [documentation officielle EF Core](https://docs.microsoft.com/ef/core/).