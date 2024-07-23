using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public static PickUpObject instance;
    public Transform pickuppoint;

    private bool pickedup = false;
    public bool collided = false;

    Rigidbody boxRb;
    BoxCollider boxBc;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }

        boxRb = GetComponent<Rigidbody>();
        boxBc = GetComponent<BoxCollider>();
    }
    
    public void PlayerPickUp()
    {
        if(pickedup)
        {
            boxBc.enabled = true;
            boxRb.useGravity = true;
            boxRb.constraints = RigidbodyConstraints.None;

            this.transform.parent = GameObject.Find("Static Objects").transform;
            
            pickedup = false;
        }
        else if(collided && !pickedup)
        {
            boxBc.enabled = false;
            boxRb.useGravity = false;
            boxRb.constraints = RigidbodyConstraints.FreezeAll; 

            this.transform.position = pickuppoint.position;
            this.transform.parent = GameObject.Find("Pick up target").transform;

            pickedup = true;
        }
    }

    void OnCollisionEnter(Collision other){

        if (other.collider.tag == "Player"){
          collided = true;
        }
    }

    void OnCollisionExit(Collision other){

        if (other.collider.tag == "Player"){
          collided = false;
        }
    }
}
