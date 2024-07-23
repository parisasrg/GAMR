using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBehavior : MonoBehaviour
{
    CanvasGroup cvg;

    private void Awake() {
        cvg = GetComponent<CanvasGroup>();    
        cvg.transform.gameObject.SetActive(false);
        // FadeOut();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(cvg, cvg.alpha, 5f));
    }

    public void FadeOut()
    {   cvg.transform.gameObject.SetActive(true);
        StartCoroutine(FadeCanvasGroup(cvg, cvg.alpha, 0));
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 0.5f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentComplete = timeSinceStarted / lerpTime;

        while(true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentComplete);
            // cg.transform.localPosition = Vector3.Lerp(cg.transform.position, Vector3.zero, percentComplete);

            cg.alpha = currentValue;

            if(percentComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }
}
