# Étape 1 : Image de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définir le répertoire de travail
WORKDIR /src

# Nettoyer le cache NuGet et les fichiers temporaires
RUN rm -rf obj bin

# Copier les fichiers projet et restaurer les dépendances
COPY ["CinephoriaServer.API.csproj", "./"]
RUN dotnet restore "CinephoriaServer.API.csproj"

# Copier le reste du code source
COPY . .

# Compiler et publier l'application avec optimisations
RUN dotnet publish "CinephoriaServer.API.csproj" -c Release -o /app/publish

# Étape 2 : Image runtime optimisée
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Définir les variables d'environnement
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Définir le répertoire de travail
WORKDIR /app

# Créer un utilisateur non-root pour la sécurité
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copier les artefacts publiés depuis l'étape de build
COPY --from=build /app/publish .

# Créer le dossier des certificats et copier les certificats (optionnel)
# RUN mkdir -p /app/certificates
# COPY certificates/ /app/certificates/

# Changer les permissions pour l'utilisateur non-root
RUN chown -R appuser:appuser /app
USER appuser

# Exposer le port de l'application
EXPOSE 80

# Définir la commande pour démarrer l'application avec health checks
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/health/live || exit 1

ENTRYPOINT ["dotnet", "CinephoriaServer.API.dll"]