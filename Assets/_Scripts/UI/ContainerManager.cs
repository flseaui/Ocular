using UnityEngine;

namespace UI
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] private GameObject _worldContainer;
        [SerializeField] private GameObject _levelContainer;

        public void SetWorldContainer(bool value)
        {
            _worldContainer.SetActive(value);
        }

        public void SetLevelContainer(bool value)
        {
            _levelContainer.SetActive(value);
        }
    }
}
