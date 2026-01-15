using UnityEngine;

/// <summary>
/// Caméra à la troisième personne qui suit une cible avec rotation souris,
/// limite de pitch et correction de collision.
/// </summary>
public class CameraOnlyUp : MonoBehaviour
{
    [Header("Suivi cible")]
    [Tooltip("Cible à suivre (ex. Player). Si vide, tente de trouver le tag Player au Start.")]
    public Transform target;
    [Tooltip("Décalage du pivot caméra par rapport à la cible (tête/torse).")]
    public Vector3 pivotOffset = new Vector3(0f, 1.5f, 0f);
    [Tooltip("Distance souhaitée entre la caméra et la cible.")]
    public float distance = 4f;
    [Tooltip("Distance minimale autorisée pour la collision.")]
    public float minDistance = 1.2f;
    [Tooltip("Distance maximale autorisée pour la collision.")]
    public float maxDistance = 6f;
    [Tooltip("Vitesse d'interpolation de la caméra vers la position finale.")]
    public float followSmooth = 12f;

    [Header("Collision caméra")]
    [Tooltip("Rayon du SphereCast pour éviter les murs.")]
    public float collisionRadius = 0.3f;
    [Tooltip("Marge entre la caméra et l'obstacle.")]
    public float collisionBuffer = 0.15f;
    [Tooltip("Masque de collision utilisé pour la caméra.")]
    public LayerMask collisionMask = ~0;

    [Header("Sensibilité souris")]
    [Tooltip("Sensibilité horizontale de la caméra.")]
    public float sensitivityX = 180f;
    [Tooltip("Sensibilité verticale de la caméra.")]
    public float sensitivityY = 140f;

    [Header("Limites verticales")]
    [Tooltip("Limite basse du pitch (regarder vers le bas).")]
    public float minPitch = -70f;
    [Tooltip("Limite haute du pitch (regarder vers le haut).")]
    public float maxPitch = 70f;

    private float _yaw;
    private float _pitch = 10f;

    private bool _cursorLocked = false;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _yaw = angles.y;
        _pitch = angles.x;

        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) target = player.transform;
        }

        // Synchronisation des valeurs de l'Inspector
        pivotOffset.y = Mathf.Max(pivotOffset.y, 30f); // Assure une hauteur minimale cohérente.
        minPitch = -70f;
        maxPitch = 70f;
    }

    private void Update()
    {
        // Vérifier si la souris est verrouillée pour tourner la caméra
        _cursorLocked = Cursor.lockState == CursorLockMode.Locked;
        
        // Si la souris est libérée, on ne tourne pas la caméra
        if (!_cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    private void LateUpdate()
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
}
