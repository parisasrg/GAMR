using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockBossFight : MonoBehaviour
{
    LevelManager levelManager;

    float transitionSpeed = .5f;

    public GameObject boss1;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = LevelManager.instance; 

        boss1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;  
    }

    // Update is called once per frame
    void Update()
    {
        if(levelManager.miniBoss1 == true)
        {
            Vector3 newPos = new Vector3(transform.position.x, 2f, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * transitionSpeed);

            boss1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; 
            if(boss1 != null)
            {
                foreach (Behaviour behaviour in boss1.gameObject.GetComponentsInChildren<Behaviour>())
                {
                    behaviour.enabled = true;
                }
            }    
        }
        
    }
}
