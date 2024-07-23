using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public string name;

    public Vector3 position;
    public Quaternion rotation;

    public float timer;
}

public class ObjectsPositions
{
    public string prefabName;
    public List<Position> positions;

    public ObjectsPositions()
    {
        this.positions = new List<Position>();
    }
}

[System.Serializable]
public class Objects
{
    public string prefabName;
    public GameObject gameobj;
}
