using System.Collections;
using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
    [Header("Réglages")]
    public float delayBeforeDisappear = 0.3f;
    public float respawnDelay = 2.0f;
    public bool respawn = true;

    [Header("Références")]
    [Tooltip("Objet visible (MeshRenderer). Si vide, prend ceux du parent.")]
    public Renderer[] renderersToHide;

    [Tooltip("Collider solide de la plateforme (pas trigger). Si vide, cherche sur le parent.")]
    public Collider solidCollider;

    private bool triggered = false;

    void Awake()
    {
        // Auto-find si pas assigné
        if (solidCollider == null)
            solidCollider = GetComponentInParent<Collider>();

        if (renderersToHide == null || renderersToHide.Length == 0)
            renderersToHide = GetComponentsInParent<Renderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);

        // Cache visuel
        foreach (var r in renderersToHide)
            r.enabled = false;

        // Désactive collision solide
        if (solidCollider != null)
            solidCollider.enabled = false;

        if (respawn)
        {
            yield return new WaitForSeconds(respawnDelay);

            // Réactive
            foreach (var r in renderersToHide)
                r.enabled = true;

            if (solidCollider != null)
                solidCollider.enabled = true;

            triggered = false;
        }
    }
}
