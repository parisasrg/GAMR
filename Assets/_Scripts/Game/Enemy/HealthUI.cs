using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterStats))]
public class HealthUI : MonoBehaviour {

    public GameObject uiPrefab;
    public Transform target;
    //float visibleTime = 5;

    float lastMadeVisibleTime;
    Transform ui;
    Image healthSlider;
    public Transform cam;
    
	// Use this for initialization
	void Start () {

        ui = Instantiate(uiPrefab, GameObject.FindGameObjectWithTag("Canvas").transform).transform;
        healthSlider = ui.GetChild(0).GetComponent<Image>();
        ui.gameObject.SetActive(true);

        GetComponent<CharacterStats>().OnHealthChanged += OnHealthChanged;
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
                StartCoroutine(InitialiseDestroyUI());
            }
        }
    }

    IEnumerator InitialiseDestroyUI()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(ui.gameObject);
    }

    void LateUpdate () {
        if (ui != null)
        {
            ui.position = target.position;
            ui.forward = -cam.forward;
        }
	}
}
