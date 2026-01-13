using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour
{
    [Header("Réglages du Jump Pad")]
    [Tooltip("Vitesse vers le haut donnée au joueur")]
    public float upwardSpeed = 14f;

    [Tooltip("Optionnel : poussée vers l'avant")]
    public float forwardSpeed = 0f;

    [Tooltip("Temps minimum entre 2 déclenchements")]
    public float cooldown = 0.25f;

    private float nextAllowedTime = 0f;

    private void Reset()
    {
        // Sécurité : force le collider en trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JumpPad trigger touché par : " + other.name);

        // Anti-spam
        if (Time.time < nextAllowedTime)
            return;

        // On vérifie bien que c'est le joueur
        if (!other.CompareTag("Player"))
            return;

        // On récupère le contrôleur du joueur
        PlayerControllerCC controller = other.GetComponent<PlayerControllerCC>();
        if (controller == null)
        {
            Debug.LogWarning("JumpPad : PlayerControllerCC introuvable sur " + other.name);
            return;
        }

        // Lancement du joueur
        controller.ExternalLaunch(upwardSpeed, forwardSpeed, transform.forward);

        nextAllowedTime = Time.time + cooldown;
    }
}