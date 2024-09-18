using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource adsrcDrone;
    public AudioClip sfxDS_TakeOff, sfxDS_Steady_Up, sfxDS_Up, sfxDS_Up_Steady, sfxDS_Steady_Down, sfxDS_Down, sfxDS_Down_Steady, sfxDS_Steady1, sfxDS_Steady2, sfxDS_Move, sfxDS_Land;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void sTakeOff()
    {
        adsrcDrone.clip = sfxDS_TakeOff;
        adsrcDrone.Play();
    }
}
