using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GlassesController : MonoBehaviour
    {
        public static Color CurrentGlassesColor;
        public static Action OnGlassesToggled;
        
        public List<Glasses> ActiveGlasses;
        
        private Dictionary<Color, Image> _colorIndicators;
        [SerializeField] private GameObject _colorIndicator;
        [SerializeField] private GameObject _glassesContainer;

        private Color CombinedColor
        {
            get
            {
                var temp = Color.black;
                ActiveGlasses.Where(x => x.Enabled).ForEach(x =>
                {
                    temp.r += x.Color.r;
                    temp.g += x.Color.g;
                    temp.b += x.Color.b;
                });
                CurrentGlassesColor = temp;
                return temp;
            }
        }

        private void Awake()
        {
            _colorIndicators = new Dictionary<Color, Image>();
        }

        private void Update()
        {
            foreach (var glasses in ActiveGlasses)
                if (Input.GetKeyDown(glasses.Keybind))
                    ToggleGlasses(glasses);
        }

        public void ResetGlasses(List<Glasses> glasses)
        {
            glasses.ForEach(x => x.Enabled = false);
            ActiveGlasses = glasses;
            _colorIndicators.Clear();
            for (var i = 0; i < _glassesContainer.transform.childCount; ++i)
                Destroy(_glassesContainer.transform.GetChild(i).gameObject);

            foreach (var activeGlasses in ActiveGlasses)
                CreateGlassesIndicator(activeGlasses);

            // ToggleGlasses(ActiveGlasses[0], true);
        }

        public void AddGlasses(Glasses glasses)
        {
            glasses.Enabled = false;
            ActiveGlasses.Add(glasses);
            CreateGlassesIndicator(glasses);
        }

        public void AddGlasses(List<Glasses> glasses)
        {
            glasses.ForEach(x => x.Enabled = false);
            ActiveGlasses.AddRange(glasses);
            foreach (var activeGlasses in glasses)
                CreateGlassesIndicator(activeGlasses);
        }

        private void CreateGlassesIndicator(Glasses glasses)
        {
            var indicator = Instantiate(_colorIndicator, _glassesContainer.transform);
            indicator.GetComponent<Image>().color = glasses.Enabled ? glasses.Color : glasses.Color / 2;
            _colorIndicators.Add(glasses.Color, indicator.GetComponent<Image>());
        }

        public void ToggleGlasses(Glasses glasses, bool bypassLimit = false)
        {
            if (ActiveGlasses.Contains(glasses))
            {
                if (bypassLimit || glasses.Enabled || ActiveGlasses.Count(x => x.Enabled) < 2)
                {
                    var index = ActiveGlasses.IndexOf(glasses);
                    
                    _colorIndicators[glasses.Color].color = ActiveGlasses[index].Enabled
                        ? _colorIndicators[glasses.Color].color / 2
                        : glasses.Color;

                    ActiveGlasses[index].Enabled = !ActiveGlasses[index].Enabled;

                    OnGlassesToggled?.Invoke();
                }
            }
        }
    }
}