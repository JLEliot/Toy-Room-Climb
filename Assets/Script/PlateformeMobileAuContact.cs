using System.Collections;
using UnityEngine;

public class PlateformeMobileAuContact : MonoBehaviour
{
    [Header("Déclenchement")]
    [Tooltip("Tag du joueur qui active la plateforme.")]
    public string playerTag = "Player";

    [Header("Mouvement")]
    [Tooltip("Hauteur du déplacement vertical en mètres.")]
    public float liftHeight = 1f;

    [Tooltip("Durée (en secondes) pour monter ou descendre.")]
    public float moveDuration = 1f;

    [Tooltip("Pause au sommet avant de redescendre.")]
    public float pauseAtTop = 0.2f;

    [Tooltip("Courbe d'interpolation pour un mouvement fluide.")]
    public AnimationCurve motionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Physique")]
    [Tooltip("Rigidbody optionnel de la plateforme (Kinematic recommandé).")]
    [SerializeField]
    private Rigidbody platformRigidbody;

    private Vector3 startPosition;
    private bool isMoving;

    private void Awake()
    {
        startPosition = transform.position;

        if (platformRigidbody == null)
        {
            platformRigidbody = GetComponent<Rigidbody>();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isMoving)
        {
            return;
        }

        if (!collision.collider.CompareTag(playerTag))
        {
            return;
        }

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        isMoving = true;

        Vector3 topPosition = startPosition + Vector3.up * liftHeight;

        yield return MoveBetween(startPosition, topPosition);

        if (pauseAtTop > 0f)
        {
            yield return new WaitForSeconds(pauseAtTop);
        }

        yield return MoveBetween(topPosition, startPosition);

        isMoving = false;
    }

    private IEnumerator MoveBetween(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float normalized = Mathf.Clamp01(elapsed / moveDuration);
            float eased = motionCurve.Evaluate(normalized);
            Vector3 newPosition = Vector3.LerpUnclamped(from, to, eased);

            if (platformRigidbody != null && platformRigidbody.isKinematic)
            {
                platformRigidbody.MovePosition(newPosition);
            }
            else
            {
                transform.position = newPosition;
            }

            yield return null;
        }

        if (platformRigidbody != null && platformRigidbody.isKinematic)
        {
            platformRigidbody.MovePosition(to);
        }
        else
        {
            transform.position = to;
        }
    }
}
