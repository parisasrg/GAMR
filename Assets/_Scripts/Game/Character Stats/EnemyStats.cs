using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Animator enemyAnim;
    public bool isDead;


    private void Awake() 
    {
        enemyAnim = GetComponent<Animator>();
    }

    void OnEnable() 
    {
        currentHealth = maxHealth;
    }

    public override void TakeDamage (int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, damage);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();

        enemyAnim.SetTrigger("Dead1");

        if(!isDead)
            LevelManager.manager.UpdateKillings();

        StartCoroutine(InitialiseDeath());
    }

    IEnumerator InitialiseDeath()
    {
        isDead = true;
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}
