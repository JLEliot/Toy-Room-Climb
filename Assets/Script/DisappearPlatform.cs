using System.Collections;
using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeDisappear = 0.15f;
    public float respawnAfter = 2.0f; // ⬅ Réapparition après 3 secondes

    [Header("What to disable")]
    [Tooltip("Si vide : on prend tous les Renderers du GameObject et de ses enfants")]
    public Transform visualsRoot;

    [Tooltip("Collider solide (IsTrigger OFF). Si vide -> auto")]
    public Collider solidColliderToDisable;

    [Tooltip("Collider trigger (IsTrigger ON). Si vide -> auto")]
    public Collider triggerCollider;

    private Renderer[] cachedRenderers;
    private bool isRunning;

    void Awake()
    {
        if (visualsRoot == null)
            visualsRoot = transform;

        // Cache tous les renderers (visuels)
        cachedRenderers = visualsRoot.GetComponentsInChildren<Renderer>(true);

        // Auto-detection des colliders si non assignés
        if (solidColliderToDisable == null || triggerCollider == null)
        {
            Collider[] cols = GetComponents<Collider>();
            foreach (var c in cols)
            {
                if (c.isTrigger)
                {
                    if (triggerCollider == null)
                        triggerCollider = c;
                }
                else
                {
                    if (solidColliderToDisable == null)
                        solidColliderToDisable = c;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isRunning) return;
        if (!other.CompareTag("Player")) return;

        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        isRunning = true;

        // Petit délai avant disparition
        yield return new WaitForSeconds(delayBeforeDisappear);

        // Cacher les visuels
        foreach (var r in cachedRenderers)
            if (r) r.enabled = false;

        // Désactiver les colliders
        if (solidColliderToDisable) solidColliderToDisable.enabled = false;
        if (triggerCollider) triggerCollider.enabled = false;

        // Temps avant réapparition
        yield return new WaitForSeconds(respawnAfter);

        // Réactiver visuels et colliders
        foreach (var r in cachedRenderers)
            if (r) r.enabled = true;

        if (solidColliderToDisable) solidColliderToDisable.enabled = true;
        if (triggerCollider) triggerCollider.enabled = true;

        isRunning = false;
    }
}
