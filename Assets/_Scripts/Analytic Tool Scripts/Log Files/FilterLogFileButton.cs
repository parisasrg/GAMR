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
