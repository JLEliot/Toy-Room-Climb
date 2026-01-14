using UnityEngine;

namespace Script.UI
{
    /// <summary>
    /// Gère le curseur de la souris dans le menu principal
    /// La souris est verrouillée dans la fenêtre (pas de défilement hors de la fenêtre)
    /// mais elle reste visible et on peut cliquer partout
    /// </summary>
    public class MenuCursorManager : MonoBehaviour
    {
        void Start()
        {
            // Laisser la souris complètement libre dans le menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void Update()
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

