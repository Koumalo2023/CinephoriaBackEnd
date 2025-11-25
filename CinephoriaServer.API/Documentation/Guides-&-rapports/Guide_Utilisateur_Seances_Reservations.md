# Guide Utilisateur - Réservation de Séances de Cinéma

## 📋 Rôle Utilisateur
L'utilisateur final peut rechercher des séances, réserver des places, gérer ses réservations et recevoir des notifications automatiques.

---

## 🎬 Workflow 1: Recherche et Réservation

### Objectif : Trouver et réserver des places pour une séance

#### Fonctionnalités Utilisées :
- **Recherche de séances** (ancienne)
- **Consultation des sièges** (ancienne)
- **Création de réservation** (ancienne)
- **Rappels automatiques** (nouvelle)

#### Endpoints Clés :
```http
GET /api/reservation/movie/{movieId}/sessions
GET /api/reservation/showtime/{showtimeId}/seats
POST /api/reservation/create
```

#### Workflow Détaillé :

**1. Recherche de Séances**
```bash
# Trouver toutes les séances d'un film
GET /api/reservation/movie/123/sessions
# Réponse : Liste des séances disponibles avec horaires et salles
```

**2. Choix des Places**
```bash
# Voir les sièges disponibles pour une séance
GET /api/reservation/showtime/789/seats
# Réponse : Plan de salle avec sièges disponibles/occupés
```

**3. Confirmation de Réservation**
```bash
# Créer la réservation
POST /api/reservation/create
Body: {
  "showtimeId": 789,
  "appUserId": "user-123",
  "seatIds": [101, 102, 103],
  "totalPrice": 37.50
}
```

**4. Réception de la Confirmation**
- **Email automatique** avec les détails de la réservation
- **QR Code** pour l'entrée en salle
- **Rappels automatiques** programmés

---

## 📋 Workflow 2: Gestion des Réservations Personnelles

### Objectif : Consulter et gérer ses propres réservations

#### Fonctionnalités Utilisées :
- **Consultation des réservations** (ancienne)
- **Annulation de réservation** (ancienne)
- **Statuts de réservation** (nouvelle)
- **Expiration automatique** (nouvelle)

#### Endpoints Clés :
```http
GET /api/reservation/user/{appUserId}
DELETE /api/reservation/cancel/{reservationId}
```

#### Workflow Détaillé :

**1. Consultation des Réservations**
```bash
# Voir toutes mes réservations
GET /api/reservation/user/user-123
# Réponse : Liste avec statuts, dates, et détails
```

**2. Gestion des Réservations**
- **Réservation active** : Statut "Confirmed", QR Code disponible
- **En attente de paiement** : Statut "PendingPayment", délai de 15 minutes
- **Expirée** : Statut "Expired", réservation automatiquement annulée
- **Annulée** : Statut "Cancelled", places libérées

**3. Annulation de Réservation**
```bash
# Annuler une réservation (avant expiration)
DELETE /api/reservation/cancel/456
# Réponse : Message de confirmation
```

---

## 🔔 Workflow 3: Notifications et Rappels

### Objectif : Recevoir les notifications automatiques du système

#### Fonctionnalités Utilisées :
- **Rappels de paiement** (nouvelle)
- **Rappels de séance** (nouvelle)
- **Avertissements d'expiration** (nouvelle)
- **Confirmations** (ancienne)

#### Notifications Automatiques :

**1. Rappel de Paiement**
- **Quand** : Immédiatement après la réservation
- **Contenu** : "Votre réservation expire dans 15 minutes. Paiement requis."
- **Action** : Le paiement doit être effectué dans les 15 minutes

**2. Rappel de Séance**
- **Quand** : 24h et 1h avant la séance
- **Contenu** : "Rappel : Votre séance [Film] commence [heure]"
- **Action** : Préparer le QR Code pour l'entrée

**3. Avertissement d'Expiration**
- **Quand** : 5 minutes avant l'expiration
- **Contenu** : "Attention : Votre réservation expire bientôt"
- **Action** : Effectuer le paiement immédiatement

**4. Confirmation de Réservation**
- **Quand** : Immédiatement après création/paiement
- **Contenu** : Détails complets + QR Code
- **Action** : Conserver pour l'entrée

---

## 🎫 Workflow 4: Utilisation du Billet

### Objectif : Utiliser le billet pour accéder à la séance

#### Fonctionnalités Utilisées :
- **QR Code** (ancienne)
- **Validation** (ancienne)
- **Statut utilisé** (nouvelle)

#### Processus d'Entrée :

**1. Préparation**
- **Ouvrir** l'email de confirmation ou l'application
- **Afficher** le QR Code sur l'écran du téléphone
- **Vérifier** l'horaire et la salle

**2. Validation à l'Entrée**
- **Présenter** le QR Code au personnel
- **Scan automatique** par le système
- **Validation instantanée**

**3. Après Validation**
- **Statut mis à jour** automatiquement en "Used"
- **Accès autorisé** à la salle
- **Impossible de réutiliser** le même QR Code

---

## 💡 Conseils Utilisateur

### Pour une Expérience Optimale

**1. Planification**
- Réservez à l'avance pour avoir le meilleur choix de places
- Consultez les séances à venir via l'application
- Vérifiez les nouveautés chaque mercredi

**2. Gestion du Temps**
- Paiement requis dans les 15 minutes suivant la réservation
- Arrivez 15 minutes avant le début de la séance
- Le QR Code n'est valable qu'une seule fois

**3. Problèmes Courants**
- **QR Code perdu** : Consultez vos emails ou l'application
- **Réservation expirée** : Créez une nouvelle réservation
- **Changement de plans** : Annulez avant l'expiration

---

## ❓ FAQ Utilisateur

### Questions Fréquentes

**Q: Combien de temps ai-je pour payer ma réservation ?**
R: 15 minutes. Après ce délai, la réservation expire automatiquement.

**Q: Puis-je annuler ma réservation après paiement ?**
R: Oui, jusqu'à 15 minutes avant le début de la séance.

**Q: Que faire si je perds mon QR Code ?**
R: Consultez vos emails ou reconnectez-vous à l'application pour le retrouver.

**Q: Les rappels sont-ils envoyés par SMS ou email ?**
R: Par email à l'adresse utilisée lors de la création du compte.

**Q: Puis-je modifier mes sièges après réservation ?**
R: Non, il faut annuler et recréer la réservation avec les nouveaux sièges.

---

## 📱 Utilisation sur Mobile

### Application Mobile Recommandée

**Fonctionnalités Clés :**
- Recherche rapide de séances
- Réservation en quelques clics
- Stockage des QR Codes hors ligne
- Notifications push pour les rappels
- Historique des réservations

**Avantages :**
- Accès rapide aux QR Codes
- Notifications en temps réel
- Interface optimisée mobile
- Synchronisation automatique

---

## 🛡️ Sécurité et Confidentialité

### Protection des Données

**Vos données sont sécurisées :**
- Paiements cryptés
- QR Codes uniques et temporaires
- Aucun stockage des informations bancaires
- Conformité RGPD

**Conseils de sécurité :**
- Ne partagez pas vos QR Codes
- Utilisez des mots de passe forts
- Vérifiez les emails d'expéditeur légitimes
- Signalez tout comportement suspect

Ce guide permet à l'utilisateur de tirer pleinement parti du système de réservation, en bénéficiant des fonctionnalités automatiques tout en comprenant les processus et limitations.