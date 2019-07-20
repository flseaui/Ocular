using System.IO;
using Level;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace OldEditor
{
    public class LevelSelectPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _content;

        [SerializeField] private GameObject _levelBannerPrefab;
        [SerializeField] private GameObject _levelNamePanelPrefab;
        
        private void Start()
        {
            CustomCursor.SetCursor(null);
            var prefabs = Directory.GetFiles("Assets/_Prefabs/Levels", "*.prefab", SearchOption.AllDirectories);
            foreach(var file in prefabs)
            {
                var assetPath = file.Replace(Application.dataPath, "").Replace('\\', '/');
                var source = (GameObject) AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                var banner = Instantiate(_levelBannerPrefab, _content.transform);
                banner.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = source.name;
                banner.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    SceneManager.LoadSceneAsync("NewEditor").completed += operation =>
                    {
                        PlayerPrefs.SetString("LevelToLoad", assetPath);
                    };
                });
                banner.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (File.Exists($"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png"))
                        File.Delete($"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png");
                    
                    AssetDatabase.Refresh();
                    
                    AssetDatabase.DeleteAsset(assetPath);
                    Destroy(banner);
                });
                banner.transform.Find("EditButton").GetComponent<Button>().onClick.AddListener(() =>
                {
                    var panel = Instantiate(_levelNamePanelPrefab, GameObject.Find("Canvas").transform);
                    panel.transform.Find("NameInputField").GetComponent<TMP_InputField>().onEndEdit.AddListener(text =>
                    {
                        var oldFile = $"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png";
                        source.name = text;
                        AssetDatabase.RenameAsset(assetPath, text);
                        source.GetComponent<LevelInfo>().Name = assetPath = AssetDatabase.GetAssetPath(source);
                        banner.transform.Find("LevelName").GetComponent<TextMeshProUGUI>().text = text;
                        if (File.Exists(oldFile))
                            File.Move(oldFile, $"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png");
                        
                        AssetDatabase.Refresh();
                    });
                    panel.transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Destroy(panel.gameObject);
                    });
                });
                var img = banner.transform.Find("LevelPreview").GetComponent<Image>();
                Texture2D tex = null;
                if (File.Exists($"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png") )
                {
                    var fileData = File.ReadAllBytes($"Assets/_Prefabs/Levels/Thumbnails/thumb_{source.name}.png");
                    tex = new Texture2D(2, 2);
                    tex.LoadImage(fileData);
                }
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            }
        }

        public void NewLevel(int size)
        {
            SceneManager.LoadSceneAsync("NewEditor").completed += operation =>
            {
                PlayerPrefs.SetString("LevelToLoad", "New");
                PlayerPrefs.SetInt("LevelSize", size);
            };
        }
        
    }
}

#endif