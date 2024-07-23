using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using MRTKExtensions.Gesture;

public class PistolTrigger : MonoBehaviour, IMixedRealityTouchHandler
{
    public GameObject gun;
    public GameObject pistolBody;
    bool isPinched = false;

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        // gun.GetComponent<Gun>().Shoot();
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) {}
    public void OnTouchUpdated(HandTrackingInputEventData eventData) { }

    private void Update() {
        if(pistolBody.GetComponent<RadialView>().enabled && pistolBody.GetComponent<SolverHandler>().enabled && pistolBody.GetComponent<WeaponVisibility>().isVisible)
        {
            if(GestureUtils.IsPinching(Handedness.Right) && !isPinched)
            {
                isPinched = true;
                gun.GetComponent<Gun>().Shoot();
            }
            else if(!GestureUtils.IsPinching(Handedness.Right))
            {
                isPinched = false;
            }
        }    
    }
}
