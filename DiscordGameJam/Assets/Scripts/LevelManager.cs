using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private GameObject[] _levelPrefabs;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _indicatorPrefab;

    private GameObject _level, _player, _selector;
    
    public int CurrentLevel;
    
    public void LoadNextLevel()
    {
        Destroy(_selector);  
        Destroy(_player);
        Destroy(_level);
        
        ++CurrentLevel;
        LoadLevel(CurrentLevel);
    }

    public void LoadLevel(int index)
    {
        _level = Instantiate(_levelPrefabs[index]);
        var spawnPos = _level.GetComponent<LevelDetails>().PlayerSpawnPosition.position;
        _player = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
        _selector = Instantiate(_indicatorPrefab).GetComponent<Indicator>().Player = _player;
        GlassesManager.Instance.Player = _player;
        GlassesManager.Instance.Reload();
    }

    public void StartGame()
    {
        CurrentLevel = 1;
        StartCoroutine(LoadNewScene("Game"));
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            LoadLevel(CurrentLevel);
        };
    }
    
    private IEnumerator LoadNewScene(string scene)
    {
        var async = SceneManager.LoadSceneAsync(scene);

        while (!async.isDone) yield return null;
    }
    
}
