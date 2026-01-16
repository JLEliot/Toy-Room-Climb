using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameplaySceneName = "SampleScene";
        [SerializeField] private string gameplaySceneName2 = "COCHONPLATFORME";

        public void Play()
        {
            SceneManager.LoadScene(gameplaySceneName);
            SceneManager.LoadScene(gameplaySceneName2, LoadSceneMode.Additive);
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