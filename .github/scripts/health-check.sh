#!/bin/bash
# Script de vérification de santé pour Cinephoria Backend

set -e

ENVIRONMENT=$1
PORT=$([ "$ENVIRONMENT" = "staging" ] && echo "5001" || echo "5000")
MAX_RETRIES=10
RETRY_INTERVAL=10

echo "🔍 Vérification de santé pour l'environnement $ENVIRONMENT sur le port $PORT..."

# Vérification des containers Docker
echo "📦 Vérification des containers Docker..."
if ! docker ps | grep -q "cinephoria-backend-$ENVIRONMENT"; then
    echo "❌ Container backend non trouvé"
    exit 1
fi

if ! docker ps | grep -q "cinephoria-postgres-$ENVIRONMENT"; then
    echo "❌ Container PostgreSQL non trouvé"
    exit 1
fi

echo "✅ Containers Docker en cours d'exécution"

# Vérification de l'endpoint health avec retries
for i in $(seq 1 $MAX_RETRIES); do
    echo "Tentative $i/$MAX_RETRIES de connexion à l'API..."
    
    # Vérifier l'endpoint health
    RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$PORT/health/ready" || true)
    
    if [ "$RESPONSE" = "200" ]; then
        echo "✅ Vérification de santé réussie ! Le service est prêt."
        
        # Vérification détaillée des bases de données
        echo "📊 Vérification détaillée des services..."
        HEALTH_DETAILS=$(curl -s "http://localhost:$PORT/health/ready")
        echo "Détails de santé : $HEALTH_DETAILS"
        
        # Vérification de l'API principale
        API_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$PORT/api/movies" || true)
        if [ "$API_RESPONSE" = "200" ] || [ "$API_RESPONSE" = "401" ] || [ "$API_RESPONSE" = "403" ]; then
            echo "✅ API principale accessible (HTTP $API_RESPONSE)"
        else
            echo "⚠️  API principale non accessible (HTTP $API_RESPONSE)"
        fi
        
        exit 0
    fi
    
    echo "⏳ Service pas encore prêt (HTTP $RESPONSE), nouvelle tentative dans $RETRY_INTERVAL secondes..."
    sleep $RETRY_INTERVAL
done

echo "❌ Échec de la vérification de santé après $MAX_RETRIES tentatives"
echo "📋 Dernier état des containers :"
docker ps

echo "📜 Logs du container backend :"
docker logs "cinephoria-backend-$ENVIRONMENT" --tail 20

exit 1