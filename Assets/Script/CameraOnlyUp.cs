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

    void Start()
    {
        // bloque le curseur (FPS/TPS style)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // initialise avec l'orientation actuelle
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        // rotation horizontale
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        yaw += mouseX;

        // rotation verticale
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}