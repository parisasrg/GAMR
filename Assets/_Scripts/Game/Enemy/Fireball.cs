using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int damage = 5;
    PlayerStats playerStat;

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);     
        }
        else if(col.gameObject.CompareTag("Player"))
        {
            playerStat = PlayerManager.instance.player.GetComponent<PlayerStats>();
            playerStat.currentHealth -= damage;

            playerStat.HealthChanged();
        }
    }
}
