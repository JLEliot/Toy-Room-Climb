using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 60f;
    public float rotationSpeed = 20f;

    [Tooltip("Accélération au sol (léger smoothing ~200ms)")]
    public float groundAcceleration = 120f;

    [Tooltip("Décélération au sol")]
    public float groundDeceleration = 160f;

    [Header("Air Control")]
    [Tooltip("Multiplicateur de vitesse en l'air")]
    public float airSpeedMultiplier = 0.65f;

    [Tooltip("Accélération en l'air")]
    public float airAcceleration = 40f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 6f;
    public float gravity = -55f;

    // ===== JumpPad / External impulse =====
    [Header("External Launch (JumpPad)")]
    [Tooltip("Smoothing de la poussée externe (plus grand = s'arrête plus vite)")]
    public float externalDamping = 8f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector3 planarVelocity;

    private Vector3 externalVelocity = Vector3.zero; // <-- impulsion du jump pad

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Appelé par JumpPad.cs
    /// Donne une impulsion vers le haut + optionnel forward
    /// </summary>
    public void ExternalLaunch(float upSpeed, float forwardSpeed, Vector3 forwardDir)
    {
        // Reset vertical pour que la poussée soit bien "franche"
        verticalVelocity.y = 0f;

        // Ajoute une impulsion (velocity change)
        externalVelocity = (Vector3.up * upSpeed) + (forwardDir.normalized * forwardSpeed);

        // Optionnel : petit trigger anim si tu veux
        if (animator) animator.SetTrigger("Jump");
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        // --- Ground stick
        if (isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        // =========================
        // INPUT (NEW INPUT SYSTEM)
        // =========================
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.zKey.isPressed)
                input.y += 1f;
            if (Keyboard.current.sKey.isPressed)
                input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.qKey.isPressed)
                input.x -= 1f;
            if (Keyboard.current.dKey.isPressed)
                input.x += 1f;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        bool jumpPressed =
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame;

        // =========================
        // CAMERA RELATIVE MOVE
        // =========================
        Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * input.y + camRight * input.x;

        // =========================
        // HORIZONTAL VELOCITY
        // =========================
        float maxSpeed = moveSpeed * (isGrounded ? 1f : airSpeedMultiplier);
        Vector3 targetPlanarVelocity = moveDir * maxSpeed;

        float accel = isGrounded
            ? (input.sqrMagnitude > 0.01f ? groundAcceleration : groundDeceleration)
            : airAcceleration;

        planarVelocity = Vector3.MoveTowards(
            planarVelocity,
            targetPlanarVelocity,
            accel * Time.deltaTime
        );

        // =========================
        // ROTATION (léger smoothing)
        // =========================
        Vector3 lookDir = planarVelocity;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                rotationSpeed * Time.deltaTime
            );
        }

        // =========================
        // JUMP
        // =========================
        if (jumpPressed && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }

        // =========================
        // GRAVITY
        // =========================
        verticalVelocity.y += gravity * Time.deltaTime;

        // =========================
        // FINAL MOVE (planar + vertical + external)
        // =========================
        Vector3 finalMove =
            planarVelocity +
            verticalVelocity +
            externalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // Damping de la poussée externe (pour pas rester boosté)
        externalVelocity = Vector3.Lerp(externalVelocity, Vector3.zero, externalDamping * Time.deltaTime);

        // =========================
        // ANIMATOR PARAMETERS
        // =========================
        if (animator)
        {
            float speed01 = Mathf.Clamp01(
                new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude / moveSpeed
            );

            animator.SetFloat("Speed", speed01);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("YVelocity", verticalVelocity.y);
        }
    }
}
