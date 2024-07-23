using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionTracker : MonoBehaviour
{
    public List<Vector3> positions = new List<Vector3>();
    public List<FOVCamera> points = new List<FOVCamera>();

    public void ResetPositions()
    {
        positions.Clear();
    }
}
