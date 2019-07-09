using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelEditor
{
    public class CreateLevelWindow : MonoBehaviour
    {
        public void CreateLevel()
        {
            SceneManager.LoadScene("NewEditor");
        }
    }
}