using Game;
using UnityEngine;

namespace Misc
{
    public class NextLevelButton : MonoBehaviour
    {
        public void NextLevel()
        {
            GameManager.OnLevelLoad?.Invoke();
        }
    }
}
