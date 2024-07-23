using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    LevelManager levelManager;

    public bool isDead = false;
    public Animator animator;
    public bool bossDead = false;

    void Start () {
        levelManager = LevelManager.instance;
    }

    public override void Die()
    {
        base.Die();

        animator.SetTrigger("Death");

        if(gameObject.name != "Boss_1" && !isDead)
        {
            levelManager.ObjectiveStatus(gameObject.name);
        }
        else
        {
            bossDead = true;
            levelManager.keyPos = transform.position;
        }
        
        StartCoroutine(InitialiseDeath());
    }

    IEnumerator InitialiseDeath()
    {
        isDead = true;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
