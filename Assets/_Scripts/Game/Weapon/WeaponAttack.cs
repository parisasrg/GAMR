using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    PlayerManager playerManager;

    public float power;
    bool isAttacking = false;
    
    
    void Start () {
        playerManager = PlayerManager.instance;
    }


    public void OnAttack()
    {
        isAttacking = true;
    }

    public void OnTriggerEnter(Collider col)
    {
       if(col.tag == "Enemy" && isAttacking == true)
        {
            CharacterCombat playerCombat = playerManager.player.GetComponent<CharacterCombat>();
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            playerCombat.Attack(enemy);
            isAttacking = false;
        }
    }
}