using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WorldSelect : MonoBehaviour
{
    private GraphicRaycaster _graphicRaycaster;
    private PointerEventData _pointer;

    // Prefabs
    [SerializeField] private GameObject _levelButtonPrefab;
    
    [SerializeField] private GameObject _levelContainer;
    private void Awake()
    {
        var canvas = GameObject.Find("Canvas");
        _pointer = new PointerEventData(canvas.GetComponent<EventSystem>());
        _graphicRaycaster = canvas.GetComponent<GraphicRaycaster>();
    }
    
    private void Update()
    {
        var obj = GetObjectUnderMouse();

        if (obj is null)
            return;
        
        if (obj.name == "WorldOne")
            if (Input.GetMouseButtonDown(0))
                SwitchToLevelSelect();        
    }

    private void SwitchToLevelSelect()
    {
        transform.Find("WorldContainer").gameObject.SetActive(false);
        _levelContainer.SetActive(true);
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene("Game");
    }
    
    private GameObject GetObjectUnderMouse()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit,Mathf.Infinity, LayerMask.GetMask("UI")))
        {
            return hit.collider.gameObject;
        }
            
        return null;
    }
    
    private GameObject GetUIUnderMouse()
    {
        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(_pointer, results);

        return results.Count > 0 ? results[0].gameObject : null;
    }
}
