# Guide Administrateur - Gestion des Séances et Réservations

## 📋 Rôle Administrateur
L'administrateur a un accès complet à toutes les fonctionnalités du système, incluant la gestion des séances, des réservations, des statistiques et de la configuration système.

---

## 🎬 Workflow 1: Gestion Complète des Séances

### Objectif : Programmer et gérer l'ensemble des séances de cinéma

#### Fonctionnalités Utilisées :
- **Création de séances** (ancienne)
- **Modification de séances** (ancienne) 
- **Suppression de séances** (ancienne)
- **Statuts automatiques** (nouvelle)
- **Statistiques des séances** (nouvelle)

#### Endpoints Clés :
```http
POST /api/showtime/create
PUT /api/showtime/update  
DELETE /api/showtime/delete/{showtimeId}
GET /api/showtime/status
GET /api/showtime/stats
POST /api/showtime/update-status
PUT /api/showtime/{id}/status
```

#### Workflow Détaillé :

**1. Programmation Hebdomadaire**
```bash
# Créer une nouvelle séance
POST /api/showtime/create
Body: {
  "movieId": 123,
  "theaterId": 1,
  "startTime": "2025-11-15T20:00:00",
  "endTime": "2025-11-15T22:30:00",
  "price": 12.50
}

# Vérifier toutes les séances programmées
GET /api/showtime/status
```

**2. Surveillance Quotidienne**
```bash
# Obtenir les statistiques globales
GET /api/showtime/stats
# Réponse : {
#   "totalShowtimes": 45,
#   "upcomingCount": 12,
#   "ongoingCount": 3,
#   "completedCount": 30
# }

# Forcer la mise à jour des statuts si nécessaire
POST /api/showtime/update-status
```

**3. Gestion des Modifications**
```bash
# Modifier une séance existante
PUT /api/showtime/update
Body: {
  "showtimeId": 456,
  "startTime": "2025-11-15T20:30:00",
  "price": 11.50
}

# Annuler une séance
DELETE /api/showtime/delete/456
```

**4. Intervention sur les Statuts**
```bash
# Forcer manuellement un statut
PUT /api/showtime/789/status
Body: {
  "status": "Cancelled"
}
```

---

## 📊 Workflow 2: Analyse et Reporting

### Objectif : Analyser les performances et l'occupation

#### Fonctionnalités Utilisées :
- **Films avec séances** (nouvelle)
- **Films récents** (nouvelle)
- **Statistiques détaillées** (nouvelle)

#### Endpoints Clés :
```http
GET /api/showtime/movies-with-showtimes
GET /api/showtime/recent-movies
GET /api/showtime/stats
```

#### Workflow Détaillé :

**1. Analyse des Programmes**
```bash
# Voir tous les films programmés
GET /api/showtime/movies-with-showtimes
# Réponse : Liste des films avec nombre de séances et disponibilités

# Films ajoutés récemment
GET /api/showtime/recent-movies
# Réponse : Films ajoutés le dernier mercredi avec leurs séances
```

**2. Reporting Quotidien**
```bash
# Statistiques complètes
GET /api/showtime/stats
# Utiliser ces données pour :
# - Ajuster la programmation
# - Identifier les films populaires
# - Optimiser l'occupation des salles
```

---

## 🔔 Workflow 3: Gestion des Rappels et Notifications

### Objectif : Superviser le système de rappels automatiques

#### Fonctionnalités Utilisées :
- **Rappels de paiement** (nouvelle)
- **Rappels de séance** (nouvelle)
- **Avertissements d'expiration** (nouvelle)
- **Statistiques des rappels** (nouvelle)

#### Endpoints Clés :
```http
POST /api/reservation/send-payment-reminders
POST /api/reservation/send-showtime-reminders  
POST /api/reservation/send-expiration-warnings
GET /api/reservation/reminder-stats
GET /api/reservation/pending-payment-reminders
GET /api/reservation/pending-showtime-reminders
```

#### Workflow Détaillé :

**1. Surveillance Matinale**
```bash
# Vérifier l'état des rappels
GET /api/reservation/reminder-stats
# Réponse : {
#   "pendingPaymentReminders": 5,
#   "pendingShowtimeReminders": 12,
#   "expiringReservations": 3
# }
```

**2. Intervention Manuelle**
```bash
# Envoyer manuellement des rappels si nécessaire
POST /api/reservation/send-payment-reminders
POST /api/reservation/send-showtime-reminders
POST /api/reservation/send-expiration-warnings
```

**3. Suivi des Rappels en Attente**
```bash
# Voir les détails des rappels
GET /api/reservation/pending-payment-reminders
GET /api/reservation/pending-showtime-reminders
```

---

## ⚙️ Workflow 4: Configuration et Maintenance

### Objectif : Configurer et maintenir le système

#### Fonctionnalités Utilisées :
- **Test de configuration email** (nouvelle)
- **Statistiques email** (nouvelle)
- **Gestion des réservations** (ancienne)

#### Endpoints Clés :
```http
GET /api/reservation/email-stats
POST /api/reservation/test-email-config
GET /api/reservation/showtime/{showtimeId}
```

#### Workflow Détaillé :

**1. Maintenance du Système Email**
```bash
# Vérifier les statistiques d'email
GET /api/reservation/email-stats

# Tester la configuration
POST /api/reservation/test-email-config
```

**2. Supervision des Réservations**
```bash
# Voir toutes les réservations d'une séance
GET /api/reservation/showtime/789
```

---

## 🎯 Tableau de Bord Administrateur

### Métriques Clés à Surveiller :

| Métrique | Endpoint | Fréquence | Seuil d'Alerte |
|----------|----------|-----------|----------------|
| Séances à venir | `/api/showtime/upcoming` | Quotidienne | < 5 séances |
| Rappels en attente | `/api/reservation/reminder-stats` | Quotidienne | > 10 rappels |
| Taux d'occupation | `/api/showtime/stats` | Hebdomadaire | < 60% |
| Échecs d'email | `/api/reservation/email-stats` | Quotidienne | > 5% d'échecs |

### Actions Automatiques Configurées :

1. **Mise à jour des statuts** : Toutes les 5 minutes
2. **Expiration des réservations** : Toutes les 15 minutes  
3. **Rappels de paiement** : Toutes les heures
4. **Rappels de séance** : 24h et 1h avant la séance

---

## 🔧 Configuration Requise

### Services de Fond
```csharp
// Dans Program.cs
builder.Services.AddHostedService<ShowtimeStatusBackgroundService>();
builder.Services.AddHostedService<ReservationExpirationBackgroundService>(); 
builder.Services.AddHostedService<ReservationReminderBackgroundService>();
```

### Paramètres Email
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.cinephoria.com",
    "Port": 587,
    "FromEmail": "admin@cinephoria.com"
  }
}
```

---

## 📞 Support et Dépannage

### Problèmes Courants :

1. **Statuts non mis à jour** → Vérifier `ShowtimeStatusBackgroundService`
2. **Rappels non envoyés** → Tester avec `/api/reservation/test-email-config`
3. **Expirations non traitées** → Vérifier `ReservationExpirationBackgroundService`

### Logs de Surveillance :
- `logs/CinephoriaLog{date}.txt`
- Surveiller les erreurs liées aux services de fond

Ce guide permet à l'administrateur de gérer efficacement l'ensemble du système de séances et réservations, en combinant les fonctionnalités anciennes et nouvelles pour une gestion optimale.