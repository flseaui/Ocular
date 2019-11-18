using System;
using DG.Tweening;
using Level;
using UnityEngine;

namespace Misc
{
    public class FogAdjuster : MonoBehaviour
    {
        [SerializeField]
        private LevelController _levelController;
        [SerializeField]
        private Transform _fogPlane;

        private HeightFogGlobal _fog;

        private void Awake()
        {
            _fog = GetComponent<HeightFogGlobal>();

            LevelController.OnLevelLoaded += OnLevelLoaded;
        }

        private void OnDestroy()
        {
            LevelController.OnLevelLoaded -= OnLevelLoaded;
        }

        private void OnLevelLoaded()
        {
            if (_levelController.CurrentLevelInfo.HasCustomFog)
            {
                DOTween.To(() => _fog.fogHeightEnd, x => _fog.fogHeightEnd = x, _levelController.CurrentLevelInfo.FogHeightEnd, 1.3f);
                DOTween.To(() => _fog.fogHeightStart, x => _fog.fogHeightStart = x, _levelController.CurrentLevelInfo.FogHeightStart, 1.3f);

                var position = _fogPlane.DOMoveY(_levelController.CurrentLevelInfo.FogPlaneY, 1.3f);
            }
            else
            {
                _fog.fogHeightEnd = -0.3f;
                _fog.fogHeightStart = -2.8f;

                var position = _fogPlane.position;
                position = new Vector3(position.x, -2.86f, position.z);
                _fogPlane.position = position;
            }

            if (_levelController.CurrentLevelInfo.HasCustomFogColor)
            {
                DOTween.To(() => _fog.fogColor, x => _fog.fogColor = x, _levelController.CurrentLevelInfo.FogColor, 1.3f);
            }
            else
            {
                _fog.fogColor = new Color(0.6235294f, 0.9294118f, 0.9333333f);
            }
        }
    }
}
