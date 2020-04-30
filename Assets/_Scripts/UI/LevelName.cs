using System;
using Level;
using System.Collections;
using Doozy.Engine.Extensions;
using TMPro;
using Unity.Mathematics;
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

            _levelNameText.text = _levelController.LevelInfo.InGameName;
            StartCoroutine(ShowName());
        }

        public void ShowText()
        {
            _animator.enabled = false;
            _levelNameText.rectTransform.anchoredPosition = new float2(0, -77.09f);
            _levelNameText.color = _levelNameText.color.Opaque();
        }

        public void HideText()
        {
            _levelNameText.rectTransform.anchoredPosition = new float2(0, 50.64978f);
            _levelNameText.color = _levelNameText.color.WithAlpha(0);
            _animator.enabled = true;
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
