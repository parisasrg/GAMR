using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStats))]
public class PlayerHealthUI : MonoBehaviour
{
    public GameObject uiPrefab;
    public Transform target;
    //float visibleTime = 5;

    float lastMadeVisibleTime;
    Transform ui;
    Image healthSlider;
    Transform cam;

    // Start is called before the first frame update
    void Start () {
        cam = Camera.main.transform;

        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if(c.name == "Game Canvas")
            {
                ui = Instantiate(uiPrefab, c.transform).transform;
                healthSlider = ui.GetChild(0).GetComponent<Image>();
                ui.gameObject.SetActive(true);
            break;
            }
        }

        GetComponent<PlayerStats>().OnHealthChanged += OnHealthChanged;
	}

    void OnHealthChanged(int maxHealth, int currentHealth) {
        if (ui != null)
        {
            ui.gameObject.SetActive(true);
            lastMadeVisibleTime = Time.time;

            float healthPercent = (float)currentHealth / maxHealth;
            healthSlider.fillAmount = healthPercent;
            if (currentHealth <= 0)
            {
                Destroy(ui.gameObject);
            }
        }
    }

    void LateUpdate () {
        if (ui != null)
        {
            ui.position = target.position;
            ui.forward = -cam.forward;
        }
	}
}
