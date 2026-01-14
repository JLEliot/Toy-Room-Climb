using UnityEngine;

public class CameraOnlyUp : MonoBehaviour
{
    [Header("Suivi cible")]
    public Transform target;
    public Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f); // Ajustement pour que la caméra soit au niveau de la tête ou du torse
    public float distance = 4f;
    public float minDistance = 1.2f;
    public float maxDistance = 6f;
    public float followSmooth = 12f;

    [Header("Collision caméra")]
    public float collisionRadius = 0.3f;
    public float collisionBuffer = 0.15f;
    public LayerMask collisionMask = ~0;

    [Header("Sensibilité souris")]
    public float sensitivityX = 180f;
    public float sensitivityY = 140f;

    [Header("Limites verticales")]
    public float minPitch = -70f;
    public float maxPitch = 70f; // Suppression de la limite pour regarder complètement vers le ciel

    private float _yaw;
    private float _pitch = 10f;

    private bool _cursorLocked = true;

    void Start()
    {
        LockCursor();
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) target = player.transform;
        }

        // Synchronisation des valeurs de l'Inspector
        pivotOffset.y = Mathf.Max(pivotOffset.y, 30f); // Assure que la hauteur est correctement appliquée
        minPitch = -70f;
        maxPitch = 70f;
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
        if (!_cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch); // Permet de regarder complètement vers le ciel
    }

    void LateUpdate()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
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
            correctedDistance = Mathf.Clamp(hit.distance - collisionBuffer, minDistance, maxDistance);
        }

        Vector3 finalPosition = pivot + desiredDirection * correctedDistance;
        transform.position = Vector3.Lerp(transform.position, finalPosition, followSmooth * Time.deltaTime);
        transform.rotation = rotation;
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _cursorLocked = false;
    }
}
