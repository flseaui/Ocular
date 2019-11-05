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

    private GameObject _liveTutorialTwo;

    public void Awake()
    {
        LevelController.OnLevelLoaded += () =>
        {
            switch (_levelController.CurrentLevelInfo.InGameName)
            {
                case "A New Beginning":
                    Instantiate(_tutorialOnePrefab, _canvas.transform);
                    break;
                case "The Bridge":
                    _liveTutorialTwo = Instantiate(_tutorialTwoPrefab, _canvas.transform);
                    break;
            }
        };
    }

    public void DisableTutorialTwo()
    {
        if (_liveTutorialTwo != null)
        {
            _liveTutorialTwo.GetComponent<LeftAndRight>().FadeOut();
        }
    }
}
