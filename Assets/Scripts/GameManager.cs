using UnityEngine;

public class GameManager : MonoBehaviour
{
    private LevelController _levelController;
    private GlassesController _glassesController;

    private void Awake()
    {
        _levelController = GetComponent<LevelController>();
        _glassesController = GetComponent<GlassesController>();
    }

    private void Start()
    {
        _levelController.LoadLevel(0);
    }
}
