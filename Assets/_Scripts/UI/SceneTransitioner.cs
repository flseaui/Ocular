using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SceneTransitioner : MonoBehaviour
    {
        public void Start()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
