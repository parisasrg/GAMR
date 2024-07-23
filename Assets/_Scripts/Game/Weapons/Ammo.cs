using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class Ammo : MonoBehaviour, IMixedRealityTouchHandler
{
    public GameObject gun;
    public AudioClip pickItemClip;

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        gun.GetComponent<Gun>().gunData.magStored += gun.GetComponent<Gun>().gunData.magSize;
        gun.GetComponent<Gun>().ChangeText();
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
