using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Food : MonoBehaviour, IMixedRealityTouchHandler
{
    public int heal = 0;
    public AudioClip pickItemClip;

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        PlayerManager.instance.player.GetComponent<PlayerStats>().IncreaseHealth(heal);
        GetComponent<AudioSource>().clip = pickItemClip;
        GetComponent<AudioSource>().Play();
        // GetComponent<AudioSource>().PlayOneShot(pickItemClip);
        StartCoroutine(InitialiseDestroy());
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) {}
    public void OnTouchUpdated(HandTrackingInputEventData eventData) {}

    IEnumerator InitialiseDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
