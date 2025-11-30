#!/bin/bash
# Script de déploiement backend pour Cinephoria

set -e  # Exit on error

ENVIRONMENT=$1
COMPOSE_FILE="docker-compose.$ENVIRONMENT.yml"

echo "🚀 Déploiement du backend Cinephoria pour l'environnement $ENVIRONMENT..."

# Vérifier que le fichier docker-compose existe
if [ ! -f "$COMPOSE_FILE" ]; then
    echo "❌ Fichier $COMPOSE_FILE non trouvé"
    exit 1
fi

echo "📋 Configuration détectée :"
echo "   - Environnement : $ENVIRONMENT"
echo "   - Fichier Docker Compose : $COMPOSE_FILE"

# Arrêter les containers existants
echo "⏹️  Arrêt des containers existants..."
docker-compose -f "$COMPOSE_FILE" down || true

# Nettoyer les anciennes images (optionnel)
echo "🧹 Nettoyage des anciennes images..."
docker image prune -f || true

# Puller la nouvelle image
echo "📥 Téléchargement de la nouvelle image Docker..."
docker-compose -f "$COMPOSE_FILE" pull

# Démarrer les containers
echo "🔄 Démarrage des containers..."
docker-compose -f "$COMPOSE_FILE" up -d

# Attendre que les services soient prêts
echo "⏳ Attente du démarrage des services..."
sleep 30

# Vérifier la santé des services
echo "🏥 Vérification de la santé des services..."
if [ -f "./health-check.sh" ]; then
    ./health-check.sh "$ENVIRONMENT"
else
    echo "⚠️  Script health-check.sh non trouvé, vérification manuelle..."
    
    # Vérification basique des containers
    if docker ps | grep -q "cinephoria-backend-$ENVIRONMENT"; then
        echo "✅ Container backend en cours d'exécution"
    else
        echo "❌ Container backend non démarré"
        exit 1
    fi
    
    if docker ps | grep -q "cinephoria-postgres-$ENVIRONMENT"; then
        echo "✅ Container PostgreSQL en cours d'exécution"
    else
        echo "❌ Container PostgreSQL non démarré"
        exit 1
    fi
fi

echo "✅ Déploiement $ENVIRONMENT terminé avec succès!"
echo "📊 Application accessible sur :"
if [ "$ENVIRONMENT" = "staging" ]; then
    echo "   - Backend: http://localhost:5001"
    echo "   - Health: http://localhost:5001/health/ready"
else
    echo "   - Backend: http://localhost:5000"
    echo "   - Health: http://localhost:5000/health/ready"
fi