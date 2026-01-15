using UnityEngine;

namespace Script.UI
{
    /// <summary>
    /// Gère le curseur de la souris dans le menu principal.
    /// La souris est libre, visible et utilisable sur tout l'UI.
    /// </summary>
    public class MenuCursorManager : MonoBehaviour
    {
        private void Start()
        {
            // Laisser la souris complètement libre dans le menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Update()
        {
            // S'assurer que la souris reste toujours libre au menu
            if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
