using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Range(0,1)]
    public float fillAmount = 1;
    public Transform UIPos;
    public GameObject UIPrefab;
    Image healthSlider;
    CharacterStats playerStat;


    void Awake()
    {
        playerStat = GetComponent<CharacterStats>();
        playerStat.OnHealthChanged += OnHealthChanged;

        healthSlider = Instantiate(UIPrefab, UIPos).transform.GetChild(0).GetComponent<Image>();
    }


    void OnHealthChanged(int maxHealth, int currentHealth, int lastcurrenthealth)
    {
        if(healthSlider != null)
        {
            StartCoroutine(HealthChangeProgress(maxHealth, currentHealth, lastcurrenthealth));
            if(currentHealth <= 0)
            {
                Destroy(healthSlider.gameObject);
            }
        }
    }

    IEnumerator HealthChangeProgress(int maxHealth, int currentHealth, int lastcurrenthealth)
    {
        for(int x = lastcurrenthealth; x >= currentHealth; x--)
        {
            lastcurrenthealth = x;
            float healthPrecent = (float) lastcurrenthealth / maxHealth;
            healthSlider.fillAmount = healthPrecent;
            // healthSlider.transform.localScale = new Vector3(healthPrecent,1,1);

            yield return new WaitForSeconds(0.01f);
        }
    }
}
