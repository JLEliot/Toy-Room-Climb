using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 60f;
    public float rotationSpeed = 20f;

    [Tooltip("Accélération au sol (plus haut = plus réactif)")]
    public float groundAcceleration = 25f;

    [Tooltip("Freinage au sol (plus haut = stop plus rapide)")]
    public float groundDeceleration = 30f;

    [Tooltip("Accélération en l’air (plus bas = moins de contrôle)")]
    public float airAcceleration = 6f;

    [Tooltip("Vitesse max en l’air (0.4 = 40% de la vitesse sol)")]
    public float airSpeedMultiplier = 0.45f;

    [Header("Turn Smoothing")]
    [Tooltip("Empêche les demi-tours instantanés. 0 = off")]
    public float turnResponsiveness = 10f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 6f;
    public float gravity = -55f;

    private CharacterController controller;
    private Vector3 velocity;               // vitesse verticale
    private Vector3 planarVelocity;         // vitesse horizontale actuelle

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;

        // --- Ground stick
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // --- Input (ancien Input, marche si Active Input Handling = Both)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        // --- Convertir input selon caméra
        Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = camForward * input.z + camRight * input.x;

        // --- Vitesse cible (sol vs air)
        float maxSpeed = moveSpeed * (isGrounded ? 1f : airSpeedMultiplier);
        Vector3 targetPlanarVel = moveDir * maxSpeed;

        // --- Accélération / Décélération (sol) + contrôle réduit (air)
        float accel = isGrounded ? groundAcceleration : airAcceleration;

        // si pas d’input au sol -> on freine plus fort (décélération)
        if (isGrounded && input.sqrMagnitude < 0.001f)
            accel = groundDeceleration;

        // interpolation vers la vitesse cible (fluidité)
        planarVelocity = Vector3.MoveTowards(planarVelocity, targetPlanarVel, accel * Time.deltaTime);

        // --- Applique déplacement horizontal
        controller.Move(planarVelocity * Time.deltaTime);

        // --- Rotation du perso vers la direction de déplacement (avec un peu de latence)
        Vector3 lookDir = planarVelocity;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);

            float turn = (turnResponsiveness <= 0f) ? (rotationSpeed) : (turnResponsiveness);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turn * Time.deltaTime);
        }

        // --- Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }

        // --- Gravity + vertical move
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Animator params
        if (animator)
        {
            float speed01 = Mathf.Clamp01(new Vector3(planarVelocity.x, 0f, planarVelocity.z).magnitude / Mathf.Max(1f, moveSpeed));
            animator.SetFloat("Speed", speed01);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("YVelocity", velocity.y);
        }
    }
}
