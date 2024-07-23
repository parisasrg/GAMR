using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public static CameraTracker camtrack;
    public Camera camera;

    private Vector3 worldSpaceCorner;
    public List<FOVCamera> FOVpoints = new List<FOVCamera>();
    public List<Vector3> FOVpositions = new List<Vector3>();

    private void Awake() {
        if(camtrack == null)
        {
            camtrack = this;
        }
    }

    void Update()
    {
        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 15, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        for (int i = 0; i < 4; i++)
        {
            worldSpaceCorner = camera.transform.TransformVector(frustumCorners[i]);
            Debug.DrawRay(camera.transform.position, worldSpaceCorner, Color.blue);
        }
    }

    public void AddFOVpoint(float timer)
    {
        // calculating the corner points of player's FoV
        Vector3[] frustumCorners = new Vector3[4];
        camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 15, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

        // Adding the point into a list of FoV points
        FOVpoints.Add(new FOVCamera{camName = camera.gameObject.name,
                                    camPos = camera.transform.position, 
                                    camCor1 = camera.transform.TransformVector(frustumCorners[0]),
                                    camCor2 = camera.transform.TransformVector(frustumCorners[1]),
                                    camCor3 = camera.transform.TransformVector(frustumCorners[2]),
                                    camCor4 = camera.transform.TransformVector(frustumCorners[3]),
                                    timer = timer});
        FOVpositions.Add(camera.transform.position);
    }

    public void ReplayFOVData(GameObject fov, FOVCamera points)
    {
        // fov.transform.position = points.camPos;
        // fov.GetComponent<FOVPyramid>().DisplayFOVPyramid(points.camPos, points.camCor1, points.camCor2, points.camCor3, points.camCor4);
    }

    public void ResetPositions()
    {
        FOVpoints.Clear();
        FOVpositions.Clear();
    }
}
