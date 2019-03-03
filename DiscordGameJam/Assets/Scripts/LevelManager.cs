using System;
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
    
    [NonSerialized]
    public int CurrentLevel = -1;

    public Camera Camera;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CurrentLevel > -1)
                RestartLevel();
        }
    }
    
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
        _selector = Instantiate(_indicatorPrefab);
        _selector.GetComponent<Indicator>().Player = _player;
        GlassesManager.Instance.Player = _player;
        GlassesManager.Instance.Reload();
        GameObject.Find("CameraTarget").transform.GetChild(0).transform.localPosition = _level.GetComponent<LevelDetails>().CameraPos;
    }

    public void RestartLevel()
    {
        Destroy(_selector);  
        Destroy(_player);
        Destroy(_level);
        LoadLevel(CurrentLevel);
    }
    
    public void StartGame()
    {
        CurrentLevel = 0;
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
