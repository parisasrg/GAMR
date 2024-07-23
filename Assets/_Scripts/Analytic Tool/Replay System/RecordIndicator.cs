using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordIndicator : MonoBehaviour
{
    public static RecordIndicator recordindicator;
    public Image[] indicators;
    bool blink = false;

    private void Awake() {
        if(recordindicator == null)
        {
            recordindicator = this;
        }

        StopBlink();
    }

    public IEnumerator Blink(Image[] tempindicate)
    {
        while(blink)
        {
            // indicator.SetActive(!indicator.activeSelf);
            foreach(Image indicator in tempindicate)
            {
                indicator.enabled = !indicator.enabled;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void StartBlink()
    {
        blink = true;
        StartCoroutine(Blink(indicators));
    }

    public void StopBlink()
    {
        blink = false;
        StopCoroutine(Blink(indicators));

        foreach(Image indicator in indicators)
        {
            indicator.enabled = false;
        }
    }
}
