using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hand
{
    // Timer when recording
    public float timer;
    // Num of children of hand game object
    public int numChildren;

    public List<string> handChildrenName = new List<string>();

    public List<Vector3> handChildrenPos = new List<Vector3>();
    public List<Quaternion> handChildrenRotation = new List<Quaternion>();
}
