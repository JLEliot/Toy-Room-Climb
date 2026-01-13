using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 60f;
    public float rotationSpeed = 20f;
    public float groundAcceleration = 120f;
    public float groundDeceleration = 160f;
    public float airSpeedMultiplier = 0.65f;
    public float airAcceleration = 40f;
    public float jumpHeight = 6f;
    public float gravity = -55f;

    [Header("Ledge Detection")]
    public LayerMask climbLayer;
    public float raycastRange = 10f; 
    public float bodyRayHeight = 1.0f; 
    public float headRayHeight = 2.5f; 

    public float climbSpeed = 5f; 

    [Header("Animation Tuning")]
    public float rootMotionVerticalBoost = 1.0f; // Remis à 1.0 comme la v10 (augmentez si besoin)
    public float rootMotionForwardBoost = 1.0f; 

    // --- NOUVEAU : LE COOLDOWN ---
    [Header("Cooldown")]
    public float climbCooldown = 3.0f; // 3 secondes d'attente
    private float finishClimbTime = -999f; // Permet de grimper dès le début

    private bool hitBody = false;
    private bool hitHead = false;
    private bool canGrabLedge = false;
    private bool isClimbing = false;
    private bool isMantling = false; 

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 planarVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // 1. BLOCAGE DES COMMANDES
        // Si l'animation de montée (Mantle) est en cours, on bloque TOUT.
        // Le joueur ne peut plus bouger, le script s'arrête ici pour cette frame.
        if (isMantling) return; 

        bool isGrounded = controller.isGrounded;
        Vector2 input = GetInput();
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

        // --- DETECTION ---
        float scaleY = transform.localScale.y;
        Vector3 bodyRayOrigin = transform.position + Vector3.up * (bodyRayHeight * scaleY);
        Vector3 headRayOrigin = transform.position + Vector3.up * (headRayHeight * scaleY);

        hitBody = Physics.Raycast(bodyRayOrigin, transform.forward, out RaycastHit hitInfo, raycastRange, climbLayer);
        hitHead = Physics.Raycast(headRayOrigin, transform.forward, raycastRange, climbLayer);

        canGrabLedge = hitBody && !hitHead;

        // --- LOGIQUE ESCALADE AVEC COOLDOWN ---
        // On vérifie si 3 secondes se sont écoulées depuis la dernière escalade
        bool isCooldownOver = Time.time >= finishClimbTime + climbCooldown;

        if (!isClimbing && !isGrounded && canGrabLedge && input.y > 0 && isCooldownOver)
        {
            StartClimbing();
        }

        if (isClimbing && !hitHead && input.y > 0)
        {
            StartCoroutine(MantleRoutine());
        }
        
        if (isClimbing && isGrounded) isClimbing = false;

        if (isClimbing) HandleClimbing(input, jumpPressed, hitInfo);
        else HandleWalking(input, isGrounded, jumpPressed);

        UpdateAnimator(isGrounded);
    }

    // --- ROOT MOTION (Reste identique à la v10) ---
    void OnAnimatorMove()
    {
        if (isMantling && animator)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y *= rootMotionVerticalBoost;
            
            // On applique le boost avant
            Vector3 forwardMove = transform.forward * velocity.magnitude * rootMotionForwardBoost;
            velocity.x = forwardMove.x;
            velocity.z = forwardMove.z;

            controller.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }

    IEnumerator MantleRoutine()
    {
        isMantling = true; // C'est cette variable qui bloque les commandes dans Update()
        isClimbing = false; 
        
        // Stop net du personnage
        planarVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;
        
        if(animator) animator.SetTrigger("Mantle");

        // Durée de l'animation (ajustable)
        yield return new WaitForSeconds(1.5f); 

        // --- CORRECTION DU BUG FLOTTANT ---
        // 1. Petite poussette vers l'avant pour être sûr d'être sur le plateau
        controller.Move(transform.forward * 0.5f);
        
        // 2. GRAVITÉ FORCÉE : On plaque le cylindre au sol immédiatement
        // Cela corrige le bug où le cylindre reste en l'air
        controller.Move(Vector3.down * 5.0f);

        // --- FIN DU COOLDOWN ---
        // On note l'heure actuelle pour démarrer le compteur de 3 secondes
        finishClimbTime = Time.time;

        isMantling = false; // On rend les commandes au joueur
        verticalVelocity = Vector3.zero; 
    }

    // --- FONCTIONS CLASSIQUES (INCHANGÉES) ---
    Vector2 GetInput() {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null) {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.zKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.qKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
        }
        return Vector2.ClampMagnitude(input, 1f);
    }

    void StartClimbing() { isClimbing = true; verticalVelocity = Vector3.zero; planarVelocity = Vector3.zero; }

    void HandleClimbing(Vector2 input, bool jumpPressed, RaycastHit wallHit) {
        if (wallHit.collider != null) {
            Vector3 directionToWall = (wallHit.point - transform.position).normalized;
            controller.Move(directionToWall * 5f * Time.deltaTime); 
            Vector3 lookAtWall = -wallHit.normal; lookAtWall.y = 0;
            if(lookAtWall != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookAtWall);
        }
        float verticalMove = Mathf.Max(0, input.y) * (climbSpeed * transform.localScale.y); 
        verticalVelocity = Vector3.up * verticalMove;
        controller.Move(verticalVelocity * Time.deltaTime);

        if (jumpPressed) { 
            isClimbing = false;
            verticalVelocity = Vector3.up * 10f; 
            planarVelocity = -transform.forward * 10f; 
        }
    }

    void HandleWalking(Vector2 input, bool isGrounded, bool jumpPressed) {
        if (isGrounded && verticalVelocity.y < 0f) verticalVelocity.y = -2f;
        Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
        camForward.y = 0f; camRight.y = 0f; camForward.Normalize(); camRight.Normalize();
        Vector3 moveDir = camForward * input.y + camRight * input.x;
        float maxSpeed = moveSpeed * (isGrounded ? 1f : airSpeedMultiplier);
        Vector3 targetPlanarVelocity = moveDir * maxSpeed;
        float accel = isGrounded ? (input.sqrMagnitude > 0.01f ? groundAcceleration : groundDeceleration) : airAcceleration;
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVelocity, accel * Time.deltaTime);
        controller.Move(planarVelocity * Time.deltaTime);
        Vector3 lookDir = planarVelocity; lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.05f) {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        if (jumpPressed && isGrounded) {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    void UpdateAnimator(bool isGrounded) {
        if (!animator) return;
        float speed01 = Mathf.Clamp01(new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude / moveSpeed);
        animator.SetFloat("Speed", speed01);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", verticalVelocity.y);
    }
}