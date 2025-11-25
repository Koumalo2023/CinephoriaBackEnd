# Guide Employé - Gestion des Séances et Réservations

## 📋 Rôle Employé
L'employé a accès aux fonctionnalités opérationnelles : gestion des séances, validation des réservations, et assistance aux clients. Pas d'accès aux configurations système.

---

## 🎬 Workflow 1: Gestion Opérationnelle des Séances

### Objectif : Gérer les séances au quotidien

#### Fonctionnalités Utilisées :
- **Création de séances** (ancienne)
- **Modification de séances** (ancienne)
- **Consultation des statuts** (nouvelle)
- **Séances à venir** (nouvelle)

#### Endpoints Clés :
```http
POST /api/showtime/create
PUT /api/showtime/update
GET /api/showtime/status
GET /api/showtime/upcoming
GET /api/showtime/ongoing
```

#### Workflow Détaillé :

**1. Programmation Quotidienne**
```bash
# Créer une séance supplémentaire
POST /api/showtime/create
Body: {
  "movieId": 123,
  "theaterId": 2,
  "startTime": "2025-11-15T23:00:00",
  "endTime": "2025-11-16T01:30:00",
  "price": 10.00
}

# Vérifier les séances programmées
GET /api/showtime/status
```

**2. Surveillance des Séances en Temps Réel**
```bash
# Séances à venir dans les 24h
GET /api/showtime/upcoming
# Utilisation : Préparer les salles, vérifier le matériel

# Séances en cours
GET /api/showtime/ongoing
# Utilisation : Surveillance des séances actives
```

**3. Modifications de Dernière Minute**
```bash
# Reporter une séance
PUT /api/showtime/update
Body: {
  "showtimeId": 456,
  "startTime": "2025-11-15T21:00:00"
}
```

---

## 🎫 Workflow 2: Validation et Accueil des Clients

### Objectif : Gérer l'entrée des clients et valider les réservations

#### Fonctionnalités Utilisées :
- **Validation QR Code** (ancienne)
- **Consultation des réservations** (ancienne)
- **Statuts des réservations** (nouvelle)

#### Endpoints Clés :
```http
POST /api/reservation/validate
GET /api/reservation/showtime/{showtimeId}
```

#### Workflow Détaillé :

**1. Préparation de l'Accueil**
```bash
# Voir toutes les réservations pour une séance
GET /api/reservation/showtime/789
# Réponse : Liste des réservations avec sièges et statuts
```

**2. Validation des Billets**
```bash
# Scanner et valider un QR Code
POST /api/reservation/validate
Body: "QR_CODE_DATA_HERE"
# Réponse : "QRCode validé avec succès" ou erreur
```

**3. Gestion des Situations Spéciales**
- **Réservation expirée** : Le système bloque automatiquement la validation
- **QR Code déjà utilisé** : Le système détecte la double utilisation
- **Problème technique** : Consulter les réservations manuellement via l'endpoint showtime

---

## 📋 Workflow 3: Assistance Client et Information

### Objectif : Fournir des informations aux clients

#### Fonctionnalités Utilisées :
- **Séances disponibles** (ancienne)
- **Sièges disponibles** (ancienne)
- **Statuts des séances** (nouvelle)
- **Films avec séances** (nouvelle)

#### Endpoints Clés :
```http
GET /api/reservation/movie/{movieId}/sessions
GET /api/reservation/showtime/{showtimeId}/seats
GET /api/showtime/movies-with-showtimes
GET /api/showtime/recent-movies
```

#### Workflow Détaillé :

**1. Recherche de Séances**
```bash
# Trouver les séances d'un film spécifique
GET /api/reservation/movie/123/sessions
# Utilisation : Aider un client à trouver une séance

# Voir les sièges disponibles
GET /api/reservation/showtime/789/seats
# Utilisation : Aider à choisir des places
```

**2. Information sur la Programmation**
```bash
# Films actuellement programmés
GET /api/showtime/movies-with-showtimes
# Réponse : Films avec nombre de séances et disponibilités

# Nouveautés de la semaine
GET /api/showtime/recent-movies
# Réponse : Films ajoutés récemment
```

**3. Gestion des Demandes Spéciales**
- **Groupes** : Vérifier la disponibilité via l'endpoint seats
- **Anniversaires** : Programmer des séances spéciales
- **Accessibilité** : Vérifier les sièges adaptés

---

## 🔄 Workflow 4: Coordination avec l'Équipe

### Objectif : Assurer une communication efficace entre employés

#### Fonctionnalités Utilisées :
- **Statuts des séances** (nouvelle)
- **Réservations par séance** (ancienne)

#### Points de Coordination :

**1. Passage de Relais**
```bash
# Vérifier l'état des séances en cours
GET /api/showtime/ongoing

# Vérifier les séances à venir
GET /api/showtime/upcoming
```

**2. Alertes Importantes**
- **Séance complète** : Via l'endpoint seats
- **Problème technique** : Noter et reporter via modification de séance
- **Client particulier** : Partager l'information via le système de réservation

---

## 🎯 Checklist Quotidienne Employé

### Ouverture (8h00)
- [ ] Vérifier les séances de la journée : `GET /api/showtime/upcoming`
- [ ] Contrôler les réservations pour la première séance : `GET /api/reservation/showtime/{id}`
- [ ] Vérifier l'état des salles

### Pendant la Journée
- [ ] Surveiller les séances en cours : `GET /api/showtime/ongoing`
- [ ] Valider les entrées des clients : `POST /api/reservation/validate`
- [ ] Aider aux réservations en ligne

### Fermeture (23h00)
- [ ] Vérifier que toutes les séances sont terminées
- [ ] Consulter les statistiques de la journée
- [ ] Préparer le rapport pour l'administration

---

## 💡 Bonnes Pratiques Employé

### Communication Client
- **Toujours vérifier** la disponibilité via les endpoints avant de promettre des places
- **Expliquer le système** de rappels automatiques aux clients
- **Informer** sur les délais d'annulation (15 minutes avant expiration)

### Gestion des Incidents
- **Problème de validation** : Utiliser l'endpoint showtime pour vérifier manuellement
- **Client sans réservation** : Vérifier la disponibilité en temps réel
- **Séance annulée** : Modifier le statut et informer les clients réservés

### Coordination Équipe
- **Partager les informations** sur les séances problématiques
- **Signaler les tendances** (films populaires, problèmes récurrents)
- **Utiliser le système** comme source de vérité unique

---

## 📞 Support Interne

### Contacts :
- **Administrateur système** : Pour les problèmes techniques
- **Manager** : Pour les décisions de programmation
- **Équipe technique** : Pour les bugs et améliorations

### Escalade des Problèmes :
1. **Problème simple** : Résoudre via les fonctionnalités disponibles
2. **Problème complexe** : Documenter et escalader à l'administrateur
3. **Urgence** : Utiliser les procédures d'urgence établies

Ce guide permet à l'employé d'utiliser efficacement toutes les fonctionnalités du système pour offrir un service client optimal tout en maintenant une coordination efficace avec l'équipe.