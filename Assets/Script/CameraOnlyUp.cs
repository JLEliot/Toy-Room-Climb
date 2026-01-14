using UnityEngine;

public class CameraOnlyUp : MonoBehaviour
{
    [Header("Sensibilité souris")]
    public float sensitivityX = 180f;
    public float sensitivityY = 140f;

    [Header("Limites verticales")]
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch = 10f;

     [Header("Curseur")]
        [Tooltip("Verrouille la souris au démarrage (utile en gameplay).")]
        [SerializeField] private bool lockCursorOnStart = false;
    
        [Tooltip("Autorise Échap/clic gauche pour verrouiller/déverrouiller la souris.")]
        [SerializeField] private bool allowCursorToggle = false;
    
        private bool cursorLocked;

    void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
        
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        if (allowCursorToggle)
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
        }

        // --- Si la souris est libérée en mode verrouillé, on ne tourne pas la caméra
        if (lockCursorOnStart && !cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
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