using Game;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;

        [SerializeField]
        private GameObject _pauseMenuUI;

        private CanvasGroup _cg;

        public static bool Restarting;

        private void Awake()
        {
            _cg = _pauseMenuUI.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            //_pauseMenuUI.SetActive(false);
            _cg.alpha = 0;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

        public void Resume()
        {
            _cg.alpha = 0;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
            //_pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        private void Pause()
        {
            _cg.alpha = 1;
            _cg.interactable = true;
            _cg.blocksRaycasts = true;
            //_pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }

        /*---------PAUSE MENU BUTTONS---------*/

        public void Restart()
        {
            Restarting = true;
            Resume();
            var manager = GameObject.Find("GameManager");
            manager.GetComponent<GameManager>().Player.GetComponent<Player.Player>().SuperKill();
            manager.GetComponent<LevelController>().RestartLevel();
        }

        public void Settings()
        {
            Time.timeScale = 1f;
        }

        public void Quit()
        {
            Time.timeScale = 1f;
            LoadingScreen.SceneToLoad = "MainMenu";
            SceneManager.LoadScene("LoadingScene");
        }

    }
}
