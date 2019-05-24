using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Misc {
    public class HideOnStart : MonoBehaviour
    {
        private enum Mode
        {
            HideMesh,
            Disable
        }

        [SerializeField]
        private Mode _mode;
        
        public void Start()
        {
            if (SceneManager.GetActiveScene().name == "NewEditor") return;
            
            switch (_mode)
            {
                case Mode.HideMesh:
                    GetComponent<MeshRenderer>().enabled = false;
                    break;
                case Mode.Disable:
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }            
        }
    }
}