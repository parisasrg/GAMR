using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class OkButton : MonoBehaviour, IMixedRealityTouchHandler
{
    [SerializeField]
    private GameObject panel;
    
    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        panel.SetActive(false);
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) {}
    public void OnTouchUpdated(HandTrackingInputEventData eventData) {}
}
