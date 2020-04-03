using Game;
using Level;
using UnityEngine;

namespace Misc
{
    public class NextLevelButton : MonoBehaviour
    {
        public void NextLevel()
        {
            LevelController.OnLevelBeginUnload?.Invoke();
        }
    }
}
