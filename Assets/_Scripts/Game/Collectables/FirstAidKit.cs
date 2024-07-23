using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidKit : MonoBehaviour
{
    PlayerStats playerStats;

    private void Awake() {
        playerStats = PlayerManager.instance.player.GetComponent<PlayerStats>();
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Player"))
        {
            playerStats.IncreaseHealth();
            PlayerManager.instance.firstAidExist = false;
            Destroy(this.gameObject);
        }
    }
}
