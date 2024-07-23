using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class NavigationBaker : MonoBehaviour {

    private List<NavMeshSurface> surfaces;
    public Transform[] objectsToRotate;

    [SerializeField]
    private GameObject mrps;
    private GameObject sa;
    private int count;

    void Awake() {
        surfaces = new List<NavMeshSurface>();
    }

    // Use this for initialization
    void Update () 
    {
        sa = FindChild(mrps, "OpenXR Spatial Mesh Observer");
        if(sa != null)
        {
            count = sa.transform.childCount;

            for(int i = 0; i < sa.transform.childCount; i++)
            {
                if(sa.transform.GetChild(i).GetComponent<NavMeshSurface>() == null)
                {
                    sa.transform.GetChild(i).gameObject.AddComponent<NavMeshSurface>();
                    // surfaces.Add();
                }   

                sa.transform.GetChild(i).GetComponent<NavMeshSurface>().BuildNavMesh();            
            }


            // for (int j = 0; j < objectsToRotate.Length; j++) 
            // {
            //     objectsToRotate [j].localRotation = Quaternion.Euler (new Vector3 (0, 50*Time.deltaTime, 0) + objectsToRotate [j].localRotation.eulerAngles);
            // }

            // for (int i = 0; i < surfaces.Count; i++) 
            // {
            //     Debug.Log(surfaces[i]);
            //     surfaces [i].BuildNavMesh ();    
            // }
        } 
    }

    private GameObject FindChild(GameObject tpName, string objName)
    {
        for(int i = 0; i < tpName.transform.childCount; i++)
        {
            if(tpName.transform.GetChild(i).name == objName)
            {
                return tpName.transform.GetChild(i).gameObject;
            }

            GameObject tmp = FindChild(tpName.transform.GetChild(i).gameObject, objName);

            if(tmp != null)
            {
                return tmp;
            }
        }

        return null;
    }

}