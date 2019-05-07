using System.IO;
using Level;
using Sirenix.Utilities;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LevelEditor
{
    public class LevelSelectPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content;

        [SerializeField] private GameObject _levelBannerPrefab;
        [SerializeField] private GameObject _levelNamePanelPrefab;
        
        private void Start()
        {
            CustomCursor.SetCursor(null);
            var prefabs = Directory.GetFiles("Assets/Prefabs/Levels", "*.prefab", SearchOption.AllDirectories);
            foreach(var file in prefabs)
            {
                var assetPath = file.Replace(Application.dataPath, "").Replace('\\', '/');
                var source = (GameObject) AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                var banner = Instantiate(_levelBannerPrefab, _content.transform);
                banner.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = source.name;
                banner.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    SceneManager.LoadSceneAsync("LevelEditor").completed += operation =>
                    {
                        PlayerPrefs.SetString("LevelToLoad", assetPath);
                    };
                });
                banner.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    Debug.Log(assetPath);
                    Destroy(banner);
                });
                banner.transform.Find("EditButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    var panel = Instantiate(_levelNamePanelPrefab, GameObject.Find("Canvas").transform);
                    panel.transform.Find("NameInputField").GetComponent<TMP_InputField>().onEndEdit.AddListener(text =>
                    {
                        source.name = text;
                        AssetDatabase.RenameAsset(assetPath, text);
                        source.GetComponent<LevelInfo>().Name = assetPath = AssetDatabase.GetAssetPath(source);
                        banner.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = text;
                    });
                    panel.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Destroy(panel.gameObject);
                    });
                });
            }
        }

        public void NewLevel()
        {
            SceneManager.LoadSceneAsync("LevelEditor").completed += operation =>
            {
                PlayerPrefs.SetString("LevelToLoad", "New");
            };
        }
        
    }
}