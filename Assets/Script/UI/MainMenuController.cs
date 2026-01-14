using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "SampleScene";

        public void Play()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}