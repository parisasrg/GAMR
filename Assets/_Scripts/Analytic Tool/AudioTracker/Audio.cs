using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Audio : MonoBehaviour
{
    public float timer;
    
    public string audioSourceObject;

    public string audioClip;
    public float audioLength;
    public Vector3 audioPosition;
    public Quaternion audioRotation;
}
