
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ButtonWalkable : Walkable
{
    [SerializeField]
    private Button _buttonModel;

    private Vector3 _initialPosition;

    [SerializeField] private List<Colorable> _targetBlocks;

    [SerializeField, ColorPalette("RGB")] private Color _color;
    
    private void Start()
    {
        if (_buttonModel == null)
            transform.Find("ButtonModel").gameObject.AddComponent<Button>();

        _initialPosition = _buttonModel.transform.localPosition;

        _buttonModel.OnStateChanged += () =>
        {
            if (_buttonModel.State)
            {
                _buttonModel.transform.localPosition = _initialPosition - new Vector3(0, .1f, 0);
                _targetBlocks.ForEach(t => t.ChangeColorWithOutline(_color));
            }
            else
            {
                _buttonModel.transform.localPosition = _initialPosition;
                _targetBlocks.ForEach(t => t.ResetColorFromOutline());
            }
        };
    }
}
