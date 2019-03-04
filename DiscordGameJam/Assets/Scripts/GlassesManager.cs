using System;
using UnityEngine;
using UnityEngine.UI;

public class GlassesManager : Singleton<GlassesManager>
{
    public enum GlassesColor
    {
        Red,
        Green,
        Blue,
        Yellow,
        Magenta,
        Cyan,
        White,
        Black
    }

    public Image RedIndicator,
        GreenIndicator,
        BlueIndicator;
    

    private int _numOfGlasses;

    public GameObject Player;

    [NonSerialized] public bool BlueGlasses;

    public GlassesColor Color;

    [NonSerialized] public bool GreenGlasses;

    public Action<GlassesColor> OnGlassesSwitched;

    [NonSerialized] public bool RedGlasses = true;

    private GlassesColor CalculateColor()
    {
        if (RedGlasses && GreenGlasses && BlueGlasses) return GlassesColor.White;
        if (RedGlasses && GreenGlasses) return GlassesColor.Yellow;
        if (RedGlasses && BlueGlasses) return GlassesColor.Magenta;
        if (GreenGlasses && BlueGlasses) return GlassesColor.Cyan;
        if (GreenGlasses) return GlassesColor.Green;
        if (BlueGlasses) return GlassesColor.Blue;
        if (RedGlasses) return GlassesColor.Red;
        return GlassesColor.Black;
    }

    public void Reload()
    {
      //  _glassesFilter.Reset();
        RedGlasses = true;
        GreenGlasses = false;
        BlueGlasses = false;
        GreenIndicator.GetComponent<Image>().color = new Color(0, 255, 0, .25f);
        BlueIndicator.GetComponent<Image>().color = new Color(0, 0, 255, .25f);
        RedIndicator.GetComponent<Image>().color = new Color(255, 0, 0, 1);
        OnGlassesSwitched?.Invoke(CalculateColor());
    }
    
    private void Start()
    {
        Reload();
    }

    private void Update()
    {
        
        
        _numOfGlasses =
            (RedGlasses ? 1 : 0) +
            (BlueGlasses ? 1 : 0) +
            (GreenGlasses ? 1 : 0);

        if (Input.GetKeyDown(KeyCode.Q))
            RedToggle();
        if (Input.GetKeyDown(KeyCode.W))
            GreenToggle();
        if (Input.GetKeyDown(KeyCode.E))
            BlueToggle();
    }

    public void CheckDead(Vector3 pos)
    {
        if (pos == Player.GetComponent<PathManager>().FindClosestWaypoint(Player.transform.position).transform
                .position)
            LevelManager.Instance.RestartLevel();
    }


    public void RedToggle()
    {
        if (!(RedGlasses && _numOfGlasses == 1 || !RedGlasses && _numOfGlasses == 2))
        {
            RedGlasses = !RedGlasses;
           // _glassesFilter.SetRedFilter(RedGlasses);
            OnGlassesSwitched?.Invoke(CalculateColor());
            RedIndicator.GetComponent<Image>().color = new Color(255, 0, 0, RedGlasses ? 1 : .25f);
        }
    }

    public void GreenToggle()
    {
        if (!(GreenGlasses && _numOfGlasses == 1 || !GreenGlasses && _numOfGlasses == 2))
        {
            GreenGlasses = !GreenGlasses;
           // _glassesFilter.SetGreenFilter(GreenGlasses);
            OnGlassesSwitched?.Invoke(CalculateColor());
            GreenIndicator.GetComponent<Image>().color = new Color(0, 255, 0, GreenGlasses ? 1 : .25f);
        }
    }

    public void BlueToggle()
    {
        if (!(BlueGlasses && _numOfGlasses == 1 || !BlueGlasses && _numOfGlasses == 2))
        {
            BlueGlasses = !BlueGlasses;
            //_glassesFilter.SetBlueFilter(BlueGlasses);
            OnGlassesSwitched?.Invoke(CalculateColor());
            BlueIndicator.GetComponent<Image>().color = new Color(0, 0, 255, BlueGlasses ? 1 : .25f);
        }
    }
}