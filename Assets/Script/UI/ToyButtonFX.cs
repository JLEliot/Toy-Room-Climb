using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UI
{
    /// <summary>
    /// Ajoute un feedback visuel (scale/rotation) et audio aux boutons UI.
    /// </summary>
    public class ToyButtonFX : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Feel 'toy/cartoon'")]
        [Tooltip("Scale appliquée au survol.")]
        [SerializeField] private float hoverScale = 1.08f;
        [Tooltip("Scale appliquée lors du press.")]
        [SerializeField] private float pressScale = 0.95f;
        [Tooltip("Vitesse d'interpolation des transitions.")]
        [SerializeField] private float speed = 16f;
        [Tooltip("Amplitude de la rotation oscillante (en degrés).")]
        [SerializeField] private float wobbleDegrees = 3.0f;
        [Tooltip("Vitesse de l'oscillation.")]
        [SerializeField] private float wobbleSpeed = 14f;

        [Header("Audio (optionnel)")]
        [Tooltip("Source audio utilisée pour jouer les sons.")]
        [SerializeField] private AudioSource audioSource;
        [Tooltip("Clip joué au survol.")]
        [SerializeField] private AudioClip hoverClip;
        [Tooltip("Clip joué au clic.")]
        [SerializeField] private AudioClip clickClip;
        [Tooltip("Volume du son de survol.")]
        [SerializeField] private float hoverVolume = 0.7f;
        [Tooltip("Volume du son de clic.")]
        [SerializeField] private float clickVolume = 0.9f;

        private Vector3 _baseScale;
        private Quaternion _baseRot;
        private bool _hover;
        private bool _press;

        private void Awake()
        {
            _baseScale = transform.localScale;
            _baseRot = transform.localRotation;
        }

        private void Update()
        {
            float targetMul = _press ? pressScale : (_hover ? hoverScale : 1f);

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _baseScale * targetMul,
                1f - Mathf.Exp(-speed * Time.unscaledDeltaTime)
            );

            if (_hover && !_press)
            {
                float z = Mathf.Sin(Time.unscaledTime * wobbleSpeed) * wobbleDegrees;
                Quaternion targetRot = _baseRot * Quaternion.Euler(0f, 0f, z);

                transform.localRotation = Quaternion.Lerp(
                    transform.localRotation,
                    targetRot,
                    1f - Mathf.Exp(-speed * Time.unscaledDeltaTime)
                );
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(
                    transform.localRotation,
                    _baseRot,
                    1f - Mathf.Exp(-speed * Time.unscaledDeltaTime)
                );
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _hover = true;
            if (audioSource && hoverClip) audioSource.PlayOneShot(hoverClip, hoverVolume);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hover = false;
            _press = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _press = true;
            if (audioSource && clickClip) audioSource.PlayOneShot(clickClip, clickVolume);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _press = false;
        }
    }
}
