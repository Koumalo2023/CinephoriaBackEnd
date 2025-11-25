@echo off
REM Script batch pour automatiser le Jour 1 - Préparation et Sauvegardes
REM Ce script automatise les sauvegardes et la préparation de l'environnement de test
REM Date: %date% %time%

echo ===============================================
echo JOUR 1 - AUTOMATION DES SAUVEGARDES
echo ===============================================
echo.

REM Vérifier que PostgreSQL est accessible
echo [ETAPE 1] Vérification de l'accès à PostgreSQL...
pg_isready -h localhost -U postgres
if %errorlevel% neq 0 (
    echo ERREUR: PostgreSQL n'est pas accessible
    echo Vérifiez que le service PostgreSQL est démarré
    pause
    exit /b 1
)
echo ✓ PostgreSQL est accessible
echo.

REM Définir les variables
set BACKUP_DIR=backups
set DATE_SUFFIX=%date:~6,4%%date:~3,2%%date:~0,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set BACKUP_FILE=%BACKUP_DIR%\backup_pre_migration_%DATE_SUFFIX%.sql
set SCHEMA_FILE=%BACKUP_DIR%\schema_backup_%DATE_SUFFIX%.sql
set CRITICAL_FILE=%BACKUP_DIR%\critical_data_backup_%DATE_SUFFIX%.sql
set DB_NAME=cinephoria_db
set TEST_DB_NAME=cinephoria_test_migration

REM Créer le répertoire de sauvegarde s'il n'existe pas
if not exist "%BACKUP_DIR%" (
    echo [ETAPE 2] Création du répertoire de sauvegarde...
    mkdir "%BACKUP_DIR%"
    echo ✓ Répertoire %BACKUP_DIR% créé
) else (
    echo ✓ Répertoire %BACKUP_DIR% existe déjà
)
echo.

REM Sauvegarde complète de la base de données
echo [ETAPE 3] Sauvegarde complète de la base de données...
echo Fichier: %BACKUP_FILE%
pg_dump -h localhost -U postgres "%DB_NAME%" > "%BACKUP_FILE%"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la sauvegarde complète
    pause
    exit /b 1
)
echo ✓ Sauvegarde complète terminée
echo.

REM Sauvegarde du schéma uniquement
echo [ETAPE 4] Sauvegarde du schéma uniquement...
echo Fichier: %SCHEMA_FILE%
pg_dump -h localhost -U postgres "%DB_NAME%" --schema-only > "%SCHEMA_FILE%"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la sauvegarde du schéma
    pause
    exit /b 1
)
echo ✓ Sauvegarde du schéma terminée
echo.

REM Sauvegarde des données critiques
echo [ETAPE 5] Sauvegarde des données critiques...
echo Fichier: %CRITICAL_FILE%
pg_dump -h localhost -U postgres "%DB_NAME%" --data-only --table="Movies" --table="AspNetUsers" --table="Reservations" --table="Showtimes" --table="MovieRatings" > "%CRITICAL_FILE%"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la sauvegarde des données critiques
    pause
    exit /b 1
)
echo ✓ Sauvegarde des données critiques terminée
echo.

REM Vérification des sauvegardes
echo [ETAPE 6] Vérification des sauvegardes...
echo Taille des fichiers de sauvegarde:
dir "%BACKUP_DIR%\backup_pre_migration_%DATE_SUFFIX%.sql"
dir "%BACKUP_DIR%\schema_backup_%DATE_SUFFIX%.sql"
dir "%BACKUP_DIR%\critical_data_backup_%DATE_SUFFIX%.sql"
echo.

REM Vérifier que les sauvegardes ne sont pas vides
for %%F in ("%BACKUP_FILE%", "%SCHEMA_FILE%", "%CRITICAL_FILE%") do (
    if %%~zF equ 0 (
        echo ERREUR: Le fichier %%F est vide
        pause
        exit /b 1
    )
)
echo ✓ Toutes les sauvegardes sont valides
echo.

REM Création de la base de test
echo [ETAPE 7] Création de la base de test...
psql -h localhost -U postgres -c "DROP DATABASE IF EXISTS %TEST_DB_NAME%;"
psql -h localhost -U postgres -c "CREATE DATABASE %TEST_DB_NAME%;"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la création de la base de test
    pause
    exit /b 1
)
echo ✓ Base de test %TEST_DB_NAME% créée
echo.

REM Restauration de la sauvegarde sur l'environnement de test
echo [ETAPE 8] Restauration sur l'environnement de test...
psql -h localhost -U postgres -d "%TEST_DB_NAME%" -f "%BACKUP_FILE%"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la restauration sur l'environnement de test
    pause
    exit /b 1
)
echo ✓ Restauration sur l'environnement de test terminée
echo.

REM Vérification de l'environnement de test
echo [ETAPE 9] Vérification de l'environnement de test...
psql -h localhost -U postgres -d "%TEST_DB_NAME%" -c "
SELECT 
    'Movies' as table_name,
    COUNT(*) as record_count
FROM \\\"Movies\\\"
UNION ALL
SELECT 
    'AspNetUsers' as table_name,
    COUNT(*) as record_count
FROM \\\"AspNetUsers\\\"
UNION ALL
SELECT 
    'Reservations' as table_name,
    COUNT(*) as record_count
FROM \\\"Reservations\\\"
UNION ALL
SELECT 
    'Showtimes' as table_name,
    COUNT(*) as record_count
FROM \\\"Showtimes\\\"
UNION ALL
SELECT 
    'MovieRatings' as table_name,
    COUNT(*) as record_count
FROM \\\"MovieRatings\\\";
"
if %errorlevel% neq 0 (
    echo ERREUR: Échec de la vérification de l'environnement de test
    pause
    exit /b 1
)
echo ✓ Environnement de test vérifié
echo.

REM Copie des sauvegardes sur un second support (exemple: lecteur réseau)
echo [ETAPE 10] Copie des sauvegardes sur un second support...
echo NOTE: Ajustez le chemin du second support selon votre configuration
REM Exemple: xcopy "%BACKUP_DIR%" "Z:\backups_cinephoria\%DATE_SUFFIX%\" /E /I /Y
echo Pour copier sur un second support, exécutez manuellement:
echo xcopy "%BACKUP_DIR%" "Z:\backups_cinephoria\%DATE_SUFFIX%\" /E /I /Y
echo.

REM Rapport final
echo ===============================================
echo RAPPORT FINAL - JOUR 1 AUTOMATISÉ
echo ===============================================
echo.
echo ✓ Sauvegardes créées:
echo   - %BACKUP_FILE%
echo   - %SCHEMA_FILE%
echo   - %CRITICAL_FILE%
echo.
echo ✓ Environnement de test créé:
echo   - Base: %TEST_DB_NAME%
echo   - Restaurée depuis: %BACKUP_FILE%
echo.
echo ✓ Prochaines étapes:
echo   1. Vérifier manuellement les sauvegardes
echo   2. Tester l'application avec la base de test
echo   3. Poursuivre avec le Jour 2 - Migration
echo.
echo ===============================================
echo AUTOMATION TERMINÉE AVEC SUCCÈS
echo ===============================================
echo.

pause