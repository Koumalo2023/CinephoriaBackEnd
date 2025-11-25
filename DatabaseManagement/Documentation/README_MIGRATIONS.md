# 📚 Migrations Base de Données - Cinéphoria

Ce dossier contient tous les scripts et la documentation nécessaires pour gérer les migrations de la base de données Cinéphoria.

## 📁 Fichiers Disponibles

### 📋 Documentation
- **[`MIGRATION_GUIDE.md`](MIGRATION_GUIDE.md)** - Guide complet de migration et bonnes pratiques
- **[`README_MIGRATIONS.md`](README_MIGRATIONS.md)** - Ce fichier, vue d'ensemble des migrations

### 🔧 Scripts de Migration
- **[`migration_script.sql`](migration_script.sql)** - Script initial de création des tables
- **[`simple_migration.sql`](simple_migration.sql)** - Migration ciblée pour résoudre les incohérences
- **[`targeted_migration.sql`](targeted_migration.sql)** - Version avancée avec vérifications

### ✅ Scripts de Validation
- **[`final_validation.sql`](final_validation.sql)** - Validation complète après migration
- **[`check_database_status.sql`](check_database_status.sql)** - Vérification de l'état de la base à tout moment

### 🚀 Scripts de Déploiement Automatique
- **[`deploy_database.bat`](deploy_database.bat)** - Déploiement automatique Windows
- **[`deploy_database.sh`](deploy_database.sh)** - Déploiement automatique Linux/Mac

### 📊 Documentation Historique
- **[`PLAN_RESOLUTION_INCOHERENCES.md`](PLAN_RESOLUTION_INCOHERENCES.md)** - Plan de résolution des incohérences
- **[`TODO_EXECUTION_PLAN.md`](TODO_EXECUTION_PLAN.md)** - Plan d'exécution détaillé

## 🎯 Utilisation Rapide

### Pour un Nouvel Environnement

**Option 1 - Automatique (Recommandé) :**
```bash
# Windows
deploy_database.bat

# Linux/Mac
chmod +x deploy_database.sh
./deploy_database.sh
```

**Option 2 - Manuel :**
```bash
# Créer la base
psql -h localhost -U postgres -c "CREATE DATABASE cinephoria_db;"

# Appliquer les migrations
psql -h localhost -U postgres -d cinephoria_db -f migration_script.sql
psql -h localhost -U postgres -d cinephoria_db -f simple_migration.sql

# Valider
psql -h localhost -U postgres -d cinephoria_db -f final_validation.sql
```

### Pour Vérifier l'État Actuel
```bash
psql -h localhost -U postgres -d cinephoria_db -f check_database_status.sql
```

## 🔄 Gestion des Migrations Futures

### Créer une Nouvelle Migration

1. **Nommer le fichier** : `migration_YYYYMMDD_description.sql`
2. **Utiliser le modèle** :
```sql
-- Vérifier si la modification existe déjà
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE ...) THEN
        -- Appliquer la modification
        ALTER TABLE ... ADD COLUMN ...;
    END IF;
END $$;

-- Créer les index nécessaires
CREATE INDEX IF NOT EXISTS ...;

-- Valider
\echo 'Migration YYYYMMDD_description appliquée'
```

3. **Tester** sur un environnement de staging
4. **Documenter** dans [`MIGRATION_GUIDE.md`](MIGRATION_GUIDE.md)

### Exemple de Migration Future
```sql
-- migration_20241101_add_user_preferences.sql
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_name = 'AspNetUsers' AND column_name = 'Preferences'
    ) THEN
        ALTER TABLE "AspNetUsers" ADD COLUMN "Preferences" jsonb;
        CREATE INDEX IF NOT EXISTS "IX_AspNetUsers_Preferences" ON "AspNetUsers" USING gin ("Preferences");
    END IF;
END $$;
```

## 📊 État Actuel de la Base

### Tables Principales (19 tables)
- **Movies** (17 colonnes) - Films avec métadonnées complètes
- **MovieSimilars** - Relations entre films similaires  
- **settings** - Configuration système (6 paramètres)
- **AspNetUsers** - Utilisateurs et authentification
- **UserMovieHistories** - Historique de visionnage
- **Showtimes**, **Reservations**, **Seats** - Gestion des séances
- **Cinemas**, **Theaters** - Gestion des cinémas
- **Incidents** - Gestion des incidents

### Colonnes Ajoutées Récemment (Movies)
- **`Actors`** (text[]) - Liste des acteurs du film
- **`BandeAnnonce`** (text) - URL de la bande-annonce
- **`TmdbId`** (integer) - ID TMDB pour intégration

### Index de Performance (8 index)
- Recherche par titre, genre, TmdbId
- Optimisation des relations MovieSimilars
- Index sur les clés étrangères critiques

## 🛡️ Sécurité et Sauvegarde

### Avant Toute Migration
```bash
# Sauvegarde
pg_dump -h localhost -U postgres -d cinephoria_db -f backup_$(date +%Y%m%d).sql

# Restauration (si nécessaire)
psql -h localhost -U postgres -d cinephoria_db -f backup_YYYYMMDD.sql
```

### Variables d'Environnement Recommandées
```bash
export PG_HOST=localhost
export PG_USER=postgres
export PG_DB=cinephoria_db
```

## 📞 Support et Dépannage

### Problèmes Courants
1. **Connexion refusée** - Vérifier que PostgreSQL est démarré
2. **Permissions insuffisantes** - Utiliser un utilisateur avec droits adéquats
3. **Tables déjà existantes** - Les scripts utilisent `IF NOT EXISTS`

### Vérifications
```sql
-- Vérifier la connexion
SELECT version();

-- Vérifier les tables
\dt

-- Vérifier une table spécifique
SELECT * FROM information_schema.columns WHERE table_name = 'Movies';
```

---

**Dernière Migration** : 18/10/2024  
**Version Base** : 1.1.0  
**Statut** : ✅ Opérationnelle  
**Prochaine Revue** : 18/11/2024

Pour toute question, consulter [`MIGRATION_GUIDE.md`](MIGRATION_GUIDE.md) ou contacter l'équipe de développement.