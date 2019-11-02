using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void StartNewGame()
        {
            LoadingScreen.SceneToLoad = "Game";
            LoadingScreen.Instance.Show();
        }

        private IEnumerator LoadScene(string scene)
        {
            var AO = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            AO.allowSceneActivation = false;
            while (AO.progress < 0.9f)
            {
                yield return null;
            }

            //Fade the loading screen out here

            AO.allowSceneActivation = true;
        }

        public void StartMenuMusic()
        {
            GetComponent<AudioSource>().Play();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
