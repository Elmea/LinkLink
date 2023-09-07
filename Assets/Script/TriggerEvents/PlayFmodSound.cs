using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class PlayFmodSound : MonoBehaviour
{
    public FMODAsset A_EventSound;
        
    public void PlaySound (FMODAsset A_EventSound) {
            if(A_EventSound != null)
            {
                FMOD_StudioSystem.instance.PlayOneShot(A_EventSound, transform.position);
            }
            else
            {
                Debug.LogError("EventSound is null");
            }
            
        }
}
