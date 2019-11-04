using Michsky.UI.Zone;
using UnityEngine;

namespace UI.Settings.Visuals
{
    public class DisplayMode : MonoBehaviour
    {
        public void Change()
        {
            switch (GetComponent<HorizontalSelector>().CurrentElement)
            {
                case "FULLSCREEN":
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case "BORDERLESS":
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case "WINDOWED":
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }
    }
}
