using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class PlayFmodSound : MonoBehaviour
{
    public void PlaySound(string FMOD_Event)
    {

        RuntimeManager.PlayOneShot(FMOD_Event, transform.position);
    }
}