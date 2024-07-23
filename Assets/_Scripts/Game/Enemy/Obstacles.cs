using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Obstacles : MonoBehaviour
{
    public GameObject cube;
    private int VertexIndex;
    public int num = 10;
    // Start is called before the first frame update
    void Start()
    {
        NavMeshTriangulation Triangulation = NavMesh.CalculateTriangulation();

        NavMeshHit hit;

        for(int i=0; i <= num; i++)
        {
            VertexIndex = Random.Range(0, Triangulation.vertices.Length);

            if(NavMesh.SamplePosition(Triangulation.vertices[VertexIndex], out hit, 2f, -1))
            {
                Instantiate(cube, hit.position, Quaternion.identity);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
