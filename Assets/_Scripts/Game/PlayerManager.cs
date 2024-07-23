using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    // Player
    public GameObject player;
    PlayerStats playerStats;

    // Floor
    public GameObject floor;

    //First aid kit
    // public GameObject firstAid;
    // public int firstAidUnlock = 0;
    // public Transform firstAidTarget;
    // public bool firstAidExist = false;

    public static PlayerManager instance;

    void Awake() {
        instance = this;
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void KillPlayer()
    {
        Destroy(player);
    }

    public void Update()
    {
        // if(playerStats.currentHealth <= firstAidUnlock && !firstAidExist)
        // {
        //     Instantiate(firstAid, firstAidTarget);
        //     firstAidExist = true;
        // }
    }
}
