using System;
using Game;
using Misc;
using UnityEngine;

namespace Level.Objects
{
    public class Clone : MonoBehaviour
    {
        private GameObject _player;

        private Vector3 _initialPlayerPos;
        private Vector3 _initialPos;
        private bool _dontMove;
        private void Start()
        {
            _player = GameObject.Find("GameManager").GetComponent<GameManager>().Player;
            _initialPlayerPos = _player.transform.position;
            _initialPos = transform.position;
        }

        private void CheckForward()
        {
            _dontMove = true;
            if (Physics.Raycast(transform.localPosition + transform.forward, new Vector3(0, -1, 0), out var hit, 2f))
                if (hit.transform.ParentHasComponent<Walkable>(out var walkable))
                {
                    _dontMove = false;
                }
        }
        
        private void Update()
        {
            var offset = _initialPlayerPos - _player.transform.position;

            transform.rotation = Quaternion.AngleAxis(-_player.transform.eulerAngles.y, transform.up);

            CheckForward();

            if (_dontMove) return;
            transform.position = _initialPos + offset;
        }
    }
}