using System.Collections;
using UnityEngine;

public class TimedPlatform : MonoBehaviour
{
    [Header("Timing")]
    [Min(0f)] public float onDuration = 2f;
    [Min(0f)] public float offDuration = 2f;
    [Min(0f)] public float startOffset = 0f;

    [Header("What to toggle")]
    [Tooltip("Si vide : prend ce GameObject comme racine des visuels")]
    public Transform visualsRoot;

    [Tooltip("Colliders à activer/désactiver. Si vide : auto-detect sur l'objet et ses enfants")]
    public Collider[] collidersToToggle;

    private Renderer[] cachedRenderers;

    private void Awake()
    {
        if (visualsRoot == null)
            visualsRoot = transform;

        // Cache des renderers (visuels)
        cachedRenderers = visualsRoot.GetComponentsInChildren<Renderer>(true);

        // Auto-detect colliders si non assignés
        if (collidersToToggle == null || collidersToToggle.Length == 0)
            collidersToToggle = GetComponentsInChildren<Collider>(true);
    }

    private void Start()
    {
        StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine()
    {
        if (startOffset > 0f)
            yield return new WaitForSeconds(startOffset);

        while (true)
        {
            SetPlatformState(true);
            if (onDuration > 0f)
                yield return new WaitForSeconds(onDuration);

            SetPlatformState(false);
            if (offDuration > 0f)
                yield return new WaitForSeconds(offDuration);
        }
    }

    private void SetPlatformState(bool state)
    {
        // Visuels
        foreach (var r in cachedRenderers)
            if (r) r.enabled = state;

        // Colliders
        foreach (var c in collidersToToggle)
            if (c) c.enabled = state;
    }
}