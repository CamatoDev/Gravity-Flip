# Gravity Flip

**Gravity Flip** est un jeu de plateforme 2D développé avec Unity et des assets 3D, proposant des puzzles dynamiques et un gameplay classique de plateformes. Incarnez un personnage naviguant sur des plateformes flottantes, évitez les obstacles et débloquez de nouveaux niveaux.

---

## 📖 Table des matières

1. [Fonctionnalités](#-fonctionnalités)
2. [Démo / Capture d’écran](#-démo--capture-décran)
3. [Installation](#-installation)
4. [Contrôles](#-contrôles)
5. [Mécaniques de jeu](#-mécaniques-de-jeu)
6. [Structure du projet](#-structure-du-projet)
7. [Compilation & Exécution](#-compilation--exécution)
8. [Contribution](#-contribution)
9. [Licence](#-licence)

---

## 🚀 Fonctionnalités

* **Déplacement 2D avec assets 3D** : mouvement latéral fluide dans un monde tridimensionnel.
* **Plateformes dynamiques** : plateformes mobiles horizontales/verticales et plateformes éphémères (temporisées).
* **Éléments interactifs** :

  * Plaques de pression (activation automatique ou manuelle)
  * Portes verrouillées par clé
  * Pi spikes (pics) et dangers
  * Pièces et vies bonus à collecter
* **Progression de niveaux** : menu principal, sélecteur de niveaux avec déverrouillage basé sur PlayerPrefs.
* **UI & Menus** :

  * Menu principal (Jouer, Options avec curseurs de volume, saisie du Name Tag)
  * Menu de pause (Continuer, Recommencer, Menu)
  * Écrans Game Over et Victoire avec messages personnalisés et statistiques.
* **Scripts du joueur** :

  * `PlayerScript` : déplacement, animations, danse d’inactivité, prise en charge clavier et manette PS4.
  * `PlayerStats` : vie, pièces, clés, mises à jour de l’UI.
* **Audio** : musiques de fond et effets sonores.

---

## 📷 Démo / Capture d’écran

*une capture d’écran ou un GIF bientôt...*

---

## 🛠️ Installation

1. **Cloner le dépôt** :

   ```bash
   git clone https://github.com/CamatoDev/gravity-flip.git
   cd gravity-flip
   ```
2. **Ouvrir dans Unity** :

   * Lancez Unity Hub et ajoutez le dossier `gravity-flip` en tant que projet.
   * Ouvrez le projet avec la version Unity 2022.3 (précisez la version).
3. **Lancer** :

   * Cliquez sur **Play** dans l’éditeur Unity pour tester le jeu.

---

## 🎮 Contrôles

| Périphérique | Action                       |
| ------------ | ---------------------------- |
| Clavier (PC) | Déplacement : `A/D` ou `←/→` |
|              | Saut : `Espace`              |
|              | Interaction : `E`            |
|              | Courir : `Left Shift`        |
| Manette PS4  | Déplacement : Stick gauche   |
|              | Saut : Rond (O)              |
|              | Interaction : Carré          |
|              | Courir : R1                  |

---

## 🔧 Mécaniques de jeu

* **Plateformes mobiles** : oscillent selon l’axe X ou Y. Configurables via le script `PlatformMover`.
* **Plateformes éphémères** : disparaissent après un délai, réapparaissent après un temps défini (`EphemeralPlatform`).
* **Plaques de pression** : activent/désactivent des cibles automatiquement ou manuellement (`PressurePlate`).
* **Clés et portes** : ramassez des clés (`KeyPickup`) et ouvrez les portes correspondantes (`Door`).
* **Pics** : infligent 1 point de dégât au joueur au contact.
* **Collectibles** : pièces et vies bonus à récupérer.
* **Timer** : chronomètre le temps écoulé par niveau.
* **GameManager** : gère les conditions de victoire/défaite et les transitions d’UI.

---

## 📂 Structure du projet

```
GravityFlip/
├── Assets/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Prefabs/
│   ├── Audio/
│   ├── Materials/
│   └── UI/
├── Packages/
├── ProjectSettings/
└── README.md
```

---

## 🏗️ Compilation & Exécution

1. Dans l’éditeur Unity, allez dans **File > Build Settings**.
2. Sélectionnez **PC, Mac & Linux Standalone**, cible **Windows**.
3. Cliquez sur **Build** et choisissez un dossier de sortie.
4. Lancez le fichier généré `Gravity Flip.exe`.

---

## 🤝 Contribution

Les contributions sont les bienvenues ! Pour contribuer :

1. Forkez le dépôt.
2. Créez une branche (`git checkout -b feature/MaFonctionnalité`).
3. Commitez vos modifications (`git commit -m 'Ajout d'une fonctionnalité'`).
4. Poussez sur la branche (`git push origin feature/MaFonctionnalité`).
5. Ouvrez une Pull Request.

Merci de suivre le style de code existant et de rédiger des messages de commit clairs.

---

## 📄 Licence

Ce projet est sous licence MIT. Voir le fichier [LICENSE](LICENSE) pour plus de détails.
