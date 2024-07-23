using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// using UnityEditor.Scripting.Python;

public class HeatmapPython : MonoBehaviour
{
    public static HeatmapPython hp;

    private string dataPath;

    public GameObject heatmapObject;
    public GameObject heatmapGuide;

    private void Awake() {
        if(hp == null)
        {
            hp = this;
        }

        heatmapObject.SetActive(false);
        heatmapGuide.SetActive(false);

        if (Application.isEditor)
        {
            dataPath = Application.dataPath +"/ReplayData";
        }
        else
        {
            dataPath = Application.persistentDataPath +"/ReplayData";
        }
    }

    public void ToggleHeatmap()
    {
        heatmapObject.SetActive(!heatmapObject.activeSelf);
        heatmapGuide.SetActive(!heatmapGuide.activeSelf);
    }
}
