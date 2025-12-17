using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;      // ta caméra (Main Camera)
    public Animator animator;              // l'Animator sur Woody

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 12f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 3f;
    public float gravity = -40f;

    private CharacterController controller;
    private Vector3 velocity;             // vitesse verticale
    private Vector3 lastPlanarMove;        // mouvement horizontal (pour Speed)

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!controller) return;

        // --- 1) Ground check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // colle au sol

        // --- 2) Inputs (WASD / ZQSD marche aussi selon ton mapping)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(x, 0f, z);
        input = Vector3.ClampMagnitude(input, 1f);

        // --- 3) Convertir input vers direction caméra (TPS)
        Vector3 camForward = cameraTransform ? cameraTransform.forward : Vector3.forward;
        Vector3 camRight   = cameraTransform ? cameraTransform.right   : Vector3.right;
        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 moveDir = camForward * input.z + camRight * input.x;

        // --- 4) Déplacement horizontal
        Vector3 planarMove = moveDir * moveSpeed;
        controller.Move(planarMove * Time.deltaTime);
        lastPlanarMove = planarMove;

        // --- 5) Rotation du perso vers la direction de déplacement
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // --- 6) Saut
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Formule jump: v = sqrt(h * -2g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator)
                animator.SetTrigger("Jump");
        }

        // --- 7) Gravité + mouvement vertical
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- 8) Paramètres Animator
        if (animator)
        {
            float speed01 = Mathf.Clamp01(new Vector3(lastPlanarMove.x, 0f, lastPlanarMove.z).magnitude / moveSpeed);
            animator.SetFloat("Speed", speed01);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("YVelocity", velocity.y);
        }
    }
}
