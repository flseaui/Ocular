using Level;
using UnityEngine;

namespace UI
{
    public class LevelButton : MonoBehaviour
    {
        [SerializeField]
        private int _worldIndex;

        [SerializeField]
        private int _levelIndex;

        public void LoadLevel()
        {
            LevelController.StartingLevelIndex = (_worldIndex, _levelIndex);
            LoadingScreen.SceneToLoad = "Game";
            LoadingScreen.Instance.Show();
        }
    }
}
