using System;
using Level;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LevelName : MonoBehaviour
    {
        [SerializeField] private LevelController _levelController;
        [SerializeField] private TextMeshProUGUI _levelNameText;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            LevelController.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            _animator.ResetTrigger("Show");
            _levelNameText.transform.position += Vector3.up;

            _levelNameText.text = _levelController.CurrentLevelInfo.InGameName;
            StartCoroutine(ShowName());
        }

        private IEnumerator ShowName()
        {
            yield return new WaitForSeconds(1.5f);

            if (PauseMenu.Restarting)
            {
                PauseMenu.Restarting = false;
                yield break;
            }

            _animator.SetTrigger("Show");
        }
    }
}
