using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public int CurrentLevel;
    
    private void Start()
    {
        CurrentLevel = 1;
    }
    
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
        CurrentLevel++;
        SceneManager.LoadScene(CurrentLevel);
       
       // StartCoroutine(LoadNewScene(CurrentLevel.ToString()));
    }

    /*private IEnumerator IsDefDead()
    {
        if (_selector != null || _player != null || _level != null)
            yield return null;
                
        ++CurrentLevel;
        LoadLevel(CurrentLevel);
    }*/
    
/*    public void LoadLevel(int index)
    {
        TotalWaypoints = 0;
        FinishedWaypoints = 0;
        NeighboredWaypoints = 0;
        _level = Instantiate(_levelPrefabs[index]);
        var spawnPos = _level.GetComponent<LevelDetails>().PlayerSpawnPosition.position;
        _player = Instantiate(_playerPrefab, spawnPos, Quaternion.identity);
        _selector = Instantiate(_indicatorPrefab);
        _selector.GetComponent<Indicator>().Player = _player;
       // StartCoroutine(nameof(WaitForWaypoints));
        GlassesManager.Instance.Player = _player;
        GlassesManager.Instance.Reload();
        GameObject.Find("CameraTarget").transform.GetChild(0).transform.localPosition = _level.GetComponent<LevelDetails>().CameraPos;
    }*/

    /*private IEnumerator WaitForWaypoints()
    {
        if (FinishedWaypoints < TotalWaypoints)
            yield return null;
        CheckNeighbors?.Invoke();
        StartCoroutine(nameof(WaitForNeighbors));
    }

    private IEnumerator WaitForNeighbors()
    {
        if (NeighboredWaypoints < TotalWaypoints)
            yield return null;
        MoveUp?.Invoke();

    }*/
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void StartGame()
    {
        CurrentLevel = 1;
        SceneManager.LoadScene("1");
        //StartCoroutine(LoadNewScene("1"));
    }
    
/*    private IEnumerator LoadNewScene(string scene)
    {
        //var async =(scene);

       // while (!async.isDone) yield return null;
    }*/
    
}
