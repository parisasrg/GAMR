using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    // Player
    public GameObject player;
    PlayerStats playerStats;

    //First aid kit
    public GameObject firstAid;
    public int firstAidUnlock = 0;
    public Transform firstAidTarget;
    public bool firstAidExist = false;

    public static PlayerManager instance;

    void Awake() {
        instance = this;
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void KillPlayer()
    {
        SceneManager.LoadScene(0);
    }

    public void Update()
    {
        if(playerStats.currentHealth <= firstAidUnlock && !firstAidExist)
        {
            Instantiate(firstAid, firstAidTarget);
            firstAidExist = true;
        }
    }
}
