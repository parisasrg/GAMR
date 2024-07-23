using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public static Collectable instance;

    public int coinValue = 1;

    Rigidbody rb;
    public int gravityDelay = 5;

    //Collectable audio
    [SerializeField]
    private AudioClip collectClip;
    private AudioSource audioSource;

    private void Awake() {
        if(!instance)
        {
            instance = this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Player"))
        {
            audioSource.clip = collectClip;
            audioSource.Play();
            if(!gameObject.name.Contains("Key"))
            {
                ScoreManager.instance.ChangeScore(coinValue);  
            }
            else
            {            
                LevelManager.instance.keyLevel += 1;
            }
            StartCoroutine(InitialiseDestroy());       
        }
    }

    IEnumerator InitialiseDestroy()
    {
        yield return new WaitForSeconds(.2f);
        Destroy(this.gameObject);
    }
}
