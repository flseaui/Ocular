using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [SerializeField] 
    private SpriteRenderer _spriteRenderer;
    
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + .5f, hit.transform.position.z);
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            _player.GetComponent<PathManager>().NavigateTo(hit.transform.position);
        }
    }
}
