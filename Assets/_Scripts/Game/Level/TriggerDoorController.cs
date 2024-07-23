using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnim = null;
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(LevelManager.instance.keyLevel == 1)
            {
                doorAnim.Play("OpenDoor", 0, 0.0f);
                StartCoroutine(InitialiseCollider());
                door.GetComponent<MeshCollider>().enabled = true; 
            }
        }
    }

    IEnumerator InitialiseCollider()
    {
        yield return new WaitForSeconds(.1f);    
        gameObject.SetActive(false);     
    }
}
