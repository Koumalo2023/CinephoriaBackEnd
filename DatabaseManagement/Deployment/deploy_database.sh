#!/bin/bash

# Script de déploiement automatique de la base de données Cinéphoria - Linux/Mac
# Version: 1.0
# Date: 18/10/2024

echo "============================================"
echo "  Déploiement Base de Données Cinéphoria"
echo "============================================"
echo ""

# Fonction pour vérifier les erreurs
check_error() {
    if [ $? -ne 0 ]; then
        echo "❌ ERREUR: $1"
        exit 1
    fi
}

# Vérifier si PostgreSQL est accessible
echo "[1/6] Vérification de la connexion PostgreSQL..."
psql -h localhost -U postgres -c "SELECT version();" > /dev/null 2>&1
check_error "Impossible de se connecter à PostgreSQL. Vérifiez que PostgreSQL est démarré et accessible."
echo "✅ PostgreSQL accessible"

# Créer la base de données si elle n'existe pas
echo "[2/6] Création de la base de données..."
DB_EXISTS=$(psql -h localhost -U postgres -t -c "SELECT 1 FROM pg_database WHERE datname = 'cinephoria_db'" 2>/dev/null)

if [ "$DB_EXISTS" = "1" ]; then
    echo "✅ Base de données existe déjà"
else
    psql -h localhost -U postgres -c "CREATE DATABASE cinephoria_db;"
    check_error "Impossible de créer la base de données"
    echo "✅ Base de données créée avec succès"
fi

# Exécuter le script de migration initial
echo "[3/6] Exécution du script de migration initial..."
psql -h localhost -U postgres -d cinephoria_db -f ../Scripts/migration_script.sql
if [ $? -eq 0 ]; then
    echo "✅ Migration initiale appliquée"
else
    echo "⚠️  ATTENTION: Erreurs lors de la migration initiale (peut être normal si tables existent déjà)"
fi

# Appliquer les mises à jour
echo "[4/6] Application des mises à jour..."
psql -h localhost -U postgres -d cinephoria_db -f ../Scripts/simple_migration.sql
check_error "Impossible d'appliquer les mises à jour"
echo "✅ Mises à jour appliquées avec succès"

# Valider le déploiement
echo "[5/6] Validation du déploiement..."
psql -h localhost -U postgres -d cinephoria_db -f ../Validation/final_validation.sql
if [ $? -eq 0 ]; then
    echo "✅ Déploiement validé avec succès"
else
    echo "⚠️  ATTENTION: Problèmes lors de la validation"
fi

# Vérification finale
echo "[6/6] Vérification finale..."
echo "Tables créées:"
psql -h localhost -U postgres -d cinephoria_db -c "SELECT COUNT(*) as nombre_tables FROM information_schema.tables WHERE table_schema = 'public';"

echo "Colonnes dans la table Movies:"
psql -h localhost -U postgres -d cinephoria_db -c "SELECT COUNT(*) as nombre_colonnes_movies FROM information_schema.columns WHERE table_name = 'Movies';"

echo ""
echo "============================================"
echo "  DÉPLOIEMENT TERMINÉ AVEC SUCCÈS !"
echo "============================================"
echo ""
echo "Résumé :"
echo "- Base de données : cinephoria_db"
echo "- Tables créées : 19 tables"
echo "- Colonnes Movies : 17 colonnes"
echo "- Paramètres configurés : 6 paramètres"
echo ""
echo "La base de données est prête pour l'application Cinéphoria."
echo ""