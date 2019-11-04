using Michsky.UI.Zone;
using UnityEngine;

namespace UI.Settings.Visuals
{
    public class Resolution : MonoBehaviour
    {
        public void Change()
        {
            switch (GetComponent<HorizontalSelector>().CurrentElement)
            {
                case "7680 x 4320 (8K)":
                    Screen.SetResolution(7680, 4320, Screen.fullScreen);
                    break;
                case "5120 x 2880 (5K)":
                    Screen.SetResolution(5120, 2880, Screen.fullScreen);
                    break;
                case "3840 x 2160 (4K)":
                    Screen.SetResolution(3840, 2160, Screen.fullScreen);
                    break;
                case "2560 x 1440":
                    Screen.SetResolution(2560, 1440, Screen.fullScreen);
                    break;
                case "1920 x 1080":
                    Screen.SetResolution(1920, 1080, Screen.fullScreen);
                    break;
                case "1664 x 936":
                    Screen.SetResolution(1664, 936, Screen.fullScreen);
                    break;
                case "1280 x 720":
                    Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
                case "1024 x 576":
                    Screen.SetResolution(1024, 576, Screen.fullScreen);
                    break;
                case "640 x 360":
                    Screen.SetResolution(640, 360, Screen.fullScreen);
                    break;

            }
        }
    }
}
