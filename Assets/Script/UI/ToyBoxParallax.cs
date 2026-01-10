using UnityEngine;

namespace Script.UI
{
    public class ToyBoxParallax : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float amplitude = 10f;
        [SerializeField] private float speed = 0.8f;

        private Vector2 _start;

        private void Awake()
        {
            if (!target) target = transform as RectTransform;
            _start = target.anchoredPosition;
        }

        private void Update()
        {
            float t = Time.unscaledTime * speed;
            target.anchoredPosition =
                _start + new Vector2(Mathf.Sin(t) * amplitude, Mathf.Cos(t * 1.2f) * amplitude * 0.6f);
        }
    }
}