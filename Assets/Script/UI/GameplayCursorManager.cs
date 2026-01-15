using UnityEngine;

namespace Script.UI
{
    /// <summary>
    /// Gère le curseur de la souris en gameplay.
    /// Au lancement : la souris est visible et déverrouillée.
    /// Au premier clic : la souris se verrouille au centre et disparaît.
    /// Échap : la souris réapparaît et se déverrouille.
    /// </summary>
    public class GameplayCursorManager : MonoBehaviour
    {
        private bool _cursorLocked = false;
        private bool _firstClickDone = false;

        [Tooltip("Autorise le basculement du verrouillage via clic/Échap.")]
        [SerializeField] private bool allowCursorToggle = true;

        private void Start()
        {
            // Au démarrage de la scène de gameplay, la souris est libre et visible
            UnlockCursor();
        }

        private void Update()
        {
            if (allowCursorToggle)
            {
                // --- Échap : libère la souris
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UnlockCursor();
                }

                // --- Clic gauche : verrouille la souris seulement au premier clic
                if (Input.GetMouseButtonDown(0) && !_firstClickDone)
                {
                    _firstClickDone = true;
                    LockCursor();
                }
                // --- Si déjà verrouillée et qu'on appuie Échap puis clic, reverrouille
                else if (Input.GetMouseButtonDown(0) && !_cursorLocked && _firstClickDone)
                {
                    LockCursor();
                }
            }
        }

        /// <summary>
        /// Verrouille le curseur au centre de l'écran et le cache.
        /// </summary>
        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _cursorLocked = true;
        }

        /// <summary>
        /// Libère le curseur et le rend visible.
        /// </summary>
        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _cursorLocked = false;
        }
    }
}
