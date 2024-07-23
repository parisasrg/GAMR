using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBake : MonoBehaviour
{
    // public LayerMask ground;

    private void Awake() {
        if(!GetComponent<NavMeshSurface>())
        {
            this.gameObject.AddComponent<NavMeshSurface>();

            this.gameObject.GetComponent<NavMeshSurface>().layerMask = LayerMask.GetMask("Ground", "Boxes");
            this.gameObject.GetComponent<NavMeshSurface>().ignoreNavMeshObstacle = false;
            this.gameObject.GetComponent<NavMeshSurface>().useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            this.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
