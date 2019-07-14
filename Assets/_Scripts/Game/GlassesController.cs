using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using DarkTonic.MasterAudio;
using Level;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public enum OcularState
    {
        Zero,   // white
        One,    // red
        Two,    // yellow 
        Three,  // blue
        Four,   // orange
        Five,   // magenta
        Six,     // green
        
        Null
    }
    
    /// <summary>
    /// <c>GlassesController</c> keeps track of and manipulates all current glasses.
    /// </summary>
    public class GlassesController : MonoBehaviour
    {
        /// <summary>
        /// The combined color of all active glasses.
        /// </summary>
        [ShowInInspector, ReadOnly]
        public static OcularState CurrentOcularState;
        /// <summary>
        /// <c>OnGlassesToggled</c> is invoked when glasses are removed or put on.
        /// </summary>
        public static Action OnGlassesToggled;
        
        /// <summary>
        /// The list of currently active glasses.
        /// </summary>
        public List<Glasses> ActiveGlasses;
        
        private Dictionary<Color, Image> _colorIndicators;
        [SerializeField] private GameObject _colorIndicator;
        [SerializeField] private GameObject _glassesContainer;

        private List<PlaySoundResult> _musicStreams;
        
        private readonly IDictionary<string, OcularState> _glassesMap = new Dictionary<string, OcularState>
        {
            {"", OcularState.Zero},
            {"1", OcularState.One},
            {"2", OcularState.Two},
            {"3", OcularState.Three},
            {"12", OcularState.Four},
            {"13", OcularState.Five},
            {"23", OcularState.Six}
        };
        
        private MapController _mapController;
        private MapController MapController
        {
            get
            {
                if (_mapController == null)
                    _mapController = GetComponent<LevelController>().CurrentLevelInfo.GetComponent<MapController>();
                return _mapController;
            }
        }

        /// <summary>
        /// Calculate the current ocular state based on the enabled glasses.
        /// </summary>
        private void CalcOcularState()
        {
            var glasses = ActiveGlasses.Where(x => x.Enabled).Select(x => x.GlassesType).ToArray();
            var str = "";
            glasses.ForEach(x => str += (int) x + 1);
            CurrentOcularState = _glassesMap[str];
        }

        private void Awake()
        {
            _colorIndicators = new Dictionary<Color, Image>();
            _musicStreams = new List<PlaySoundResult>();
        }

        private IEnumerator Start()
        {
            _musicStreams.Add(MasterAudio.PlaySound("World1_A"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_B"));
            _musicStreams.Add(MasterAudio.PlaySound("World1_C"));
            
            yield return new WaitForFixedUpdate();
            ToggleGlasses(ActiveGlasses[0], true);
        }

        private void Update()
        {
            foreach (var glasses in ActiveGlasses)
                if (Input.GetKeyDown(glasses.Keybind))
                    ToggleGlasses(glasses);
        }

        /// <summary>
        /// Resets the active glasses to the provided list.
        /// </summary>
        /// <param name="glasses">
        /// The list of glasses to be used.
        /// </param>
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

        /// <summary>
        /// Adds a pair of glasses to the glasses list, defaults to disabled.
        /// </summary>
        /// <param name="glasses">
        /// The glasses to be added.
        /// </param>
        public void AddGlasses(Glasses glasses)
        {
            glasses.Enabled = false;
            ActiveGlasses.Add(glasses);
            CreateGlassesIndicator(glasses);
        }

        /// <summary>
        /// Adds multiple pairs of glasses to the glasses list, defaults to disabled.
        /// </summary>
        /// <param name="glasses">
        /// The list of glasses to be added.
        /// </param>
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
        
        /// <summary>
        /// Toggled a pair of glasses (puts them on or takes them off).
        /// </summary>
        /// <param name="glasses">
        /// The pair of glasses to be toggled.
        /// </param>
        /// <param name="bypassLimit">
        /// If true it will ignore the levels max glasses limit.
        /// </param>
        public void ToggleGlasses(Glasses glasses, bool bypassLimit = false)
        {
            if (ActiveGlasses.Contains(glasses))
            {
                var active = ActiveGlasses.Count(x => x.Enabled);

                var index = ActiveGlasses.IndexOf(glasses);

                if (active == 2 && !glasses.Enabled)
                    return;
                if (active == 1 && glasses.Enabled)
                    return;
                
                var type = (int) glasses.GlassesType;
                
                _musicStreams[type].ActingVariation.FadeToVolume(glasses.Enabled ? 0 : AudioMixing.WorldOneVolumes[type], 1);
                
                
                
                _colorIndicators[glasses.Color].color = ActiveGlasses[index].Enabled
                    ? _colorIndicators[glasses.Color].color / 2
                    : glasses.Color;

                ActiveGlasses[index].Enabled = !ActiveGlasses[index].Enabled;
                CalcOcularState();
                MapController.UpdateColorables();
                OnGlassesToggled?.Invoke();
            }
        }
    }
}