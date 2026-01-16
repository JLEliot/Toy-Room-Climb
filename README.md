# Toy-Room-Climb

## 🎯 Vision
Toy-Room-Climb est un prototype Unity de plateforme/parkour dans une chambre de jouets.
L’objectif est d’explorer un niveau vertical en combinant déplacement, saut, montée de rebords
et éléments d’environnement comme les JumpPads.

## 🧭 Objectifs de gameplay
- Encourager l’exploration verticale et l’expérimentation des routes.
- Donner un ressenti “arcade” avec un contrôle précis et réactif.
- Proposer un environnement lisible avec des repères visuels clairs.

## ✅ Features clés
- Déplacement **CharacterController** (sol + air)
- Saut et gravité configurables
- Détection et montée de rebords (ledge grab + mantle)
- JumpPad pour impulsion externe
- Caméra third-person avec collision et limites de pitch
- Menu principal avec effets UI (intro, parallax, feedback bouton)

## ⚙️ Installation & démarrage
1. Ouvrir le projet avec Unity (URP recommandé).
2. Laisser Unity importer les assets.
3. Ouvrir la scène principale depuis `Assets/Scenes`.
4. Appuyer sur **Play**.

## 🕹️ Contrôles (par défaut)
- **ZQSD / WASD** : Déplacement
- **Space** : Saut (maintenir pour grab un rebord)
- **Souris** : Rotation caméra (en gameplay)
- **Échap** : Déverrouille le curseur en gameplay

## 🧱 Architecture & structure
- `Assets/Script/` : scripts gameplay (player, caméra, JumpPad)
- `Assets/Script/UI/` : scripts UI (menu, effets visuels)
- `Assets/Scenes/` : scènes Unity
- `Docs/` : documentation projet

## 🔍 Points d’attention (niveau/design)
- Les JumpPads supposent que le joueur a le tag **Player**.
- Les surfaces grimpables doivent être sur le layer défini par `Climb Layer`.
- Le comportement de caméra dépend du verrouillage du curseur en gameplay.

## 📚 Documentation
- Documentation détaillée : `Docs/PROJECT_DOCUMENTATION.md`
- Version PDF : `Docs/PROJECT_DOCUMENTATION.pdf`

## 👥 Contributeurs & contributions (synthèse)
Cette section résume les apports majeurs sans exposer l’historique des commits.
- **Noah** : colliders, plateformes, ajustements de scène et repères de progression, ajout et gestion de la skybox.
- **Camille** : déplacement/animation personnage et itérations caméra, layout de scène, améliorations caméra, colliders et correctifs UI, ajout et gestion de la skybox.
- **Guillaume** : UI de menu (scènes, scripts, audio), packages et réglages projet,intégration de PRs.
- **Benjamin Djaoui** : système d’escalade.

