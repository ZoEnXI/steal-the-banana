# 🍌 Steal the Banana

## Le Concept

**Steal the Banana** est un jeu d'action multijoueur en équipe ultra-nerveux. Une fois par an, le Roi Tatam et la Reine Robi offrent au village la "Banane Enchantée" (jouée par un joueur !). Tous les singes de la jungle s'affrontent pour capturer cette banane vivante et la ramener dans leur propre cabane.

## Pourquoi c'est fun (Les Mécaniques)

- **1 Banane vs Tous :** Le joueur-banane doit survivre et fuir. Les singes doivent l'attraper, mais aussi se castagner entre eux pour se voler le précieux colis.
- **Classes de Singes :** Orang-outan (lent mais puissant, assomme les concurrents), Lémurien (rapide avec un dash). Une classe = une compétence simple.
- **Le Plot Twist "Chad Banana" :** Dans la dernière minute de la partie (ou de façon aléatoire), la Banane Enchantée en a marre. Elle se transforme en Banane Hyper Musclée : elle devient gigantesque, ultra-rapide et c'est elle qui se met à chasser et démonter les singes !
- **Format :** Parties express de 6 minutes. Un Leaderboard affiche les héros du village (ceux qui ont livré le plus de bananes).

---

## 🛠 Découpage du développement (Architecture Rojo)

Puisque tu as setup Rojo (`Shared`, `Client`, `Server`), voici comment vous devez découper votre sprint par phases. C'est l'ordre chronologique exact pour ne pas vous marcher sur les pieds.

### Phase 1 : Le Squelette & Le Lobby (La Base)

L'objectif est d'avoir un cycle de jeu qui tourne dans le vide sans bug (Lobby -> Jeu -> Fin).

- **Server (`ServerScriptService`) :**
  - Créer le gestionnaire d'état (`GameManager`) : Attente de joueurs -> Compte à rebours -> Lancement de la partie de 6 min -> Fin de partie -> Téléportation au lobby.
- **Client (`StarterPlayerScripts` / `StarterGui`) :**
  - UI du Lobby : Un menu simple pour choisir son équipe (Orang-outan ou Lémurien) avant que la partie commence.
  - Affichage du chronomètre de la partie en haut de l'écran.
- **Shared (`ReplicatedStorage`) :**
  - Créer un module `GameConstants` (Temps d'une partie, nombre minimum de joueurs, stats de base des joueurs).

### Phase 2 : Rôles et Compétences (L'Identité)

On donne vie aux singes et à la banane.

- **Server :**
  - Au lancement, tirer au sort un joueur pour être la Banane. Lui appliquer le modèle de banane (`Weld`).
  - Appliquer les statistiques de vitesse et de saut selon la classe choisie par les autres joueurs.
- **Client :**
  - Gérer les inputs des compétences (ex: appuyer sur 'Maj' pour le dash du Lémurien ou la frappe de l'Orang-outan).
  - Envoyer un `RemoteEvent` au serveur quand la compétence est utilisée.
- **Shared :**
  - Module `ClassData` contenant les infos de chaque classe (Cooldowns, Vitesse, Force de saut).

### Phase 3 : Le Grab et la Bagarre (Le Cœur du Jeu)

C'est ici que le fun se crée. Il faut que ce soit fluide.

- **Server :**
  - Système de collision/hitbox : Quand un singe tape la banane, elle s'attache à son dos.
  - Si un singe tape un singe porteur : la banane tombe au sol (ou passe au singe attaquant).
  - Système de "Struggle" : Écouter si la Banane martèle l'espace pour se libérer.
  - Vérifier si un porteur entre dans sa zone de cabane (Score +1).
- **Client :**
  - UI de "Struggle" pour la Banane (une jauge qui se remplit quand on spamme).
  - Animations de frappe et effets visuels/sonores quand on attrape la banane.

### Phase 4 : Le Plot Twist "Chad Banana" & Polish

La cerise sur le gâteau.

- **Server :**
  - Déclencheur chronométré : À la minute 5:00, envoyer l'événement "Chad Banana".
  - Booster drastiquement les stats du joueur-banane (vitesse, dégâts) et lui donner la capacité de repousser les singes.
- **Client :**
  - Changement de musique brutal (musique épique/boss).
  - Écran qui tremble légèrement, grosses particules autour de la banane.
- **Shared :**
  - Module `LeaderboardManager` pour sauvegarder et afficher les victoires via le `DataStore`.

le pouvoir des singes (alt) permet d'étendre le bras et faire un effet grappin
le pouvoir de la banane (alt) permet d'enlever sa peau de banane et de courir super vite pendant 5s (rechargeable toutes les 50s)

## Déroulement d'une partie :

On se connecte dans un lobby (ou on peut acheter des upgrades, des skins etc...)

La partie va commencer (ouverture de la popup "ChoseTeamPopup").
On choisit son équipe (Orang-outan ou Lémurien).
Quand la partie commence, une popup "randomBanana" s'ouvre et choisit aléatoirement un joueur pour être la banane. (calculé le % de chance d'être la banane en fonction du nombre de joueurs + des bonus achetés par les joueurs + si on fait une partie et qu'on devient pas la banane faut faire un +1% à chaque partie)

Les teams et la banane spawns dans la map.
La banane doit survivre en se cachant et en se débattant pour se libérer.
T1 : Les orang-outans doivent attraper la banane et la ramener dans leur cabane.
T2 : Les lémuriens doivent attraper la banane et la ramener dans leur cabane.
T1 et T2 peuvent s'assommer entre eux.

## On gagne des pièces :

Si on assomme un singe de l'équipe : 5 pièces
Si on attrape la banane : 10 pièces
Un seconde avec la banane dans nos mains : 1 pièce
Si on ramène la banane dans sa cabane : 1000 pièces
Si un joueur de notre équipe ramène la banane dans sa cabane : 100 pièces par joueur de l'équipe

Si on devient "Chad Banana" : 2000 pièces
En tant que chad banana, si on tape un singe on prend 10% des pièces qu'il a gagné dans la partie

## On perd des pièces :

Si on tape un allié : -2 pièces
Si l'équipe adverse à la banane +5s = -2 pièces toutes les 5 secondes

Il faut assommer la banane pour pouvoir la prendre (afficher le proximity prompt uniquement si knock)

La partie s'arrête si une team ramène la banane dans sa cabane.
La partie dure 6 minutes maximum.

Au bout de 5 minutes, la banane devient "Chad Banana", les rôles changent, les singes se cachent pour ne pas se faire tuer par la banane.

à la fin, podium avec quel équipe a gagné le plus de pièce et le mvp (stats sur celui qui a assommé le plus de joueurs, qui a gagné le plus de pièces, et les moins bons, ceux qui ont le plus tapés ses alliés etc...). Donc l'équipe qui a fait le plus de pièces gagne.

et le meilleur joueur de la partie gagne un bonus de 5% de pièce (bonus qu'on peut améliorer dans le shop)
