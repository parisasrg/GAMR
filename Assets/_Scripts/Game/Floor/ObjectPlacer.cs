using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToPlace;

    [SerializeField]
    private List<GameObject> objectToActivate;

    private void Awake() {
        objectToPlace.SetActive(false);
    }

    public void PlaceObject(Vector3 location)
    {
        objectToPlace.SetActive(true);
        // objectToPlace.transform.position = location;
        // objectToPlace.transform.forward = Camera.main.transform.forward;
        // objectToPlace.transform.position = new Vector3(-5.57420015f, location.y, -1.19920003f);
        objectToPlace.transform.position = new Vector3(objectToPlace.transform.position.x, location.y, objectToPlace.transform.position.z);

        if(objectToActivate.Count != 0)
        {
            foreach(GameObject obj in objectToActivate)
            {
                obj.SetActive(true);
            }
        }
        
        if(!objectToPlace.GetComponent<NavMeshSurface>())
        {
            objectToPlace.AddComponent<NavMeshSurface>();
            objectToPlace.GetComponent<NavMeshSurface>().BuildNavMesh();
        }   
    }
}