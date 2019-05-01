using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LevelEditor
{
    public class LevelEditor : MonoBehaviour
    {
        [SerializeField] GameObject _currentObject;
        private GameObject _level;
        private void Start()
        {
            CreateLevel();
        }
        
        public void CreateLevel()
        {
            Addressables.LoadAsset<GameObject>("blank_level").Completed += handle =>
            {
                _level = Instantiate(handle.Result).transform.Find("MainFloor").gameObject;
                GameObject.Find("Main Camera").GetComponent<CameraOrbit>().Target = _level.transform;
            };
        }

        public void PlaceObject(Vector3 position)
        {
            Instantiate(_currentObject, position, Quaternion.identity, _level.transform);
        }
    }
}