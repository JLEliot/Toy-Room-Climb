using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.UI
{
    public class ToyButtonFX : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Feel 'toy/cartoon'")]
        [SerializeField] private float hoverScale = 1.08f;
        [SerializeField] private float pressScale = 0.95f;
        [SerializeField] private float speed = 16f;
        [SerializeField] private float wobbleDegrees = 3.0f;
        [SerializeField] private float wobbleSpeed = 14f;

        [Header("Audio (optionnel)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private float hoverVolume = 0.7f;
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
