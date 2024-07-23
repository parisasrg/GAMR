using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public bool isDead = false;
    public Animator animator;

    public event System.Action<int, int> OnHealthChanged;

    public int heal = 0;

    public override void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        if(damageDot <= 0.2f)
        {
            currentHealth -= 0;
            Debug.Log(transform.name + " takes 0 damage.");
        }
        else if(damageDot > 0.2 && damageDot <= 0.5)
        {
            currentHealth -= Mathf.RoundToInt((float)damage*0.5f);
            Debug.Log(transform.name + " takes " + Mathf.RoundToInt((float)damage*0.5f) + " damage.");
        }
        else if(damageDot > 0.5)
        {
            currentHealth -= damage;
            Debug.Log(transform.name + " takes " + damage + " damage.");
        }        

        HealthChanged();
    }

    public void HealthChanged()
    {
        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        base.Die();

        isDead = true;

        animator.SetTrigger("Death");
        
        LevelManager.instance.GameOver();
        Destroy(gameObject);
    }

    public void IncreaseHealth()
    {
        if((currentHealth + heal) < maxHealth)
        {
            currentHealth += heal;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currentHealth);
        }
    }
}
