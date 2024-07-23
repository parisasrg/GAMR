using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hand
{
    public float timer;
    public int numChildren;

    public List<string> handChildrenName = new List<string>();

    public List<Vector3> handChildrenPos = new List<Vector3>();
    public List<Quaternion> handChildrenRotation = new List<Quaternion>();
}
