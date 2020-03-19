using System.Collections;
using Game;
using Level;
using Michsky.UI.Zone;
using Misc;
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

        public LevelName _levelName;
        
        private void Awake()
        {
            _cg = _pauseMenuUI.GetComponent<CanvasGroup>();
            _levelName = GameObject.Find("LevelNameManager").GetComponent<LevelName>();
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

            if (Input.GetKeyDown(KeyCode.R) && !Player.Player.Teleporting && !LevelController.LevelTransitioning)
            {
                Restart();
            }
        }

        public void Resume()
        {
            _cg.alpha = 0;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
            
            _levelName.HideText();
            
            //_pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        private void Pause()
        {
            _cg.alpha = 1;
            _cg.interactable = true;
            _cg.blocksRaycasts = true;
            
            _levelName.ShowText();
            
            //_pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }

        /*---------PAUSE MENU BUTTONS---------*/

        public void Restart()
        {
            if (Restarting) return;
            
            Restarting = true;
            Resume();
            var controller = GameObject.Find("GameManager").GetComponent<LevelController>();
            var player = controller.EntityManager.Player;
            
            if (player != null)
            {
                player.GetComponent<Player.Player>().CommitDie();
            }
            
            controller.EntityManager.ClearEntities();
            controller.RestartLevel();
        }

        public void LevelSelect()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            LoadingScreen.SceneToLoad = "MainMenu";
            MenuVars.StartingPage = 3;
            SceneManager.LoadScene("MainMenu");
        }
        public void Settings()
        {
            Time.timeScale = 1f;
        }

        public void Quit()
        {
            Time.timeScale = 1f;
            GameIsPaused = false;
            LoadingScreen.SceneToLoad = "MainMenu";
            SceneManager.LoadScene("MainMenu");
        }

    }
}
