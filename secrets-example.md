# Secrets GitHub Actions - Cinephoria

Ce document liste les secrets nécessaires pour configurer les workflows GitHub Actions des projets **Frontend** et **Backend** pour les environnements **staging** et **production**.

Les secrets doivent être définis dans les paramètres du dépôt GitHub (Settings > Secrets and variables > Actions) pour chaque environnement (si vous utilisez les environnements GitHub) ou en tant que secrets de dépôt globaux.

---

## 1. Secrets communs (utilisés par les deux projets)

| Nom du secret | Description | Exemple de valeur (staging) | Exemple de valeur (production) |
|---------------|-------------|-----------------------------|--------------------------------|
| `AWS_ROLE_ARN` | ARN du rôle IAM à assumer pour les déploiements AWS. | `arn:aws:iam::123456789012:role/github-actions-cinephoria-staging` | `arn:aws:iam::123456789012:role/github-actions-cinephoria-production` |
| `AWS_REGION` | Région AWS (peut être défini comme variable d'environnement). | `eu-west-3` | `eu-west-3` |

---

## 2. Secrets Frontend (Cinephoria‑web)

Ces secrets sont utilisés par le workflow `frontend-deploy.yml`.

| Nom du secret | Description | Exemple de valeur (staging) | Exemple de valeur (production) |
|---------------|-------------|-----------------------------|--------------------------------|
| `S3_BUCKET` | Nom du bucket S3 où les fichiers statiques sont déployés. | `cinephoria-staging-app` | `cinephoria-production-app` |
| `CLOUDFRONT_ID` | ID de la distribution CloudFront à invalider après déploiement. | `E123456789ABCD` | `E987654321ABCD` |

**Remarque** : Le frontend utilise également `AWS_ROLE_ARN` pour l'authentification OIDC.

---

## 3. Secrets Backend (CinephoriaBackEnd)

Ces secrets sont utilisés par les workflows `backend-deploy.yml` et `deploy-backend.yml`.

### 3.1. Secrets d'infrastructure AWS

| Nom du secret | Description | Exemple de valeur (staging) | Exemple de valeur (production) |
|---------------|-------------|-----------------------------|--------------------------------|
| `EC2_INSTANCE_ID` | ID de l'instance EC2 sur laquelle déployer le backend. | `i-0a1b2c3d4e5f6g7h8` | `i-1b2c3d4e5f6g7h8i9` |

### 3.2. Secrets de base de données

| Nom du secret | Description | Exemple de valeur (staging) | Exemple de valeur (production) |
|---------------|-------------|-----------------------------|--------------------------------|
| `POSTGRES_PASSWORD` | Mot de passe de l'utilisateur PostgreSQL. | `StagingPgPassword123!` | `ProductionPgPassword456!` |
| `MONGODB_URI` | URI de connexion à MongoDB (sans authentification, car déployé dans le même réseau Docker). | `mongodb://mongodb:27017` | `mongodb://mongodb:27017` |

### 3.3. Secrets d'application

| Nom du secret | Description | Exemple de valeur (staging) | Exemple de valeur (production) |
|---------------|-------------|-----------------------------|--------------------------------|
| `JWT_SECRET` | Clé secrète pour signer les tokens JWT (minimum 32 caractères). | `StagingJwtSecretKey1234567890abcdef` | `ProductionJwtSecretKey1234567890abcdef` |
| `TMDb__ApiKey` | Clé API de The Movie Database (TMDB). (Optionnel) | `tmdb_staging_api_key_example` | `tmdb_production_api_key_example` |
| `SMTP_PASSWORD` | Mot de passe SMTP pour l'envoi d'emails. (Optionnel) | `staging-smtp-password` | `production-smtp-password` |

### 3.4. Secrets Docker (obsolètes après migration vers GHCR)

Les secrets suivants ne sont plus nécessaires si vous utilisez GitHub Container Registry (GHCR) comme recommandé. Si vous souhaitez toujours utiliser Docker Hub, conservez‑les :

| Nom du secret | Description | Exemple de valeur |
|---------------|-------------|-------------------|
| `DOCKER_USERNAME` | Nom d'utilisateur Docker Hub. | `votreusername` |
| `DOCKER_PASSWORD` | Token d'accès Docker Hub. | `votre-token` |

---

## 4. Variables d'environnement par défaut

Les workflows définissent également des variables d'environnement dans leurs fichiers YAML. Vous pouvez les surcharger via les secrets si nécessaire.

**Frontend** :
- `NODE_VERSION` : `'18'`
- `API_URL` : Défini dynamiquement (`https://staging-api.cinephoria.eu/api` ou `https://api.cinephoria.eu/api`)

**Backend** :
- `REGISTRY` : `ghcr.io`
- `IMAGE_NAME` : `${{ github.repository }}/cinephoria-backend`
- `DOCKER_TAG` : `latest`

---

## 5. Configuration des environnements GitHub

Pour une gestion fine, créez deux environnements dans GitHub Actions : **staging** et **production**, et affectez les secrets respectifs à chaque environnement.

### Étapes :
1. Allez dans **Settings > Environments**.
2. Créez les environnements `staging` et `production`.
3. Pour chaque environnement, ajoutez les secrets listés ci‑dessus avec les valeurs appropriées.

### Exemple de structure des secrets dans GitHub :

**Environment `staging`** :
- `AWS_ROLE_ARN` = `arn:aws:iam::123456789012:role/github-actions-cinephoria-staging`
- `S3_BUCKET` = `cinephoria-staging-app`
- `EC2_INSTANCE_ID` = `i-0a1b2c3d4e5f6g7h8`
- `POSTGRES_PASSWORD` = `StagingPgPassword123!`
- etc.

**Environment `production`** :
- `AWS_ROLE_ARN` = `arn:aws:iam::123456789012:role/github-actions-cinephoria-production`
- `S3_BUCKET` = `cinephoria-production-app`
- `EC2_INSTANCE_ID` = `i-1b2c3d4e5f6g7h8i9`
- `POSTGRES_PASSWORD` = `ProductionPgPassword456!`
- etc.

---

## 6. Vérification après ajout des secrets

Une fois les secrets ajoutés, déclenchez manuellement les workflows pour vérifier qu'ils s'exécutent sans erreur d'authentification.

- **Frontend** : Workflow `Frontend Deploy`
- **Backend** : Workflow `Backend Deploy` ou `Deploy Backend to AWS`

Si des erreurs persistent, consultez les logs de l'exécution pour identifier les secrets manquants ou incorrects.

---

*Note : Les valeurs d'exemple fournies ici sont à remplacer par vos propres valeurs sécurisées. Ne commitez jamais de secrets dans le dépôt.*