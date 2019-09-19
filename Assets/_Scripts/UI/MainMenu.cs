using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public void StartNewGame()
        {
            SceneManager.LoadScene("Game");
        }

        public void StartMenuMusic()
        {
            GetComponent<AudioSource>().Play();
        }
    }
}