using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _levels;

    private GameObject _loadedLevel;
    private int _loadedLevelNumber;

    private int LevelCount => _levels.Count;
    
    [ValueDropdown("_levels")]
    public GameObject StartingLevel;
    
    public LevelInfo CurrentLevelInfo;
    
    public void LoadNextLevel()
    {
        LoadLevel(_loadedLevelNumber + 1);
    }

    public void LoadLevel()
    {
        LoadLevel(_levels.IndexOf(StartingLevel));
    }
    
    public void LoadLevel(int levelNumber)
    {
        if (_loadedLevel != null)
            UnloadLevel();
        _loadedLevel = Instantiate(_levels[levelNumber]);
        _loadedLevel.GetComponent<MapGenerator>().FindNeighbors();
        CurrentLevelInfo = _loadedLevel.GetComponent<LevelInfo>();
        _loadedLevelNumber = levelNumber;
    }
    
    private void UnloadLevel()
    {
        Destroy(_loadedLevel);
    }

}
