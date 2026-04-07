# 🍌 Steal the Banana

**Steal the Banana** est un jeu multijoueur PVP "chacun pour soi" développé en **C# avec Unity**.
Incarnez un singe livreur dans la jungle, avec un objectif clair : ramener une banane sacrée (qui est elle-même incarnée par un autre joueur !) dans votre propre arbre.

---

## 📖 Lore : La Banane Enchantée

Une fois par an, le roi et la reine des singes (**Tatam & Robi**) offrent une banane enchantée à la tribu. Cette cérémonie sacrée déclenche une compétition acharnée : le singe qui parviendra à ramener cette relique dans sa cabane gagnera le respect éternel de tous et deviendra le héros incontesté du village.
Mais attention, la banane n'a pas toujours l'intention de se laisser faire...

## 🎮 Concept du Jeu

- **Thème :** Livraison frénétique dans la jungle.
- **Multijoueur & PVP :** Un pur _chacun pour soi_. Les singes doivent se battre pour capturer et livrer le joueur-banane.
- **Ambiance Goofy :** Un univers décalé, très drôle et chaotique.
- **Parties Rapides :** Une boucle de gameplay simple, efficace et nerveuse (des petites parties d'environ 6 minutes).
- **Leaderboard :** Suivez le nombre de parties gagnées pour prouver qui est le vrai roi de la jungle !

## ⚙️ Mécaniques Principales

- **Un Colis Vivant :** Le colis que tous les joueurs s'arrachent est contrôlé par l'un d'entre eux.
- **Système de Classes :** Jouez avec différentes classes de singes. Chaque classe possède sa propre **compétence unique** pour piéger les autres ou s'échapper.
- **Le Plot Twist Musclé :** De façon aléatoire ou à la dernière minute de jeu, la banane subit une transformation radicale : elle devient **hyper musclée** et commence à démonter tous les singes sur son passage !

## 🛠️ Technologies

- **Moteur de jeu :** Unity
- **Langage :** C#

---

## 🌐 Multiplayer Quickstart (Host + Client)

Cette section correspond a l implementation en cours dans:

- [Assets/Scripts/Multiplayer/PlayerController.cs](Assets/Scripts/Multiplayer/PlayerController.cs)
- [Assets/Scripts/Multiplayer/NetworkManagerUI.cs](Assets/Scripts/Multiplayer/NetworkManagerUI.cs)
- [Assets/Scripts/Multiplayer/LocalPlayerCameraFollow.cs](Assets/Scripts/Multiplayer/LocalPlayerCameraFollow.cs)

### 1) Creer le prefab joueur reseau

1. Cree un prefab joueur dans `Assets/Prefabs` (ex: `PlayerNetworkPrefab`).
2. Ajoute ces composants sur le prefab:
   - `NetworkObject`
   - `NetworkTransform`
   - `Rigidbody`
   - `PlayerController`
   - `LocalPlayerCameraFollow` (optionnel mais recommande)
3. Configure `NetworkTransform` en autorite serveur (valeur par defaut recommandee pour commencer).

### 2) Configurer le NetworkManager

1. Dans l objet `NetworkManager` de la scene, assigne `PlayerNetworkPrefab` dans `Player Prefab`.
2. Verifie que `Unity Transport` est bien sur le meme objet.
3. Garde `Enable Scene Management` active.

### 3) Nettoyer la scene

1. Supprime (ou desactive) le `Player` statique de test deja place dans la scene.
2. Garde un seul objet `NetworkManager` avec le script `NetworkManagerUI`.

### 4) Tester sur un seul PC (2 instances)

1. Lance une instance en `Host` avec IP `127.0.0.1` et port `7777`.
2. Lance une deuxieme instance en `Client` avec les memes valeurs.
3. Verifie que les deux joueurs apparaissent et se voient bouger.

### 5) Tester sur LAN (plusieurs PC)

1. Sur la machine Host, trouve l IP locale (ex: `192.168.1.42`).
2. Host: demarre avec cette IP + port `7777`.
3. Clients: entrent l IP de l Host + meme port.
4. Autorise le port `7777` dans le pare-feu Windows de la machine Host si necessaire.

### Notes importantes

- Le deplacement est server-authoritative: le client envoie son input, le serveur deplace le joueur.
- Si `Player Prefab` manque ou est mal configure, `NetworkManagerUI` affiche un message d erreur explicite.
