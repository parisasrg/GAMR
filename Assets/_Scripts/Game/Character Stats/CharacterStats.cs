using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Stat damage;
    public Stat armor;

    public float damageDot;

    public event System.Action<int, int> OnHealthChanged;

    void Awake() 
    {
        currentHealth = maxHealth;
    }

    public void Attack()
    {
        TakeDamage(10);
    }

    public virtual void TakeDamage (int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage.");

        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die ()
    {
        Debug.Log(transform.name + " died.");
    }
}
