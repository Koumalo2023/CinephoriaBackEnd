# Étape 1 : Image de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Définir le répertoire de travail
WORKDIR /src

# Copier uniquement le fichier solution et restaurer les dépendances
COPY *.sln ./
COPY CinephoriaServer.csproj ./

# Restaurer les dépendances
RUN dotnet restore "CinephoriaServer.csproj"

# Copier le reste du code source
COPY . .

# Compiler et publier l'application
RUN dotnet publish "CinephoriaServer.csproj" -c Release -o /app

# Étape 2 : Image runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Définir le répertoire de travail
WORKDIR /app

# Copier les artefacts publiés depuis l'étape de build
COPY --from=build /app .

# Exposer le port de l'application
EXPOSE 80

# Définir la commande pour démarrer l'application
ENTRYPOINT ["dotnet", "CinephoriaServer.dll"]