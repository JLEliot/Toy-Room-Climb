using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.UI
{
    /// <summary>
    /// Contrôleur du menu principal (Play/Quit).
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Tooltip("Nom de la scène de gameplay à charger.")]
        [SerializeField] private string gameplaySceneName = "SampleScene";

        /// <summary>
        /// Charge la scène de gameplay.
        /// </summary>
        public void Play()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }

        /// <summary>
        /// Quitte l'application (et stoppe le Play en Editor).
        /// </summary>
        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
