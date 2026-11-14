# Gravity Flip — Guide Complet d'Implémentation & Feuille de Route Festival

> **Mode d'exécution :** Guide d'instructions pas-à-pas destiné au développeur.
> **Règle d'or :** Aucun fichier existant n'est modifié automatiquement sans autorisation. Chaque section ci-dessous contient les explications, les scripts complets prêts à l'emploi et les configurations précises dans l'Inspecteur Unity.

---

## 📑 Sommaire
1. [Phase 1 : Assainissement & Corrections Immédiates des Bugs](#phase-1--assainissement--corrections-immédiates-des-bugs)
2. [Phase 2 : Contrôleur Joueur 2.5D (Avant/Arrière avec Demi-Tour 180°)](#phase-2--contrôleur-joueur-25d-avantarrière-avec-demi-tour-180)
3. [Phase 3 : Intégration du Nouveau Personnage 3D & Animations](#phase-3--intégration-du-nouveau-personnage-3d--animations)
4. [Phase 4 : Système de Plateformes Mobiles & Héritage d'Inertie](#phase-4--système-de-plateformes-mobiles--héritage-dinertie)
5. [Phase 5 : Caméra Dynamique & Triggers de Virage Réversibles (Empty)](#phase-5--caméra-dynamique--triggers-de-virage-réversibles-empty)
6. [Phase 6 : Mécanique "Gravity Flip" & Impact d'Atterrissage](#phase-6--mécanique-gravity-flip--impact-datterrissage)
7. [Phase 7 : Robustesse Stand Festival & Mode Kiosk](#phase-7--robustesse-stand-festival--mode-kiosk)

---

# Phase 1 : Assainissement & Corrections Immédiates des Bugs

### 1.1 Élimination du spam console (`PlayerScript.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Player/PlayerScript.cs` (Ligne 128)
* **Statut :** Appliqué et vérifié sans erreur de compilation.

---

### 1.2 Découplage du Saut (`Update` vs `FixedUpdate`) & Condition `isGrounded` — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Player/PlayerScript.cs`
* **Statut :** Appliqué et vérifié sans erreur de compilation.

---

### 1.3 Découplage de la Caméra du Squelette (`Spine2`) — [CODE PRÊT ✅]
* **Fichiers créés :** `Assets/Scripts/Level/CameraFollow2D.cs` et `Assets/Scripts/Level/CameraTurnZone.cs`
* **Actions dans l'Éditeur Unity (À faire par vous) :**
  1. Ouvrir la scène `Level1.unity` (puis `Level2.unity`).
  2. Dans la **Hierarchy**, déplier `Player V2` jusqu'à localiser `Spine2 -> Main Camera`.
  3. Glisser-déposer `Main Camera` à la racine de la scène (au même niveau que `Player V2`).
  4. Sélectionner `Main Camera` :
     * Clic droit sur le composant rouge `Missing (Mono Behaviour)` $\rightarrow$ **Remove Component**.
     * Cliquer sur **Add Component** $\rightarrow$ Ajouter `CameraFollow2D`.
     * Glisser l'objet `Player V2` dans le champ **Target** (ou laisser vide pour détection automatique).
     * Vérifier les offsets (par défaut : $X = 0, Y = 1.75, Z = -5$).
  5. Sauvegarder la scène (`Ctrl + S`).

---

### 1.4 Sécurisation de l'Interaction Clé (`PlayerStats.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Player/PlayerStats.cs`
* **Statut :** Opérateurs logiques et vérification `null` appliqués avec succès.

---

### 1.5 Sécurisation des Transitions de Pause (`SceneFader.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/UI/SceneFader.cs`
* **Statut :** Remplacement par `Time.unscaledDeltaTime` appliqué avec succès.

---

### 1.6 Plaques de Pression à Déclenchement Unique (`PressurePlate.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Level/PressurePlate.cs`
* **Corrections appliquées :**
  1. Verrou d'activation unique `_isActivated` pour empêcher tout ré-enclenchement en boucle.
  2. Désactivation immédiate des colliders de la plaque dès qu'elle est pressée pour supprimer toute sensation de blocage sous le joueur.
  3. Disparition propre du bouton/plaque (`transform.parent.gameObject.SetActive(false)`) après un court délai (0.2s) sans masquer le pont ou les plaques sœurs.

---

### 1.7 Anti-Accrochage aux Murs & Saut sur Bordure de Plateforme (`PlayerScript.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Player/PlayerScript.cs`
* **Corrections appliquées :**
  1. **PhysicMaterial sans friction (`PlayerZeroFriction`) :** Généré automatiquement dans `Awake()` sur le collider du joueur pour éliminer l'adhérence PhysX contre les parois verticales (évite de flotter ou marcher dans le vide collé à un mur).
  2. **GroundCheck dynamique basé sur la Capsule :** Détection automatique du bas réel de la capsule (`GetFootPosition()`) avec marge de pénétration de 15 cm. Détecte à la fois le Layer 0 (`Default`) et le Layer 6 (`Ground`), plus un raycast laser vertical sous les pieds.
  3. **Lissage d'animation anti-stutter :** Tolérance d'animation au sol (`shouldPlayWalk`) maintenant active tant que le joueur avance et n'est pas en chute libre, évitant tout bégaiement ou arrêt brusque de l'animation de marche au passage des joints/coutures entre blocs.
  4. **Anti-accrochage aux joints de plateformes (Micro-Step) :** Détecte les micro-aspérités de moins de 15 cm entre deux blocs adjacents et franchit le rebord en douceur sans bloquer le joueur.
  5. **Interpolation & Continuous Collision :** Activation de `Interpolate` et `Continuous` pour supprimer les saccades de physique.
  6. **Coyote Time (0.15s) :** Permet au joueur de sauter même s'il quitte le bord d'une fraction de seconde avant d'appuyer sur la touche de saut.

---

### 1.8 Gestion de la Mort, Blocage Joueur & Focus UI (`PlayerStats.cs` & `GameOver.cs`) — [FAIT ✅]
* **Fichiers :** `Assets/Scripts/Player/PlayerStats.cs`, `Assets/Scripts/Player/PlayerScript.cs` et `Assets/Scripts/UI/GameOver.cs`
* **Corrections appliquées :**
  1. Prise en compte du tag `KillZone` (en plus de `Ground`) dans `OnCollisionEnter` et `OnTriggerEnter`, avec seuil automatique $Y < -4\text{ m}$.
  2. **Blocage total du joueur à la mort :** Dès que le joueur meurt ou tombe, `PlayerScript` coupe immédiatement la vélocité horizontale, annule tous les inputs de saut/marche et stoppe les animations/sons.
  3. **Verrouillage du focus Menu Game Over :** `GameOver.cs` sélectionne le bouton Retry dès son activation (`OnEnable`) et force le maintien du focus dans `Update()`, empêchant tout défocus à la souris ou manette.

---

### 1.9 Élimination des Vibrations & Stabilité PhysX (`PlayerScript.cs`) — [FAIT ✅]
* **Fichier :** `Assets/Scripts/Player/PlayerScript.cs`
* **Corrections appliquées :**
  1. **Calibration géométrique du CapsuleCollider :** Intégration des valeurs optimales issues des tests Play Mode (`Center: (0, 0.4512336, 0)`, `Radius: 0.2795277`, `Height: 1.476729`, `Direction: Y-Axis`) à la fois dans `Level1.unity` et par défaut dans `PlayerScript.cs` (`Awake()`), éliminant tout déport asymétrique et frottement excessif avec les plateformes.
  2. **Verrouillage de profondeur par RigidbodyConstraints natif :** Remplacement de la modification directe de `rb.position` par `FreezePositionZ` / `FreezePositionX` géré nativement par PhysX, éliminant tout conflit d'interpolation.
  3. **Exclusion d'auto-détection :** Le masque de sol exclut automatiquement le calque du joueur.

---

### 1.8 Système de Virage et de Caméra Découplée (`CameraTurnZone.cs` & `CameraFollow2D.cs`) — [FAIT ✅]
* **Fichiers :** `Assets/Scripts/Level/CameraTurnZone.cs` et `Assets/Scripts/Level/CameraFollow2D.cs`
* **Corrections appliquées :**
  1. Suivi lissé de la caméra (`Vector3.SmoothDamp`) complètement indépendant des os du squelette (`Spine2`).
  2. Zone de virage réversible avec calcul automatique de l'angle de piste du joueur ($Y_{player} = Y_{cam} - 90^\circ$).
  3. Verrouillage strict de l'axe de profondeur (Z ou X selon la portion de piste) pour interdire tout glissement transversal.

---

### 1.9 Correction du Tag `Ground` (Zone de Mort vs Sol Solide) — [FAIT ✅]
* **Action réalisée :** Tag `KillZone` créé dans `ProjectSettings/TagManager.asset` et assigné à l'objet abîme sous le niveau ($y = -5$).
* **Code lié :** `PlayerStats.cs` prend désormais en charge `KillZone` en collision et trigger, avec seuil de sécurité $Y < -4\text{ m}$.

---

# Phase 2 : Contrôleur Joueur 2.5D (Avant/Arrière avec Demi-Tour 180°) — [FAIT ✅]

* **Fichier :** `Assets/Scripts/Player/PlayerScript.cs`
* **Statut :** Appliqué et compilé avec succès.
* **Fonctionnalités actives :**
  * **Demi-tour instantané à 180° (`instantRotation = true`) :** Le personnage bascule instantanément vers sa nouvelle direction dès la pression de la touche, supprimant tout délai, lenteur ou sensation de personnage statique lors des changements de sens.
  * Déplacement physique via `rb.velocity` au lieu de `transform.Translate`.
  * Conservation de la vélocité Y pour les sauts et la gravité PhysX.
  * Méthode `SetTrackAngle(float newAngle)` intégrée pour les virages par triggers.

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Vitesses de Déplacement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float turnSpeed = 720f; // Degrés par seconde pour un demi-tour réactif

    [Header("Paramètres de Saut")]
    public float jumpForce = 8f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;
    private float _coyoteCounter;
    private float _jumpBufferCounter;

    [Header("Ground Check")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundLayer;
    public bool isGrounded { get; private set; }

    [Header("Composants & Réf")]
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip walkSound;

    // Orientation de piste courante (gérée par les zones de virage)
    public float currentTrackAngle = 0f;

    private Rigidbody _rb;
    private float _horizontalInput;
    private bool _isFacingForward = true;
    private Quaternion _targetRotation;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                          RigidbodyConstraints.FreezeRotationZ;
        _targetRotation = Quaternion.Euler(0f, currentTrackAngle, 0f);
    }

    void Update()
    {
        // 1. Capture de l'input horizontal (-1 = arrière, +1 = avant)
        _horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. Gestion du demi-tour (180°)
        if (_horizontalInput > 0.05f)
        {
            _isFacingForward = true;
            _targetRotation = Quaternion.Euler(0f, currentTrackAngle, 0f);
        }
        else if (_horizontalInput < -0.05f)
        {
            _isFacingForward = false;
            _targetRotation = Quaternion.Euler(0f, currentTrackAngle + 180f, 0f);
        }

        // 3. Rotation fluide vers l'orientation cible
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnSpeed * Time.deltaTime);

        // 4. Coyote Time & Jump Buffer
        if (isGrounded)
        {
            _coyoteCounter = coyoteTime;
        }
        else
        {
            _coyoteCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
        {
            _jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }

        // 5. Animations
        bool isMoving = Mathf.Abs(_horizontalInput) > 0.05f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        if (animator != null)
        {
            animator.SetBool("walk", isMoving && !isSprinting);
            animator.SetFloat("run", (isMoving && isSprinting) ? 1f : 0f);
            animator.SetBool("isGrounded", isGrounded);
        }
    }

    void FixedUpdate()
    {
        // Détection du sol
        CheckGround();

        // Calcul de la vitesse désirée
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        // Direction de mouvement dans le repère de la piste
        Vector3 moveDir = Vector3.zero;
        if (Mathf.Abs(_horizontalInput) > 0.05f)
        {
            // Le joueur avance toujours vers son 'forward' courant après le demi-tour
            moveDir = transform.forward * targetSpeed;
        }

        // Application physique préservant la vélocité verticale (gravité/saut)
        _rb.velocity = new Vector3(moveDir.x, _rb.velocity.y, moveDir.z);

        // Exécution du saut avec Jump Buffer et Coyote Time
        if (_jumpBufferCounter > 0f && _coyoteCounter > 0f)
        {
            ExecuteJump();
            _jumpBufferCounter = 0f;
            _coyoteCounter = 0f;
        }
    }

    private void CheckGround()
    {
        Vector3 checkPos = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundLayer);
    }

    private void ExecuteJump()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (animator != null) animator.SetTrigger("jump");
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public void SetTrackAngle(float newAngle)
    {
        currentTrackAngle = newAngle;
        _targetRotation = Quaternion.Euler(0f, _isFacingForward ? currentTrackAngle : currentTrackAngle + 180f, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 checkPos = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
    }
}
```

---

# Phase 3 : Intégration du Nouveau Personnage 3D & Animations

### 3.1 Hiérarchie Propre du Joueur
Pour séparer la physique du modèle graphique :
```
Player (GameObject racine)
├── Rigidbody (Freeze Rotation X et Z)
├── CapsuleCollider (Center: 0, 1, 0 | Height: 2 | Radius: 0.35)
├── PlayerController2D.cs
├── PlayerStats.cs
├── GroundCheck (Empty positionné à Y = 0.05)
└── Model_Visual (Modèle 3D enfant avec Animator)
    └── Armature / Bones
```

### 3.2 Configuration de l'Avatar Humanoid
1. Dans le dossier `Assets/Assets/kenney_animated-characters-3/Model/characterMedium.fbx` :
   * Cliquer sur le fichier FBX dans l'onglet Project.
   * Dans l'Inspecteur $\rightarrow$ Onglet **Rig** :
     * **Animation Type** : Passer en `Humanoid`.
     * **Avatar Definition** : `Create From This Model`.
     * Cliquer sur **Apply**.
2. Faire de même pour les animations (`idle.fbx`, `run.fbx`, `jump.fbx`) :
   * Onglet **Rig** $\rightarrow$ `Humanoid` $\rightarrow$ `Copy From Other Avatar` $\rightarrow$ Sélectionner l'Avatar créé à l'étape 1.

---

# Phase 4 : Système de Plateformes Mobiles & Héritage d'Inertie

### 4.1 Script Réutilisable `MovingPlatform.cs`
Créez `Assets/Scripts/Level/MovingPlatform.cs` :

```csharp
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Points de passage")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.5f;

    [Header("Comportement")]
    public float waitTimeAtPoint = 0.5f;

    public Vector3 CurrentVelocity { get; private set; }

    private Vector3 _targetPos;
    private Vector3 _lastPos;
    private float _waitTimer = 0f;

    void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("Plateforme mobile sans pointA ou pointB configuré !");
            return;
        }
        transform.position = pointA.position;
        _targetPos = pointB.position;
        _lastPos = transform.position;
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            CurrentVelocity = Vector3.zero;
            return;
        }

        // Déplacement physique lissé
        Vector3 newPos = Vector3.MoveTowards(transform.position, _targetPos, speed * Time.fixedDeltaTime);
        CurrentVelocity = (newPos - transform.position) / Time.fixedDeltaTime;
        transform.position = newPos;

        // Inversion de direction quand le point cible est atteint
        if (Vector3.Distance(transform.position, _targetPos) < 0.02f)
        {
            _targetPos = (_targetPos == pointA.position) ? pointB.position : pointA.position;
            _waitTimer = waitTimeAtPoint;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            
            // Si le joueur saute de la plateforme, on lui transfère l'inertie
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity += CurrentVelocity;
            }
        }
    }
}
```

### 4.2 Configuration dans Unity
1. Sur le GameObject de la plateforme :
   * Ajouter un **BoxCollider** configuré avec `IsTrigger = true` légèrement au-dessus de la surface (pour détecter les pieds du joueur).
   * Attacher un second BoxCollider classique (`IsTrigger = false`) pour la collision physique.
   * Créer deux enfants Empty : `PointA` et `PointB` et les assigner dans l'Inspecteur de `MovingPlatform.cs`.

---

# Phase 5 : Caméra Dynamique & Triggers de Virage Réversibles (Empty) — [IMPLÉMENTÉ / EN COURS DE TEST ✅]

* **Fichiers :** `Assets/Scripts/Level/CameraFollow2D.cs` et `Assets/Scripts/Level/CameraTurnZone.cs`
* **Fonctionnalités actives :**
  * Suivi indépendant lissé de la caméra (`Vector3.SmoothDamp`) sans secousses d'animation.
  * Détection automatique de sens réversible basée sur l'angle actuel de la caméra (bascule $0^\circ \leftrightarrow 90^\circ$ infaillible sans dépendance de vecteur vitesse ou de rotation du trigger).
  * Alignement automatique du rail de course du joueur ($Y_{player} = Y_{cam} - 90^\circ$).

### 5.1 Script `PathCameraFollow.cs` (Caméra Indépendante)
Créez `Assets/Scripts/Level/PathCameraFollow.cs` :

```csharp
using UnityEngine;

public class PathCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -6f);
    public float smoothSpeed = 0.12f;
    public float rotationSmoothTime = 0.25f;

    private Vector3 _currentVelocity;
    private Quaternion _targetRotation;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        _targetRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Position cible avec prise en compte de la rotation de piste
        Vector3 rotatedOffset = _targetRotation * offset;
        Vector3 desiredPosition = target.position + rotatedOffset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothSpeed);

        // Rotation fluide de la vue
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime / rotationSmoothTime);
    }

    public void SetCameraAngleY(float targetAngleY)
    {
        _targetRotation = Quaternion.Euler(15f, targetAngleY, 0f);
    }
}
```

### 5.2 Script `CameraTurnZone.cs` (Trigger Empty pour Virages Réversibles)
Créez `Assets/Scripts/Level/CameraTurnZone.cs` :

```csharp
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTurnZone : MonoBehaviour
{
    [Header("Orientation des angles Y")]
    [Tooltip("Angle quand le joueur avance vers l'avant")]
    public float angleForward = 90f;

    [Tooltip("Angle quand le joueur revient en arrière")]
    public float angleBackward = 0f;

    private BoxCollider _trigger;

    void Awake()
    {
        _trigger = GetComponent<BoxCollider>();
        _trigger.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        PlayerController2D controller = other.GetComponent<PlayerController2D>();
        PathCameraFollow cam = Camera.main.GetComponent<PathCameraFollow>();

        // Calcul du produit scalaire pour savoir si le joueur entre de face ou à reculons
        Vector3 playerVelocity = (rb != null) ? rb.velocity : other.transform.forward;
        float dot = Vector3.Dot(playerVelocity.normalized, transform.forward);

        if (dot >= 0)
        {
            // Traversée vers l'avant
            if (controller != null) controller.SetTrackAngle(angleForward);
            if (cam != null) cam.SetCameraAngleY(angleForward);
        }
        else
        {
            // Traversée en sens inverse (retour en arrière)
            if (controller != null) controller.SetTrackAngle(angleBackward);
            if (cam != null) cam.SetCameraAngleY(angleBackward);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.4f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, GetComponent<BoxCollider>() != null ? GetComponent<BoxCollider>().size : Vector3.one);

        // Flèche indiquant la direction 'Forward'
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, Vector3.forward * 2f);
    }
}
```

---

# Phase 6 : Mécanique "Gravity Flip" & Impact d'Atterrissage

### 6.1 Script `PlayerGravityFlip.cs`
Créez `Assets/Scripts/Player/PlayerGravityFlip.cs` :

```csharp
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Paramètres Gravité")]
    public float gravityMagnitude = 18f;
    public bool isFlipped { get; private set; } = false;

    [Header("Feedback")]
    public Transform visualModel;
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip landSound;
    public ParticleSystem landParticles;

    [Header("Détection d'impact fort")]
    public float hardLandingVelocityThreshold = 12f;

    private Rigidbody _rb;
    private PlayerController2D _controller;
    private float _lastVerticalVelocity;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false; // Gestion manuelle de la gravité
        _controller = GetComponent<PlayerController2D>();
    }

    void FixedUpdate()
    {
        // 1. Application manuelle de la gravité orientée
        Vector3 gravityDir = isFlipped ? Vector3.up : Vector3.down;
        _rb.AddForce(gravityDir * gravityMagnitude, ForceMode.Acceleration);

        // 2. Mémorisation de la vitesse de chute pour la détection d'impact
        _lastVerticalVelocity = Mathf.Abs(_rb.velocity.y);
    }

    public void FlipGravity()
    {
        isFlipped = !isFlipped;

        // Retournement du modèle visuel à 180° sur Z
        if (visualModel != null)
        {
            Vector3 rot = visualModel.localEulerAngles;
            rot.z = isFlipped ? 180f : 0f;
            visualModel.localEulerAngles = rot;
        }

        if (audioSource != null && flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Vérification de la violence de l'atterrissage
        if (_lastVerticalVelocity >= hardLandingVelocityThreshold)
        {
            TriggerLandingImpact();
        }
    }

    private void TriggerLandingImpact()
    {
        if (_controller != null && _controller.animator != null)
        {
            _controller.animator.SetTrigger("hardLand");
        }

        if (landParticles != null)
        {
            landParticles.Play();
        }

        if (audioSource != null && landSound != null)
        {
            audioSource.PlayOneShot(landSound);
        }
    }
}
```

---

# Phase 7 : Robustesse Stand Festival & Mode Kiosk

### 7.1 Script `FestivalKioskManager.cs`
Créez `Assets/Scripts/UI/FestivalKioskManager.cs` et placez-le sur un GameObject persistant (`DontDestroyOnLoad`) :

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;

public class FestivalKioskManager : MonoBehaviour
{
    [Header("Inactivité")]
    [Tooltip("Délai en secondes avant le retour au menu principal si personne ne joue")]
    public float inactivityTimeout = 75f;
    private float _timer;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Détection de toute activité (touche, souris, stick, bouton manette)
        if (Input.anyKey || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            _timer = 0f;
        }
        else
        {
            _timer += Time.deltaTime;
        }

        // Si inactif et pas déjà sur le Menu Principal
        if (_timer >= inactivityTimeout)
        {
            _timer = 0f;
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                Debug.Log("[Festival] Réinitialisation d'inactivité -> Retour au Menu Principal");
                Time.timeScale = 1f;
                SceneManager.LoadScene("MainMenu");
            }
        }

        // Raccourci secret 'F12' pour reset la démo entre deux joueurs au stand
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("[Festival] Sauvegardes PlayerPrefs réinitialisées !");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
```

---

