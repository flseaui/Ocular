using System;
using Level;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _canvas;

    [SerializeField]
    private LevelController _levelController;

    [SerializeField]
    private GameObject _tutorialOnePrefab;
    [SerializeField]
    private GameObject _tutorialTwoPrefab;
    [SerializeField]
    private GameObject _tutorialThreePrefab;
    [SerializeField]
    private GameObject _tutorialZoomPrefab;

    private GameObject _liveTutorialOne;
    private GameObject _liveTutorialTwo;
    private GameObject _liveTutorialThree;
    private GameObject _liveTutorialFour;


    public void Awake()
    {
        LevelController.OnLevelLoaded += OnLevelLoaded;
    }

    public void OnDestroy()
    {
        LevelController.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded()
    {
        
        switch (_levelController.CurrentLevelInfo.InGameName)
        {
            case "A New Beginning":
                if (_liveTutorialOne != null)
                    Destroy(_liveTutorialOne);
                _liveTutorialOne = Instantiate(_tutorialOnePrefab, _canvas.transform);
                break;
            case "The Bridge":
                if (_liveTutorialTwo != null)
                    Destroy(_liveTutorialTwo);
                _liveTutorialTwo = Instantiate(_tutorialTwoPrefab, _canvas.transform);
                break;
            case "Midnight Stroll":
                if (_liveTutorialThree != null)
                    Destroy(_liveTutorialThree);
                _liveTutorialThree = Instantiate(_tutorialThreePrefab, _canvas.transform);
                break;
            case "The Forest":
                if (_liveTutorialFour != null)
                    Destroy(_liveTutorialFour);
                _liveTutorialFour = Instantiate(_tutorialZoomPrefab, _canvas.transform);
                break;
        }
    }

    public void DisableTutorialTwo()
    {
        if (_liveTutorialTwo != null)
        {
            _liveTutorialTwo.GetComponent<LeftAndRight>().FadeOut();
        }
    }

    public void DisableTutorialThree()
    {
        if (_liveTutorialThree != null)
        {
            _liveTutorialThree.GetComponent<Shift>().FadeOut();
        }
    }

    public void SetTutThree(bool value)
    {
        if (_liveTutorialThree != null)
            _liveTutorialThree.SetActive(value);
    }
}
