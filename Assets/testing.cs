using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 dir = (target.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(forward, dir);

        Debug.Log(dot);
        
    }
}
