using UnityEngine;

public class CameraOnlyUp : MonoBehaviour
{
    [Header("Suivi cible")]
    public Transform target;
    public Vector3 pivotOffset = new Vector3(0f, 1.6f, 0f);
    public float distance = 4f;
    public float minDistance = 1.2f;
    public float maxDistance = 6f;
    public float followSmooth = 12f;

    [Header("Collision caméra")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionMask = ~0;

    [Header("Sensibilité souris")]
    public float sensitivityX = 180f;
    public float sensitivityY = 140f;

    [Header("Limites verticales")]
    public float minPitch = -80f;
    public float maxPitch = 85f;

    private float yaw;
    private float pitch = 10f;

    private bool cursorLocked = true;

    void Start()
    {
        LockCursor();
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) target = player.transform;
        }
    }

    void Update()
    {
        // --- Échap : libère la souris
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        // --- Clic gauche : rebloque la souris
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }

        // --- Si la souris est libérée, on ne tourne pas la caméra
        if (!cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + pivotOffset;

        float desiredDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 desiredDirection = rotation * Vector3.back;
        float correctedDistance = desiredDistance;
        if (Physics.SphereCast(
            pivot,
            collisionRadius,
            desiredDirection,
            out RaycastHit hit,
            desiredDistance,
            collisionMask,
            QueryTriggerInteraction.Ignore
        ))
        {
            correctedDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        Vector3 finalPosition = pivot + desiredDirection * correctedDistance;
        transform.position = Vector3.Lerp(transform.position, finalPosition, followSmooth * Time.deltaTime);
        transform.rotation = rotation;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
}
