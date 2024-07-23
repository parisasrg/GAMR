using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public int spikeDamage = 4;

    GameObject player;

    //Player rigidbody
    Rigidbody playerRb;

    //Player Stat
    PlayerStats playerStat;

    Vector3 move = new Vector3(1, 1, 1);

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Player"))
        {
            //Storing Player game object to access its components
            player = PlayerManager.instance.player;

            //Storing player's rigidbody component
            playerRb = player.GetComponent<Rigidbody>();
            //Throw player away from the spike if it hits a spike
            playerRb.AddForce(-transform.forward * move.z + transform.up * move.y, ForceMode.Impulse);

            //Storing player's Player Stat component
            playerStat = player.GetComponent<PlayerStats>();
            //Damage to player when it hits spikes
            playerStat.currentHealth -= spikeDamage;
            //Update player's health bar shown in Canvas
            playerStat.HealthChanged();
        }
    }
}
