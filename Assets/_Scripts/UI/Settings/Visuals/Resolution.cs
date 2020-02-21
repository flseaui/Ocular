using Michsky.UI.Zone;
using UnityEngine;

namespace UI.Settings.Visuals
{
    public class Resolution : MonoBehaviour
    {
        private void Start()
        {
            if (PlayerPrefs.HasKey("GameRes"))
            {
                GetComponent<HorizontalSelector>().SetIndex(PlayerPrefs.GetInt("GameRes"));
            }
        }
        
        public void Change()
        {
            switch (GetComponent<HorizontalSelector>().CurrentElement)
            {
                case "7680 x 4320 (8K)":
                    PlayerPrefs.SetInt("GameRes", 0);
                    Screen.SetResolution(7680, 4320, Screen.fullScreen);
                    break;
                case "5120 x 2880 (5K)":
                    PlayerPrefs.SetInt("GameRes", 1);
                    Screen.SetResolution(5120, 2880, Screen.fullScreen);
                    break;
                case "3840 x 2160 (4K)":
                    PlayerPrefs.SetInt("GameRes", 2);
                    Screen.SetResolution(3840, 2160, Screen.fullScreen);
                    break;
                case "2560 x 1440":
                    PlayerPrefs.SetInt("GameRes", 3);
                    Screen.SetResolution(2560, 1440, Screen.fullScreen);
                    break;
                case "1920 x 1080":
                    PlayerPrefs.SetInt("GameRes", 4);
                    Screen.SetResolution(1920, 1080, Screen.fullScreen);
                    break;
                case "1664 x 936":
                    PlayerPrefs.SetInt("GameRes", 5);
                    Screen.SetResolution(1664, 936, Screen.fullScreen);
                    break;
                case "1280 x 720":
                    PlayerPrefs.SetInt("GameRes", 6);
                    Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
                case "1024 x 576":
                    PlayerPrefs.SetInt("GameRes", 7);
                    Screen.SetResolution(1024, 576, Screen.fullScreen);
                    break;
                case "640 x 360":
                    PlayerPrefs.SetInt("GameRes", 8);
                    Screen.SetResolution(640, 360, Screen.fullScreen);
                    break;

            }
        }
    }
}
