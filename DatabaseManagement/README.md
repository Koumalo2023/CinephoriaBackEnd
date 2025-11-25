# 📊 Gestion de la Base de Données - Cinéphoria

Ce dossier contient tous les outils, scripts et documentation pour la gestion de la base de données du projet Cinéphoria.

## 📁 Structure du Dossier

```
DatabaseManagement/
├── 📚 Documentation/          # Guides et documentation
├── 🚀 Deployment/            # Scripts de déploiement automatique
├── 🔧 Scripts/               # Scripts SQL de migration
├── ✅ Validation/            # Scripts de vérification et validation
└── 📋 README.md             # Ce fichier
```

## 📚 Documentation

### Guides Principaux
- **[`MIGRATION_GUIDE.md`](Documentation/MIGRATION_GUIDE.md)** - Guide complet des migrations
- **[`EF_CORE_MIGRATIONS_GUIDE.md`](Documentation/EF_CORE_MIGRATIONS_GUIDE.md)** - Guide EF Core pour le développement
- **[`README_MIGRATIONS.md`](Documentation/README_MIGRATIONS.md)** - Vue d'ensemble des migrations

### Documentation Technique
- **[`PLAN_RESOLUTION_INCOHERENCES.md`](Documentation/PLAN_RESOLUTION_INCOHERENCES.md)** - Plan de résolution des incohérences
- **[`TODO_EXECUTION_PLAN.md`](Documentation/TODO_EXECUTION_PLAN.md)** - Plan d'exécution détaillé

## 🚀 Déploiement

### Scripts de Déploiement Automatique
- **[`deploy_database.bat`](Deployment/deploy_database.bat)** - Déploiement Windows
- **[`deploy_database.sh`](Deployment/deploy_database.sh)** - Déploiement Linux/Mac

### Utilisation
```bash
# Windows
DatabaseManagement\Deployment\deploy_database.bat

# Linux/Mac
chmod +x DatabaseManagement/Deployment/deploy_database.sh
DatabaseManagement/Deployment/deploy_database.sh
```

## 🔧 Scripts SQL

### Migrations Principales
- **[`migration_script.sql`](Scripts/migration_script.sql)** - Script initial de création
- **[`simple_migration.sql`](Scripts/simple_migration.sql)** - Migration ciblée pour résoudre les incohérences
- **[`targeted_migration.sql`](Scripts/targeted_migration.sql)** - Version avancée avec vérifications

### Scripts Supplémentaires
- **[`remove_appuserid_from_movies.sql`](Scripts/remove_appuserid_from_movies.sql)** - Nettoyage spécifique
- **[`JOUR1_AUTOMATION.bat`](Scripts/JOUR1_AUTOMATION.bat)** - Automatisation jour 1

## ✅ Validation

### Vérification de l'État
- **[`final_validation.sql`](Validation/final_validation.sql)** - Validation complète après migration
- **[`check_database_status.sql`](Validation/check_database_status.sql)** - Vérification de l'état à tout moment

### Utilisation
```bash
psql -h localhost -U postgres -d cinephoria_db -f DatabaseManagement/Validation/check_database_status.sql
```

## 🎯 Stratégies de Migration

### 1. Migrations SQL Manuelles (Recommandé pour le déploiement)
- **Avantages** : Contrôle total, scripts réutilisables
- **Utilisation** : Déploiement en production, environnements multiples
- **Script principal** : [`simple_migration.sql`](Scripts/simple_migration.sql)

### 2. Migrations EF Core (Recommandé pour le développement)
- **Avantages** : Intégration avec le code, automatisation
- **Utilisation** : Développement local, petites modifications
- **Commandes** : `dotnet ef migrations add`, `dotnet ef database update`

## 🔄 Workflow Recommandé

### Pour un Nouvel Environnement
1. **Déploiement initial** : Utiliser les scripts de déploiement automatique
2. **Vérification** : Exécuter les scripts de validation
3. **Documentation** : Consulter les guides pour comprendre la structure

### Pour le Développement
1. **Modifications mineures** : Utiliser EF Core migrations
2. **Modifications majeures** : Créer des scripts SQL réutilisables
3. **Tests** : Toujours valider avec les scripts de vérification

## 📊 État Actuel

### Base de Données
- **Nom** : `cinephoria_db`
- **Tables** : 19 tables
- **Version** : 1.1.0
- **Statut** : ✅ Opérationnelle

### Dernière Migration
- **Date** : 18/10/2024
- **Objectif** : Résolution des incohérences modèles C# / schéma BD
- **Script utilisé** : [`simple_migration.sql`](Scripts/simple_migration.sql)

## 🛡️ Sécurité et Sauvegarde

### Avant Toute Migration
```bash
# Sauvegarde
pg_dump -h localhost -U postgres -d cinephoria_db -f backup_$(date +%Y%m%d).sql

# Restauration
psql -h localhost -U postgres -d cinephoria_db -f backup_YYYYMMDD.sql
```

### Variables d'Environnement Recommandées
```bash
export PG_HOST=localhost
export PG_USER=postgres
export PG_DB=cinephoria_db
```

## 📞 Support

### Problèmes Courants
1. **Connexion refusée** - Vérifier que PostgreSQL est démarré
2. **Permissions insuffisantes** - Utiliser un utilisateur avec droits adéquats
3. **Tables déjà existantes** - Les scripts utilisent `IF NOT EXISTS`

### Vérifications Rapides
```sql
-- Vérifier la connexion
SELECT version();

-- Vérifier les tables
\dt

-- Vérifier une table spécifique
SELECT * FROM information_schema.columns WHERE table_name = 'Movies';
```

---

**Dernière Mise à Jour** : 18/10/2024  
**Responsable** : Équipe de développement Cinéphoria  
**Contact** : [patricesimo.dev@gmail.com](mailto:patricesimo.dev@gmail.com)

Pour toute question, consulter la documentation dans le dossier [`Documentation/`](Documentation/).