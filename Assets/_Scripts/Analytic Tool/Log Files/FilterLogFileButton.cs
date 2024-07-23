using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FilterLogFileButton : MonoBehaviour
{
    public MeshRenderer meshrenderer;
    public TextMeshPro btnlabel;

    [HideInInspector]
    public GameObject dataParentObj;

    private void Update() 
    {
        if(this.gameObject.transform.name.Contains("Hands"))
        {
            // Fix this
            if(!this.gameObject.GetComponent<CheckBox>().checkboxed && dataParentObj.transform.GetChild(0).GetChild(1).gameObject.activeSelf == true)
            {
                dataParentObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }
            else if(this.gameObject.GetComponent<CheckBox>().checkboxed && dataParentObj.transform.GetChild(0).GetChild(1).gameObject.activeSelf == false)
            {
                dataParentObj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            if(!this.gameObject.GetComponent<CheckBox>().checkboxed && dataParentObj.activeSelf == true)
            {
                dataParentObj.SetActive(false);
            }
            else if(this.gameObject.GetComponent<CheckBox>().checkboxed && dataParentObj.activeSelf == false)
            {
                dataParentObj.SetActive(true);
            }
        }  
    }
}
