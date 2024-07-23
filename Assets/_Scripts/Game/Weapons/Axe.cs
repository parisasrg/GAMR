using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public AudioClip slashAudio;

    bool takeHit = false;

   private void OnCollisionStay(Collision other) {
        if(other.gameObject.tag == "Enemy")
        {
            if(!takeHit)
            {
                other.gameObject.GetComponent<EnemyStats>().TakeDamage(15);

                Debug.Log(takeHit);
            
                other.gameObject.GetComponent<AudioSource>().PlayOneShot(slashAudio);
            }

            takeHit = true;
        }
    }

    private void OnCollisionExit(Collision other) {
        if(takeHit)
        {
            takeHit = false;
        }
    }
}
