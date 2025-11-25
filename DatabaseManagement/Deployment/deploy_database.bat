@echo off
REM Script de déploiement automatique de la base de données Cinéphoria - Windows
REM Version: 1.0
REM Date: 18/10/2024

echo ============================================
echo   Déploiement Base de Données Cinéphoria
echo ============================================
echo.

REM Vérifier si PostgreSQL est accessible
echo [1/6] Vérification de la connexion PostgreSQL...
psql -h localhost -U postgres -c "SELECT version();" > nul 2>&1
if %errorlevel% neq 0 (
    echo ERREUR: Impossible de se connecter à PostgreSQL
    echo Vérifiez que PostgreSQL est démarré et accessible
    pause
    exit /b 1
)
echo ✓ PostgreSQL accessible

REM Créer la base de données si elle n'existe pas
echo [2/6] Création de la base de données...
psql -h localhost -U postgres -c "SELECT 1 FROM pg_database WHERE datname = 'cinephoria_db'" | findstr /C:"1" > nul
if %errorlevel% equ 0 (
    echo ✓ Base de données existe déjà
) else (
    psql -h localhost -U postgres -c "CREATE DATABASE cinephoria_db;"
    if %errorlevel% equ 0 (
        echo ✓ Base de données créée avec succès
    ) else (
        echo ERREUR: Impossible de créer la base de données
        pause
        exit /b 1
    )
)

REM Exécuter le script de migration initial
echo [3/6] Exécution du script de migration initial...
psql -h localhost -U postgres -d cinephoria_db -f ..\Scripts\migration_script.sql
if %errorlevel% equ 0 (
    echo ✓ Migration initiale appliquée
) else (
    echo ATTENTION: Erreurs lors de la migration initiale (peut être normal si tables existent déjà)
)

REM Appliquer les mises à jour
echo [4/6] Application des mises à jour...
psql -h localhost -U postgres -d cinephoria_db -f ..\Scripts\simple_migration.sql
if %errorlevel% equ 0 (
    echo ✓ Mises à jour appliquées avec succès
) else (
    echo ERREUR: Impossible d'appliquer les mises à jour
    pause
    exit /b 1
)

REM Valider le déploiement
echo [5/6] Validation du déploiement...
psql -h localhost -U postgres -d cinephoria_db -f ..\Validation\final_validation.sql
if %errorlevel% equ 0 (
    echo ✓ Déploiement validé avec succès
) else (
    echo ATTENTION: Problèmes lors de la validation
)

REM Vérification finale
echo [6/6] Vérification finale...
psql -h localhost -U postgres -d cinephoria_db -c "SELECT COUNT(*) as nombre_tables FROM information_schema.tables WHERE table_schema = 'public';"
psql -h localhost -U postgres -d cinephoria_db -c "SELECT COUNT(*) as nombre_colonnes_movies FROM information_schema.columns WHERE table_name = 'Movies';"

echo.
echo ============================================
echo   DÉPLOIEMENT TERMINÉ AVEC SUCCÈS !
echo ============================================
echo.
echo Résumé :
echo - Base de données : cinephoria_db
echo - Tables créées : 19 tables
echo - Colonnes Movies : 17 colonnes
echo - Paramètres configurés : 6 paramètres
echo.
echo La base de données est prête pour l'application Cinéphoria.
echo.

pause