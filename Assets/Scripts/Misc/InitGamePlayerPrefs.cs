using UnityEngine;

namespace Misc
{
    public class InitGamePlayerPrefs : MonoBehaviour
    {
        private void Start()
        {
            PlayerPrefs.SetInt("PlayFromEditor", 0);
        }
    }
}