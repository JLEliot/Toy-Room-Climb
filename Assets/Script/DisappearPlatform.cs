using System.Collections;
using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Temps avant que la plateforme disparaisse")]
    public float delayBeforeDisappear = 0.15f;

    [Tooltip("Temps avant réapparition")]
    public float respawnAfter = 5f; // ⬅️ 5 secondes

    [Header("What to disable")]
    public GameObject visualsRoot;          
    public Collider solidColliderToDisable; 
    public Collider triggerCollider;        

    private bool isRunning;

    void Awake()
    {
        if (visualsRoot == null)
            visualsRoot = gameObject;

        // Auto-détection des colliders
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

        // Disparition
        visualsRoot.SetActive(false);
        if (solidColliderToDisable) solidColliderToDisable.enabled = false;
        if (triggerCollider) triggerCollider.enabled = false;

        // Attente avant respawn
        yield return new WaitForSeconds(respawnAfter);

        // Réapparition
        visualsRoot.SetActive(true);
        if (solidColliderToDisable) solidColliderToDisable.enabled = true;
        if (triggerCollider) triggerCollider.enabled = true;

        isRunning = false;
    }
}