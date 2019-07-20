using UnityEngine;
using UnityEngine.SceneManagement;

namespace OldEditor
{
    public class CreateLevelWindow : MonoBehaviour
    {
        public void CreateLevel()
        {
            SceneManager.LoadScene("NewEditor");
        }
    }
}