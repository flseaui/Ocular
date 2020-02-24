using System;
using System.Collections;
using Michsky.UI.Zone;
using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UI.Settings.Audio;
using VolumeManager = UI.Settings.Audio.VolumeManager;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        private void Start()
        {
            if (MenuVars.StartingPage != -1)
            {
                StartCoroutine(DoItInATenthSec());
            }
            VolumeManager.UpdateAudioSettings();
        }

        private IEnumerator DoItInATenthSec()
        {
            yield return new WaitForSeconds(.02f);
            GameObject.Find("Canvas")
                .transform.Find("Main Panels")
                .GetComponent<MainPanelManager>()
                .PanelAnim(3);
            GameObject.Find("ContainerManager").GetComponent<ContainerManager>().SetWorldContainer(true);
        }
        
        public void StartNewGame()
        {
            LoadingScreen.SceneToLoad = "Game";
            LoadingScreen.Instance.Show();
        }

        private IEnumerator LoadScene(string scene)
        {
            var ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            ao.allowSceneActivation = false;
            while (ao.progress < 0.9f)
            {
                yield return null;
            }

            //Fade the loading screen out here

            ao.allowSceneActivation = true;
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
