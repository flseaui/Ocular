using cakeslice;
using JetBrains.Annotations;
using Level.Objects;
using Misc;
using Sirenix.Utilities;
using UnityEngine;

public class ButtonHighlightChecker : MonoBehaviour
{

    private ButtonWalkable _lastHighlighted;

    [SerializeField] private Camera _mainCamera;

    private void Update()
    {
        void DisableLast()
        {
            if (_lastHighlighted != null)
            {
                foreach (var target in _lastHighlighted.TargetBlocks)
                {
                    var outlines = target.transform.GetComponentsInChildren<Outline>();
                    outlines.ForEach(x => { x.enabled = false; });
                }
            }
        }

        var hObj = GetHighlightedObject(out _);
        if (hObj)
        {

            if (hObj.transform.ParentHasComponent<ButtonWalkable>(out var button))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _lastHighlighted = button;

                    if (button != _lastHighlighted)
                    {
                        DisableLast();
                    }

                    foreach (var target in button.TargetBlocks)
                    {
                        var outlines = target.transform.GetComponentsInChildren<Outline>();
                        _mainCamera.GetComponent<OutlineEffect>().lineColor0 =
                            Colorable.StateToColor(target.GetComponent<Colorable>().OcularState);
                        outlines.ForEach(x => { x.enabled = true; });
                    }
                }
                else
                {
                    DisableLast();
                }
            }
            else
            {
                DisableLast();
            }
        }
        else
        {
            DisableLast();
        }
    }

    [CanBeNull]
    private GameObject GetHighlightedObject(out Vector3 normal)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Model")))
        {
            normal = hit.normal;
            return hit.collider.gameObject;
        }

        normal = Vector3.zero;
        return null;
    }
}
