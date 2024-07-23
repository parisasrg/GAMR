using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using TMPro;

public class DataDisplay : MonoBehaviour, IMixedRealityTouchHandler
{
    public GameObject dataUI;
    public TMP_Text dataText;
    public string data;
    Transform cam;

    public virtual void Awake() {
        cam = Camera.main.transform;
        dataUI.SetActive(false);
    }

    public virtual void ToggleDataOn()
    {
        dataText.text = data;
        dataUI.SetActive(true);
    }

    public virtual void ToggleDataOff()
    {
        dataUI.SetActive(false);
    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        ToggleDataOn();
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) 
    {
        ToggleDataOff();
    }
    
    public void OnTouchUpdated(HandTrackingInputEventData eventData) 
    {
        
    }

    public virtual void LateUpdate () {
        if (dataUI != null)
        {
            dataUI.transform.forward = -cam.forward;
        }
	}
}
