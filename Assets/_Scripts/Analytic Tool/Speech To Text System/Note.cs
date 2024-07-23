using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using TMPro;

public class Note : MonoBehaviour, IMixedRealityTouchHandler
{
    public GameObject noteUI;
    public TMP_Text noteText;
    public string note;
    Transform cam;

    private void Awake() {
        cam = Camera.main.transform;
        noteUI.SetActive(false);
    }

    public void ToggleNoteOn()
    {
        noteText.text = note;
        noteUI.SetActive(true);
    }

    public void ToggleNoteOff()
    {
        noteUI.SetActive(false);
    }

    void LateUpdate () {
        if (noteUI != null)
        {
            noteUI.transform.forward = cam.forward;
            // noteUI.transform.LookAt(cam, Vector3.back);
            // noteUI.transform.rotation = Quaternion.Rotation(cam.position + noteUI.transform.position);
        }
	}

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        ToggleNoteOn();
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) 
    {
        ToggleNoteOff();
    }
    
    public void OnTouchUpdated(HandTrackingInputEventData eventData) 
    {
        
    }
}
