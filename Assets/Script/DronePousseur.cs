using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DronePousseur : MonoBehaviour
{
    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    public float arrivalThreshold = 0.1f;

    [Header("Push")]
    public float forcePush = 5f;

    private Rigidbody rb;
    private bool goingToB = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void FixedUpdate()
    {
        if (pointA == null || pointB == null)
        {
            return;
        }

        Vector3 target = goingToB ? pointB.position : pointA.position;
        Vector3 nextPosition = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(nextPosition);

        if (Vector3.Distance(rb.position, target) <= arrivalThreshold)
        {
            goingToB = !goingToB;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRigidbody == null)
        {
            return;
        }

        Vector3 pushDirection = transform.forward;
        playerRigidbody.AddForce(pushDirection * forcePush, ForceMode.Impulse);
    }
}
