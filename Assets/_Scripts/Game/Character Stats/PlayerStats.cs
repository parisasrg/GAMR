using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats
{
    public bool isDead = false;

    public event System.Action<int, int> OnHealthChanged;

    public GameObject gotHitScreen;

    public override void TakeDamage(int damage)
    {
        // damage -= armor.GetValue();
        // damage = Mathf.Clamp(damage, 0, int.MaxValue);

        // if(damageDot <= 0.2f)
        // {
        //     currentHealth -= 0;
        //     Debug.Log(transform.name + " takes 0 damage.");
        // }
        // else if(damageDot > 0.2 && damageDot <= 0.5)
        // {
        //     currentHealth -= Mathf.RoundToInt((float)damage*0.5f);
        //     Debug.Log(transform.name + " takes " + Mathf.RoundToInt((float)damage*0.5f) + " damage.");
        // }
        // else if(damageDot > 0.5)
        // {
        //     currentHealth -= damage;
        //     Debug.Log(transform.name + " takes " + damage + " damage.");
        // }        

        // HealthChanged();

        base.TakeDamage(damage);
        GotHurt();
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

        Destroy(gameObject);
        LevelManager.manager.gameOver = true;
    }

    public void IncreaseHealth(int heal)
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

    void GotHurt()
    {
        var color = gotHitScreen.GetComponent<Image>().color;
        color.a = 0.8f;

        gotHitScreen.GetComponent<Image>().color = color;
    }

    private void Update() 
    {
        if(gotHitScreen != null)
        {
            if(gotHitScreen.GetComponent<Image>().color.a > 0)
            {
                var color = gotHitScreen.GetComponent<Image>().color;
                color.a -= 0.01f;

                gotHitScreen.GetComponent<Image>().color = color;
            }

        }
    }
}
