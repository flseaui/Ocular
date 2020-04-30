using cakeslice;
using JetBrains.Annotations;
using Level.Objects;
using Misc;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;

public class ButtonHighlightChecker : MonoBehaviour
{

    private ButtonWalkable _lastHighlightedButton;
    private Colorable _lastHighlightedOutline;

    [SerializeField] private Camera _mainCamera;

    [SerializeField] private TutorialManager _tutManager;

    private bool _doneItOnce;

    private void Update()
    {
        void DisableLastButton()
        {
            if (_lastHighlightedButton != null)
            {
                foreach (var target in _lastHighlightedButton.TargetBlocks)
                {
                    var outlines = target.transform.GetComponentsInChildren<Outline>();
                    outlines.ForEach(x => { x.enabled = false; });
                }
            }
        }

        void DisableLastOutline()
        {
            if (_lastHighlightedOutline != null)
            {
                foreach (var controller in _lastHighlightedOutline.Controllers)
                {
                    var outlines = ((ButtonWalkable) controller).transform.GetComponentsInChildren<Outline>();
                    outlines.ForEach(x => { x.enabled = false; });
                }
            }
        }
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            var hObj = GetHighlightedObject(out _);

            if (hObj)
            {
                if (hObj.transform.ParentHasComponent<ButtonWalkable>(out var button))
                {

                    if (!_doneItOnce)
                    {
                        _tutManager.DisableTutorialThree();
                        _doneItOnce = true;
                    }

                    if (button != _lastHighlightedButton)
                    {
                        DisableLastButton();
                    }

                    _lastHighlightedButton = button;

                    foreach (var target in button.TargetBlocks)
                    {
                        var outlines = target.transform.GetComponentsInChildren<Outline>();
                        _mainCamera.GetComponent<OutlineEffect>().lineColor0 =
                            Colorable.StateToColor(target.GetComponent<Colorable>().OcularColor);
                        outlines.ForEach(x => { x.enabled = true; });
                    }

                }
                else if (hObj.transform.parent.CompareTag("Outline"))
                {
                    var colorable = hObj.transform.parent.parent.GetComponent<Colorable>();

                    if (colorable != _lastHighlightedOutline)
                    {
                        DisableLastOutline();
                    }

                    _lastHighlightedOutline = colorable;
                    
                    foreach (var controller in _lastHighlightedOutline.Controllers)
                    {
                        var buttonWalkable = (ButtonWalkable) controller;
                        var outlines = buttonWalkable.transform.GetComponentsInChildren<Outline>();
                        outlines.ForEach(x =>
                        {
                            _mainCamera.GetComponent<OutlineEffect>().lineColor0 =
                                Colorable.StateToColor(buttonWalkable.GetComponent<Colorable>().OcularColor);
                            x.enabled = true;
                        });
                    }

                }
                else
                {
                    DisableLastButton();
                    DisableLastOutline();
                }
            }
            else
            {
                DisableLastButton();
                DisableLastOutline();
            }
        }
        else
        {
            DisableLastButton();
            DisableLastOutline();
        }
       
    }

    [CanBeNull]
    private GameObject GetHighlightedObject(out float3 normal)
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
