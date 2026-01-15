using UnityEngine;

namespace Script.UI
{
    /// <summary>
    /// Anime l'apparition d'un container UI (pop + settle + fade).
    /// </summary>
    public class ToyBoxIntro : MonoBehaviour
    {
        [Header("Références")]
        [Tooltip("Racine du bloc UI à animer.")]
        [SerializeField] private RectTransform toyBoxRoot;
        [Tooltip("CanvasGroup de l'inner box (optionnel).")]
        [SerializeField] private CanvasGroup innerGroup;   // BoxInner (optionnel)
        [Tooltip("CanvasGroup du label (optionnel).")]
        [SerializeField] private CanvasGroup labelGroup;   // LabelTag (optionnel)
        [Tooltip("CanvasGroup du groupe de boutons (optionnel).")]
        [SerializeField] private CanvasGroup buttonsGroup; // ButtonsGroup (optionnel)

        [Header("Timing")]
        [Tooltip("Délai avant le démarrage de l'intro.")]
        [SerializeField] private float startDelay = 0.05f;
        [Tooltip("Durée du pop (scale initial -> overshoot).")]
        [SerializeField] private float popDuration = 0.35f;
        [Tooltip("Durée du settle (overshoot -> scale final).")]
        [SerializeField] private float settleDuration = 0.18f;

        [Header("Scale")]
        [Tooltip("Scale initiale avant le pop.")]
        [SerializeField] private float startScale = 0.85f;
        [Tooltip("Scale d'overshoot (rebond).")]
        [SerializeField] private float overshootScale = 1.06f;
        [Tooltip("Scale finale.")]
        [SerializeField] private float endScale = 1.0f;

        [Header("Fade (optionnel)")]
        [Tooltip("Délai avant le fade des éléments internes.")]
        [SerializeField] private float fadeDelay = 0.12f;
        [Tooltip("Durée du fade-in.")]
        [SerializeField] private float fadeDuration = 0.25f;

        private void Reset()
        {
            toyBoxRoot = transform as RectTransform;
        }

        private void Awake()
        {
            if (!toyBoxRoot) toyBoxRoot = transform as RectTransform;

            // état initial
            toyBoxRoot.localScale = Vector3.one * startScale;

            if (innerGroup) innerGroup.alpha = 0f;
            if (labelGroup) labelGroup.alpha = 0f;
            if (buttonsGroup) buttonsGroup.alpha = 0f;
        }

        private void Start()
        {
            StartCoroutine(PlayIntro());
        }

        /// <summary>
        /// Joue l'animation d'intro complète (pop + settle + fade).
        /// </summary>
        private System.Collections.IEnumerator PlayIntro()
        {
            // petite pause
            if (startDelay > 0f)
                yield return new WaitForSecondsRealtime(startDelay);

            // POP : start -> overshoot
            yield return ScaleTo(toyBoxRoot, startScale, overshootScale, popDuration);

            // settle : overshoot -> end
            yield return ScaleTo(toyBoxRoot, overshootScale, endScale, settleDuration);

            // Fade des éléments (optionnel)
            if (fadeDelay > 0f)
                yield return new WaitForSecondsRealtime(fadeDelay);

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float a = Mathf.Clamp01(t / fadeDuration);

                if (innerGroup) innerGroup.alpha = a;
                if (labelGroup) labelGroup.alpha = a;
                if (buttonsGroup) buttonsGroup.alpha = a;

                yield return null;
            }

            if (innerGroup) innerGroup.alpha = 1f;
            if (labelGroup) labelGroup.alpha = 1f;
            if (buttonsGroup) buttonsGroup.alpha = 1f;
        }

        /// <summary>
        /// Interpole l'échelle d'un RectTransform sur une durée donnée.
        /// </summary>
        private static System.Collections.IEnumerator ScaleTo(RectTransform rt, float from, float to, float duration)
        {
            duration = Mathf.Max(0.0001f, duration);
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float x = Mathf.Clamp01(t / duration);

                // courbe smooth
                float s = SmoothStep01(x);
                rt.localScale = Vector3.one * Mathf.Lerp(from, to, s);

                yield return null;
            }

            rt.localScale = Vector3.one * to;
        }

        private static float SmoothStep01(float x) => x * x * (3f - 2f * x);
    }
}
