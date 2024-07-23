using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using System.IO;
using TMPro;

public class FilterLogFiles : MonoBehaviour
{
    public GameObject dataParentObject;

    // Loaded data
    int dataChildrenCount;
    List<GameObject> dataChildren;
    List<SpriteRenderer> filterDataRenderer;

    // Enable/Disable loaded data button
    public Transform enableDataBtnPos;
    public GameObject enableDataBtn;

    // Color hue
    public LineRendererComponent lr;
    List<float> hues;

    bool listUpdated = false;

    private void Update() 
    {
        if(this.gameObject.activeSelf == true && !listUpdated)
        {
            StartCoroutine(InvokeUpdateCollection());
            listUpdated = true;
            StartCoroutine(InvokeUpdateCollection());
        }
        else if(this.gameObject.activeSelf == false && listUpdated)
        {
            listUpdated = false;
        }
    }

    public void UpdateFilterButtons()
    {
        dataChildrenCount = dataParentObject.transform.childCount;

        hues = lr.HueCalculator(dataChildrenCount, lr.huevalues);

        if(dataChildrenCount != 0)
        {
            for(int i=0; i < dataChildrenCount; i++)
            {
                GameObject filterbtn = Instantiate(enableDataBtn, enableDataBtnPos.position, Quaternion.identity);
                filterbtn.transform.SetParent(enableDataBtnPos);
                filterbtn.transform.name = dataParentObject.transform.GetChild(i).transform.name + "Button";
                filterbtn.GetComponent<FilterLogFileButton>().meshrenderer.materials[0].color = Color.HSVToRGB(hues[i], 1, 1);
                filterbtn.GetComponent<FilterLogFileButton>().btnlabel.text = dataParentObject.transform.GetChild(i).transform.name;
                filterbtn.GetComponent<FilterLogFileButton>().dataParentObj = dataParentObject.transform.GetChild(i).transform.gameObject;
            }
        }
    }

    private IEnumerator InvokeUpdateCollection()
    {
        yield return null;
        enableDataBtnPos.GetComponent<GridObjectCollection>().UpdateCollection();
        this.gameObject.GetComponent<GridObjectCollection>().UpdateCollection();
    }
}
