using System.Security.AccessControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SESoundData
{
    public SE se;
    public AudioClip audioClip;
    
    [Range(0,1)]
    public float volume = 1;
}
