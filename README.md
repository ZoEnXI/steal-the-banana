# 🍌 Steal the Banana

## 🎯 Concept

Steal the Banana est un jeu d'action multijoueur en equipe, rapide et chaotique, dev sur Unity en C#.

Une fois par an, Tatam et Robi offrent au village la Banane Enchantee. Sauf que cette banane est un joueur. Deux equipes de singes se battent pour la capturer et la livrer dans leur cabane.

## 🔁 Boucle de gameplay

1. Lobby avec choix d'equipe.
2. Debut de manche et selection aleatoire du joueur Banane.
3. Capture, vols, assommages, livraisons, gain/perte de pieces.
4. A 5:00, activation de Chad Banana.
5. Fin de manche a 6:00 max ou victoire immediate sur livraison.

## 🐵 Roles et fantasy

- Orang-outan: lent, puissant, assomme facilement.
- Lemurien: rapide, dash, harcelement et interception.
- Banane: role asymetrique, survie, fuite, liberation.
- Chad Banana: transformation de fin de manche, inversion de pression.

## ⚡ Pouvoirs speciaux

- Pouvoir singe (Alt): extension du bras avec effet grappin.
- Pouvoir banane (Alt): retirer la peau et sprint boost 5s, cooldown 50s.

## 💰 Economie de partie

### ✅ Gains

- Assommer un ennemi: +5 pieces.
- Attraper la banane: +10 pieces.
- 1 seconde en portant la banane: +1 piece.
- Livrer la banane dans sa cabane: +1000 pieces.
- Livraison par un allie: +100 pieces pour chaque joueur de l'equipe.
- Devenir Chad Banana: +2000 pieces.
- Chad Banana touche un singe: vole 10% des pieces gagnees par la cible dans la manche.

### ❌ Pertes

- Frapper un allie: -2 pieces.
- Equipe adverse possede la banane pendant 5 secondes: -2 pieces toutes les 5 secondes.

## 📜 Regles de manche

- Une manche dure 6 minutes maximum.
- La manche s'arrete immediatement si une equipe livre la banane.
- La banane doit etre assommee avant de pouvoir etre capturee.
- Le prompt d'interaction de capture n'apparait que si la banane est knock.
- A 5:00, la banane passe en mode Chad Banana.

## 🧱 Architecture Unity C#

Ce projet cible Unity Netcode for GameObjects avec autorite serveur.

### 🛠️ Stack technique

- Unity 6
- C#
- Netcode for GameObjects
- Unity Transport
- Input System

### 🧩 Repartition logique (Unity)

- Server authority: logique de partie, scores, collisions de capture, validation des competences.
- Client presentation: UI, camera locale, effets visuels et sonores.
- Shared gameplay data: constantes, data des classes, progression, definitions des cooldowns.

### 📁 Proposition de dossiers

- Assets/Scripts/Multiplayer
- Assets/Scripts/Game
- Assets/Scripts/Gameplay
- Assets/Scripts/UI
- Assets/Scripts/Data

## 🗺️ Roadmap de developpement

## 1️⃣ Phase 1 - Squelette reseau et lobby

Objectif: faire tourner un cycle propre Lobby -> Match -> End.

- Creer GameStateManager serveur avec etats Waiting, Countdown, InGame, EndGame.
- Ajouter UI lobby avec choix equipe et timer de manche.
- Definir GameConstants (duree manche, min joueurs, cooldowns de base).
- Synchroniser etats et timer via NetworkVariables.

## 2️⃣ Phase 2 - Roles et competences

Objectif: donner une identite jouable aux classes et a la banane.

- Tirage aleatoire du joueur Banane au lancement de manche.
- Application des stats par classe (vitesse, puissance, mobilite).
- Competences via input client, validation et execution cote serveur.
- Ajout probabilite cumulative de devenir Banane (+1% si non selectionne a la manche precedente).

## 3️⃣ Phase 3 - Capture et bagarre

Objectif: creer le coeur fun du jeu avec des interactions robustes.

- Systeme hitbox/overlap serveur pour capture de banane.
- Gestion vol de banane entre joueurs.
- Systeme struggle pour liberation de la banane.
- Validation zone cabane et attribution score/economie.

## 4️⃣ Phase 4 - Chad Banana et polish

Objectif: finaliser le pic de tension et la presentation.

- Declenchement Chad Banana a 5:00.
- Buffs massifs banane et capacite offensive.
- Effets client: musique boss, feedback camera, VFX de transformation.
- Ecran de fin avec podium, MVP, anti-MVP, recap pieces.

## 🎮 Deroulement cible d'une partie

1. Les joueurs arrivent au lobby (shop, cosmetics, upgrades).
2. Popup de choix d'equipe.
3. Popup randomBanana et selection aleatoire.
4. Spawn des equipes et de la banane dans la map.
5. Phase de capture/contre-capture.
6. Activation Chad Banana a 5:00.
7. Fin, podium, recompenses et progression meta.

## 📡 Notes d'implementation multiplayer

- Mouvement et etats critiques doivent rester server authoritative.
- Le client envoie des intents (input/commande), pas des resultats finaux.
- Les collisions de capture, score, economie et victoire doivent etre valides cote serveur uniquement.
- L'UI doit refleter les NetworkVariables et events RPC, jamais l'inverse.

## 🏁 Objectif MVP

Le MVP est valide si:

1. 2 joueurs minimum peuvent rejoindre une meme partie.
2. Une banane est choisie, visible, capturable et livrable.
3. Le score/economie evolue correctement en temps reel.
4. Chad Banana s'active a 5:00.
5. La manche se termine avec un ecran resultat lisible.
