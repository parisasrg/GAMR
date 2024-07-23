using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    Rigidbody rb;
    public int gravityDelay = 5;

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Player"))
        {
            ScoreManager.instance.ChangeScore(coinValue);
            Destroy(this.gameObject);
        }
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(WaitForGravity());
    }

    IEnumerator WaitForGravity()
    {
        rb.useGravity = false;
        yield return new WaitForSeconds(gravityDelay);
        rb.useGravity = true;
    }
}
