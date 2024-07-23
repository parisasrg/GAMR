using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using MRTKExtensions.Gesture;
using UnityEngine;

public class WeaponInteractable : MonoBehaviour, IMixedRealityTouchHandler
{
    // [SerializeField]
    private GameObject weaponBody;
    private GameObject prevweaponBody;
    bool grabbed;


    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        // touched = true;

        Debug.Log(this.gameObject.transform.name);

        if(this.gameObject.GetComponent<RadialView>() && this.gameObject.GetComponent<SolverHandler>())
        {
            weaponBody = this.gameObject;
        }
        else if(this.gameObject.GetComponentInChildren<RadialView>() && this.gameObject.GetComponentInChildren<SolverHandler>())
        {
            weaponBody = this.gameObject.transform.GetChild(0).gameObject;
        }

        // Getting rid of the gravity
        if(this.gameObject.GetComponent<Rigidbody>())
        {
            Destroy(this.gameObject.GetComponent<Rigidbody>());
        }
    }
    public void OnTouchCompleted(HandTrackingInputEventData eventData) {}
    public void OnTouchUpdated(HandTrackingInputEventData eventData) { }

    private void FixedUpdate() 
    {
        if(weaponBody)
        {

            if(GestureUtils.IsGrabbing(Handedness.Right) && (weaponBody.GetComponent<WeaponVisibility>().isVisible || !weaponBody.GetComponent<WeaponVisibility>().isVisible))
            {
                this.weaponBody.GetComponent<RadialView>().enabled = true;
                this.weaponBody.GetComponent<SolverHandler>().enabled = true;

                if(weaponBody.transform.parent && weaponBody.transform.parent.name == transform.name)
                {
                    weaponBody.transform.parent.position = weaponBody.transform.position;
                    weaponBody.transform.parent.rotation = weaponBody.transform.rotation;
                }
            }  
            else if(!GestureUtils.IsGrabbing(Handedness.Right) && weaponBody.GetComponent<WeaponVisibility>().isVisible)
            {          
                weaponBody.GetComponent<RadialView>().enabled = false;
                weaponBody.GetComponent<SolverHandler>().enabled = false;
            }
        }
    }
}

