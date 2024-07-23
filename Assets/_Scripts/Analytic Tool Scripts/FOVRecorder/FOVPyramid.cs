using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVPyramid : MonoBehaviour
{
    public float height = 1;
    public float width = 1;
    public float length = 1;

    Color matcolor;
    
    public List<MeshRenderer> hololensRenderer;

    public void SetMeshColor(float huevalue)
    {
        matcolor = Color.HSVToRGB(huevalue, 1, 1);

        foreach(MeshRenderer renderer in hololensRenderer)
        {
            renderer.material.color = matcolor;
        }

        GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(matcolor.r, matcolor.g, matcolor.b,0f));
    }

    public void DisplayFOVPyramid(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector3 p5)
    {
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = new Mesh();

        var widthOffset = width * 0.5f;
        var lengthOffset = length * 0.5f;

        var points = new Vector3[] {p1, p2, p3, p4, p5};

        // transform.position = points[0];
        // transform.position = new Vector3(0,0,0);

        mesh.vertices = new Vector3[] {
            points[0], points[1], points[2],
            points[0], points[2], points[3],
            points[0], points[1], points[4],
            points[1], points[2], points[4],
            points[2], points[3], points[4],
            points[3], points[0], points[4]
        };

        mesh.triangles = new int[] {
            0, 1, 2,
            3, 4, 5,
            8, 7, 6,
            11, 10, 9,
            14, 13, 12,
            17, 16, 15
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshFilter.mesh = mesh;
    }
}
