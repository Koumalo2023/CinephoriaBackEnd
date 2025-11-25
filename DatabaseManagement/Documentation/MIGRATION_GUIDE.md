# Guide de Migration - Base de Données Cinéphoria

## 📋 Vue d'ensemble

Ce guide documente la stratégie de migration de la base de données pour le projet Cinéphoria. Il contient tous les scripts nécessaires pour déployer et mettre à jour la base de données dans de nouveaux environnements.

## 🗂️ Structure des Scripts

### Scripts de Base
- [`migration_script.sql`](migration_script.sql) - Script initial de création des tables
- [`simple_migration.sql`](simple_migration.sql) - Migration ciblée pour les mises à jour
- [`final_validation.sql`](final_validation.sql) - Validation des migrations appliquées

### Scripts de Déploiement
- [`deploy_database.bat`](deploy_database.bat) - Script Windows pour déploiement automatique
- [`deploy_database.sh`](deploy_database.sh) - Script Linux/Mac pour déploiement automatique

## 🚀 Procédure de Déploiement

### 1. Déploiement Initial (Nouvel Environnement)

```bash
# Créer la base de données
psql -h localhost -U postgres -c "CREATE DATABASE cinephoria_db;"

# Exécuter le script de migration initial
psql -h localhost -U postgres -d cinephoria_db -f migration_script.sql

# Appliquer les mises à jour
psql -h localhost -U postgres -d cinephoria_db -f simple_migration.sql

# Valider le déploiement
psql -h localhost -U postgres -d cinephoria_db -f final_validation.sql
```

### 2. Déploiement Automatisé

Utiliser les scripts de déploiement selon votre système d'exploitation :

**Windows :**
```cmd
deploy_database.bat
```

**Linux/Mac :**
```bash
chmod +x deploy_database.sh
./deploy_database.sh
```

## 🔄 Gestion des Migrations Futures

### Quand créer une nouvelle migration ?

- **Ajout de nouvelles colonnes** dans les tables existantes
- **Création de nouvelles tables**
- **Modification des contraintes** (FK, CHECK, UNIQUE)
- **Ajout d'index** pour les performances
- **Changement des types de données**

### Structure d'une Migration

```sql
-- migration_YYYYMMDD_description.sql
-- Exemple: migration_20241018_add_user_preferences.sql

-- 1. Vérifier si la modification existe déjà
DO $$ 
BEGIN
    IF NOT EXISTS (SELECT 1 FROM information_schema.columns WHERE ...) THEN
        -- Appliquer la modification
        ALTER TABLE ... ADD COLUMN ...;
    END IF;
END $$;

-- 2. Créer les index nécessaires
CREATE INDEX IF NOT EXISTS ...;

-- 3. Mettre à jour les données si nécessaire
UPDATE ... SET ... WHERE ...;

-- 4. Valider la migration
\echo 'Migration YYYYMMDD_description appliquée avec succès'
```

## 📊 Scripts de Validation

### Vérification de l'état de la base

```sql
-- Vérifier les tables
SELECT table_name FROM information_schema.tables WHERE table_schema = 'public';

-- Vérifier les colonnes d'une table spécifique
SELECT column_name, data_type, is_nullable 
FROM information_schema.columns 
WHERE table_name = 'Movies' 
ORDER BY ordinal_position;

-- Vérifier les contraintes FK
SELECT tc.table_name, tc.constraint_name
FROM information_schema.table_constraints tc
WHERE tc.constraint_type = 'FOREIGN KEY';
```

### Vérification des performances

```sql
-- Vérifier les index
SELECT indexname, tablename 
FROM pg_indexes 
WHERE schemaname = 'public'
ORDER BY tablename, indexname;

-- Statistiques des tables
SELECT schemaname, tablename, n_live_tup as row_count
FROM pg_stat_user_tables
ORDER BY n_live_tup DESC;
```

## 🛡️ Bonnes Pratiques

### Sécurité
- Toujours sauvegarder la base avant les migrations
- Utiliser des transactions pour les modifications critiques
- Tester les migrations sur un environnement de staging

### Performance
- Créer des index pour les colonnes fréquemment interrogées
- Utiliser `IF NOT EXISTS` pour éviter les erreurs
- Valider les performances après les migrations

### Maintenance
- Documenter chaque migration avec sa date et son objectif
- Conserver les scripts de migration dans le repository
- Automatiser le processus de déploiement

## 📝 Historique des Migrations

### Migration du 18/10/2024
- **Objectif** : Résoudre les incohérences entre modèles C# et schéma BD
- **Modifications** :
  - Ajout des colonnes `Actors`, `BandeAnnonce`, `TmdbId` à la table `Movies`
  - Création de la table `MovieSimilars`
  - Création de la table `settings` avec paramètres par défaut
  - Création d'index de performance
- **Scripts utilisés** : [`simple_migration.sql`](simple_migration.sql)
- **Validation** : [`final_validation.sql`](final_validation.sql)

## 🆘 Dépannage

### Problèmes courants

1. **Erreur de connexion** : Vérifier les paramètres de connexion PostgreSQL
2. **Permissions insuffisantes** : S'assurer que l'utilisateur a les droits nécessaires
3. **Tables déjà existantes** : Utiliser `IF NOT EXISTS` dans les scripts
4. **Contraintes FK violées** : Vérifier l'intégrité des données avant suppression

### Logs et Monitoring

```sql
-- Vérifier les dernières erreurs
SELECT * FROM pg_stat_activity WHERE state = 'active';

-- Surveillance des performances
SELECT * FROM pg_stat_database WHERE datname = 'cinephoria_db';
```

---

**Dernière mise à jour** : 18/10/2024  
**Version de la base** : 1.1.0  
**Responsable** : Équipe de développement Cinéphoria