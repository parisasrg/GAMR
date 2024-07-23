using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    public GameObject aimObj;

    private LineRenderer aimLine;
    Vector3[] aimPoints;

    private void Awake() 
    {
        aimPoints = new Vector3[2];
        aimLine = GetComponent<LineRenderer>();

        aimLine.startWidth = 0.005f;
        aimLine.endWidth = 0.005f;

        // aimPoints.SetValue(transform.position, 0);
        // aimPoints.SetValue(aimObj.transform.position, 1);
    }

    private void Start() 
    {
        // aimLine.SetPosition(0, aimPoints[0]);
        // aimLine.SetPosition(1, aimPoints[0]);
    }

    private void FixedUpdate() 
    {
        aimPoints.SetValue(transform.position, 0);
        if(aimObj.activeSelf)
        {
            aimPoints.SetValue(aimObj.transform.position, 1);
        }
        else
        {
            aimPoints.SetValue(aimObj.transform.position, 1);
        }
    }

    private void Update() 
    {
        aimLine.SetPositions(aimPoints);
    }
}
