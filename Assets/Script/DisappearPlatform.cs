using System.Collections;
using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeDisappear = 2f;
    public float respawnAfter = 6f; // <- 5 secondes

    [Header("What to disable")]
    [Tooltip("Si vide: on prend tous les Renderers du GameObject et de ses enfants")]
    public Transform visualsRoot; // on ne désactive PAS le GameObject, juste ses renderers

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

        // Cache tous les renderers (visuels) du root
        cachedRenderers = visualsRoot.GetComponentsInChildren<Renderer>(true);

        // Auto-find colliders if not assigned
        if (solidColliderToDisable == null || triggerCollider == null)
        {
            Collider[] cols = GetComponents<Collider>();
            foreach (var c in cols)
            {
                if (c.isTrigger)
                {
                    if (triggerCollider == null) triggerCollider = c;
                }
                else
                {
                    if (solidColliderToDisable == null) solidColliderToDisable = c;
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

        yield return new WaitForSeconds(delayBeforeDisappear);

        // Cacher visuel (sans désactiver le GameObject)
        foreach (var r in cachedRenderers)
            if (r) r.enabled = false;

        // Désactiver collider solide
        if (solidColliderToDisable) solidColliderToDisable.enabled = false;

        // Optionnel : désactiver trigger pour éviter spam
        if (triggerCollider) triggerCollider.enabled = false;

        yield return new WaitForSeconds(respawnAfter);

        // Réactiver
        foreach (var r in cachedRenderers)
            if (r) r.enabled = true;

        if (solidColliderToDisable) solidColliderToDisable.enabled = true;
        if (triggerCollider) triggerCollider.enabled = true;

        isRunning = false;
    }
}
