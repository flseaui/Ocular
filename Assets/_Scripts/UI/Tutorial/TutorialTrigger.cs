using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    private TutorialManager _tutManager;

    private void Awake()
    {
        _tutManager = GameObject.Find("TutorialManager").GetComponent<TutorialManager>();
    }


    private void OnEnable()
    {
        _tutManager.SetTutThree(true);
    }

    private void OnDisable()
    {
        _tutManager.SetTutThree(false);
    }
}
