using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UI
{
    public class MenuPanelManager : MonoBehaviour
    {

        [SerializeField]
        private CanvasGroup _startingPanel;

        private CanvasGroup _currentPanel;

        private void Awake()
        {
            _currentPanel = _startingPanel;
        }

        [UsedImplicitly]
        public void SwitchPanels(CanvasGroup panel)
        {
            _currentPanel.alpha = 0;
            _currentPanel.gameObject.SetActive(false);
            panel.alpha = 1;
            panel.gameObject.SetActive(true);
            _currentPanel = panel;
        }
        
    }
}