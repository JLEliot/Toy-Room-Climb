# Toy-Room-Climb

## 🎯 Overview
Toy-Room-Climb est un prototype Unity de plateforme/parkour dans une chambre de jouets.
Le joueur se déplace, saute, grimpe des rebords et interagit avec des éléments comme les JumpPads.

## ✅ Features
- Déplacement CharacterController (sol + air)
- Saut et gravité configurables
- Détection et montée de rebords (ledge grab + mantle)
- JumpPad pour impulsion externe
- Menu principal avec effets UI (parallax, intro, boutons)

## ⚙️ Installation
1. Ouvrir le projet avec Unity (URP recommandé).
2. Laisser Unity importer les assets.
3. Ouvrir la scène principale depuis `Assets/Scenes`.
4. Appuyer sur **Play**.

## 🕹️ Controls
- **ZQSD / WASD** : Déplacement
- **Space** : Saut (maintenir pour grab un rebord)
- **Souris** : Rotation caméra (en gameplay)
- **Échap** : Déverrouille le curseur en gameplay

## 🧱 Architecture
- `Assets/Script/` : scripts gameplay (player, caméra, JumpPad).
- `Assets/Script/UI/` : scripts UI (menu, effets visuels).
- `Assets/Scenes/` : scènes Unity.

## 📚 Documentation
- Documentation détaillée : `Docs/PROJECT_DOCUMENTATION.md`

## 👥 Contributing
Ce projet est un prototype interne. Merci de suivre les conventions de style et
la documentation existante pour toute modification.
