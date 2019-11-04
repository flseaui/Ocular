using UnityEngine;

namespace UI.Settings.Visuals
{
    public class Vsync : MonoBehaviour
    {
        public void On()
        {
            QualitySettings.vSyncCount = 1;
        }

        public void Off()
        {
            QualitySettings.vSyncCount = 0;
        }
    }
}
