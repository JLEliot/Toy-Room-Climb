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

    [Header("Ledge Detection (Omni + Filtré)")]
    public LayerMask climbLayer;
    public float detectionRadius = 1.5f; 
    public float bodyRayHeight = 1.0f; 
    public float headRayHeight = 2.5f; 
    
    [Tooltip("Hauteur minimum du rebord par rapport aux pieds. 0.5 = genoux.")]
    public float minLedgeHeight = 0.5f; 

    public float climbSpeed = 5f; 

    [Header("Animation Tuning")]
    public float rootMotionVerticalBoost = 1.0f; 
    public float rootMotionForwardBoost = 1.0f; 

    [Header("Cooldown")]
    public float climbCooldown = 3.0f; 
    private float finishClimbTime = -999f; 

    // --- CODE JUMP PAD ---
    [Header("External Launch (JumpPad)")]
    public float externalDamping = 8f;
    private Vector3 externalVelocity = Vector3.zero; 
    // ---------------------

    private bool canGrabLedge = false;
    private bool isClimbing = false;
    private bool isMantling = false;   

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 planarVelocity;
    
    private RaycastHit omniHitInfo; 

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    public void ExternalLaunch(float upSpeed, float forwardSpeed, Vector3 forwardDir)
    {
        verticalVelocity = Vector3.zero; 
        externalVelocity = (Vector3.up * upSpeed) + (forwardDir.normalized * forwardSpeed);
        if (animator) animator.SetTrigger("Jump"); 
    }

    void Update()
    {
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, externalDamping * Time.deltaTime);

        if (isMantling) return; 

        bool isGrounded = controller.isGrounded;
        Vector2 input = GetInput();
        
        // INPUTS
        bool jumpPressedThisFrame = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool jumpIsHeld = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;

        // DETECTION
        DetectLedgeAround();

        // LOGIQUE
        bool isCooldownOver = Time.time >= finishClimbTime + climbCooldown;

        // Condition : On grimpe SI on détecte un mur ET qu'on MAINTIENT Espace
        if (!isClimbing && !isGrounded && canGrabLedge && isCooldownOver && jumpIsHeld)
        {
            StartClimbing();
        }

        if (isClimbing)
        {
            StartCoroutine(MantleRoutine());
        }
        
        if (isClimbing && isGrounded) isClimbing = false;

        if (!isClimbing) HandleWalking(input, isGrounded, jumpPressedThisFrame);

        UpdateAnimator(isGrounded);
    }

    void DetectLedgeAround()
    {
        canGrabLedge = false;
        float scaleY = transform.localScale.y;
        Vector3 detectionCenter = transform.position + Vector3.up * (bodyRayHeight * scaleY);

        Collider[] hitColliders = Physics.OverlapSphere(detectionCenter, detectionRadius, climbLayer);
        
        foreach (Collider wall in hitColliders)
        {
            Vector3 closestPoint = wall.ClosestPoint(detectionCenter);
            
            // FILTRE DE HAUTEUR
            if (closestPoint.y < transform.position.y + minLedgeHeight) 
            {
                continue; 
            }

            Vector3 directionToWall = (closestPoint - detectionCenter).normalized;
            Vector3 headCheckOrigin = transform.position + Vector3.up * (headRayHeight * scaleY);
            
            bool hitHead = Physics.Raycast(headCheckOrigin, directionToWall, detectionRadius + 0.5f, climbLayer);

            if (!hitHead)
            {
                canGrabLedge = true;
                omniHitInfo = new RaycastHit();
                omniHitInfo.point = closestPoint;
                omniHitInfo.normal = -directionToWall;
                omniHitInfo.distance = Vector3.Distance(detectionCenter, closestPoint);
                break; 
            }
        }
    }

    // --- DEBUG CORRIGÉ ---
    void OnDrawGizmos()
    {
        if (transform == null) return;

        float scaleY = transform.localScale.y;
        Vector3 detectionCenter = transform.position + Vector3.up * (bodyRayHeight * scaleY);

        Gizmos.color = new Color(1, 0.92f, 0.016f, 0.3f); 
        Gizmos.DrawSphere(detectionCenter, detectionRadius);

        // ICI J'AI REMPLACÉ DrawWireDisc PAR DrawWireCube (Compatible partout)
        Gizmos.color = Color.blue;
        Vector3 floorLimit = transform.position + Vector3.up * minLedgeHeight;
        Gizmos.DrawWireCube(floorLimit, new Vector3(2, 0.05f, 2));

        if (canGrabLedge)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(detectionCenter, omniHitInfo.point);
            Gizmos.DrawSphere(omniHitInfo.point, 0.1f);
        }
    }

    void OnAnimatorMove()
    {
        if (isMantling && animator)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y *= rootMotionVerticalBoost;
            Vector3 forwardMove = transform.forward * velocity.magnitude * rootMotionForwardBoost;
            velocity.x = forwardMove.x;
            velocity.z = forwardMove.z;
            controller.Move(velocity);
            transform.rotation *= animator.deltaRotation;
        }
    }

    IEnumerator MantleRoutine()
    {
        isMantling = true; 
        isClimbing = false; 
        
        planarVelocity = Vector3.zero;
        verticalVelocity = Vector3.zero;
        externalVelocity = Vector3.zero; 

        if (omniHitInfo.normal != Vector3.zero)
        {
            Vector3 lookDirection = -omniHitInfo.normal;
            lookDirection.y = 0; 
            if(lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
        
        if(animator) animator.SetTrigger("Mantle");

        yield return new WaitForSeconds(1.5f); 

        controller.Move(Vector3.down * 5.0f);

        finishClimbTime = Time.time;
        isMantling = false; 
        verticalVelocity = Vector3.zero; 
    }

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

    void HandleClimbing(Vector2 input, bool jumpPressed, RaycastHit wallHit) { }

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
        
        Vector3 finalVelocity = planarVelocity + verticalVelocity + externalVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
        
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
    }

    void UpdateAnimator(bool isGrounded) {
        if (!animator) return;
        float speed01 = Mathf.Clamp01(new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude / moveSpeed);
        animator.SetFloat("Speed", speed01);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("YVelocity", verticalVelocity.y);
    }
}