# Documentation Projet — Toy-Room-Climb

## 1) Vision du projet
Toy-Room-Climb est un prototype de plateforme 3D dans un univers de jouets.
Le joueur doit se déplacer dans un environnement vertical et atteindre des zones
élevées grâce à la montée de rebords et des JumpPads.

## 2) Démarrage rapide
1. Ouvrir le projet Unity.
2. Attendre la compilation des scripts.
3. Ouvrir la scène principale (`Assets/Scenes`).
4. Lancer en **Play Mode**.

## 3) Contrôles
- **ZQSD / WASD** : Déplacement
- **Space** : Saut (maintenir pour grab un rebord)
- **Souris** : Rotation caméra
- **Échap** : Déverrouille le curseur en gameplay

## 4) Architecture technique
- `Assets/Script/` : logique gameplay (joueur, caméra, JumpPad).
- `Assets/Script/UI/` : logique UI (menus, effets visuels).

## 5) Documentation des scripts

### 5.1 CameraOnlyUp (`Assets/Script/CameraOnlyUp.cs`)
**Rôle :** Caméra third-person avec rotation souris, limites verticales et correction de collision.

**Champs clés (Inspector) :**
- `Target` : transform de la cible (Player).
- `Pivot Offset` : décalage du pivot (tête/torse).
- `Distance / Min / Max` : distance caméra.
- `Collision Radius / Buffer` : réglage anti-collision.
- `Sensitivity X/Y` : sensibilité souris.
- `Min/Max Pitch` : limites verticales.

**Notes :**
- Le script tente de récupérer automatiquement un `Player` si la cible n’est pas assignée.

---

### 5.2 PlayerControllerCC (`Assets/Script/PlayerControllerCC.cs`)
**Rôle :** Déplacement CharacterController, saut, ledge grab + mantle, impulsion externe.

**Champs clés (Inspector) :**
- **Movement** : `Move Speed`, `Rotation Speed`, `Jump Height`, `Gravity`.
- **Ledge Detection** : `Climb Layer`, `Detection Radius`, `Body Ray Height`, `Head Ray Height`, `Min Ledge Height`.
- **Animation Tuning** : `Root Motion Vertical/Forward Boost`.
- **Cooldown** : `Climb Cooldown`.
- **External Launch** : `External Damping` (utilisé par JumpPad).

**Flux principal :**
1. Lecture input clavier (ZQSD/WASD).
2. Détection de rebord via OverlapSphere + Raycast.
3. Démarrage du mantle si conditions remplies.
4. Déplacement au sol / en l’air via CharacterController.

---

### 5.3 JumpPad (`Assets/Script/JumpPad.cs`)
**Rôle :** Déclenche une impulsion verticale/horizontale sur le joueur.

**Champs clés (Inspector) :**
- `Upward Speed` : vitesse verticale.
- `Forward Speed` : vitesse horizontale (direction du pad).
- `Cooldown` : délai minimal entre 2 triggers.

**Pré-requis :**
- Un collider en **Trigger**.
- Le joueur doit avoir le tag **Player**.

---

### 5.4 GameplayCursorManager (`Assets/Script/UI/GameplayCursorManager.cs`)
**Rôle :** Gestion du verrouillage du curseur en gameplay.

**Comportement :**
- Souris visible au démarrage.
- Premier clic : verrouille et cache le curseur.
- Échap : déverrouille et affiche le curseur.

---

### 5.5 MenuCursorManager (`Assets/Script/UI/MenuCursorManager.cs`)
**Rôle :** Curseur libre et visible dans le menu principal.

---

### 5.6 MainMenuController (`Assets/Script/UI/MainMenuController.cs`)
**Rôle :** Gestion du menu principal (Play/Quit).

**Champs clés (Inspector) :**
- `Gameplay Scene Name` : nom de la scène à charger.

---

### 5.7 ToyBoxIntro (`Assets/Script/UI/ToyBoxIntro.cs`)
**Rôle :** Animation d’intro de bloc UI (pop + settle + fade).

**Champs clés (Inspector) :**
- `Start Delay / Pop Duration / Settle Duration` : timings.
- `Start Scale / Overshoot Scale / End Scale` : valeurs de scale.
- `Fade Delay / Fade Duration` : fade-in des éléments internes.

---

### 5.8 ToyBoxParallax (`Assets/Script/UI/ToyBoxParallax.cs`)
**Rôle :** Parallax léger sur un RectTransform.

**Champs clés (Inspector) :**
- `Amplitude` : amplitude du mouvement.
- `Speed` : vitesse de mouvement.

---

### 5.9 ToyButtonFX (`Assets/Script/UI/ToyButtonFX.cs`)
**Rôle :** Feedback visuel et audio sur les boutons UI.

**Champs clés (Inspector) :**
- `Hover Scale`, `Press Scale`, `Speed` : animation de scale.
- `Wobble Degrees`, `Wobble Speed` : oscillation visuelle.
- `Audio Source`, `Hover Clip`, `Click Clip` : sons optionnels.

## 6) Recommandations de documentation (pipeline)
- Conserver les commentaires XML sur chaque script public.
- Maintenir cette documentation à jour lors de l’ajout de nouveaux systèmes.
- Ajouter un changelog si le scope augmente.
