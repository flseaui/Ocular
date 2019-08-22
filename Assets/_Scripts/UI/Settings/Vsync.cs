using Michsky.UI.Zone;
using UnityEngine;

namespace UI.Settings
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