
using Sirenix.OdinInspector;
using UnityEngine;

public class ButtonWalkable : Walkable
{
    [SerializeField]
    private Button _buttonModel;

    private Vector3 _initialPosition;

    private void Start()
    {
        _initialPosition = _buttonModel.transform.localPosition;
    }

    private void Update()
    {
        if (_buttonModel.State)
        {
            _buttonModel.transform.localPosition = _initialPosition - new Vector3(0, .1f, 0);
        }
        else
        {
            _buttonModel.transform.localPosition = _initialPosition;
        }
    }
}
