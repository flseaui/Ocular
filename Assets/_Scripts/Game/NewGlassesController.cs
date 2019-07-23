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


public class NewGlassesController : MonoBehaviour
{
    public enum OcularState
    {
        A,    // red
        AB,    // orange
        B,  // yellow
        BC,   // green
        C,   // blue
        AC,     // magenta
    }

    private OcularState[] states =
    {
        OcularState.A,
        OcularState.AB,
        OcularState.B,
        OcularState.BC,
        OcularState.C,
        OcularState.AC
    };
    
    public static Action OnGlassesToggled;

    public OcularState CurrentOcularState;
    
    private int index;
    
    private List<PlaySoundResult> _musicStreams;
    
    private void Awake()
    {
        _musicStreams = new List<PlaySoundResult>();
    }

    private IEnumerator Start()
    {
        _musicStreams.Add(MasterAudio.PlaySound("World1_A"));
        _musicStreams.Add(MasterAudio.PlaySound("World1_B"));
        _musicStreams.Add(MasterAudio.PlaySound("World1_C"));
            
        yield return new WaitForFixedUpdate();
        index = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            index--;
            CurrentOcularState = states[index % 6];
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            index++;
            CurrentOcularState = states[index % 6];
        }
    }
}
