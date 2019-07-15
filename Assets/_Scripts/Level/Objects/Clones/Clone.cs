using System;
using Game;
using Misc;
using Player;
using UnityEngine;

namespace Level.Objects
{
    public class Clone : MonoBehaviour
    {
        private GameObject _player;
        private void Start()
        {
            _player = GameObject.Find("GameManager").GetComponent<GameManager>().Player;
            GetComponent<ClonePathfinder>().WalkSpeed = _player.GetComponent<Pathfinder>().WalkSpeed;
        }

        private void Update()
        {
            if (_player.GetComponent<Pathfinder>().Navigating && !GetComponent<ClonePathfinder>().Navigating)
            {
                GetComponent<ClonePathfinder>().MirrorClone(_player.GetComponent<Pathfinder>().GetCurrentWalkable(out _), _player.GetComponent<Pathfinder>()._currentEnd);
            }
        }
    }
}