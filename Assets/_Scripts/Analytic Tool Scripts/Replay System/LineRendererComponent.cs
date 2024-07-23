using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererComponent : MonoBehaviour
{
    private LineRenderer lineRenderer;
    ReplaySystem replaySystem;

    // [Range(0, 0.9f)]
    // public float hue;
    public List<float> huevalues;
    private float saturation = 1;
    private float brightness = 1;

    private void Awake() {
        replaySystem = GetComponent<ReplaySystem>();

        // Calculating possible hue values
        huevalues = new List<float>();

        for (float value = 0; value <= 0.9f; value += 0.3f)
        {
            huevalues.Add(Mathf.Round(value * 3f) / 3f);
        }
    }
    
    // Start is called before the first frame update
    public void resetLineRenderer(){
        lineRenderer.positionCount = 0;
    }

    void setLineRenderer(float huevalue){
        Material mat = new Material(Shader.Find("Standard"));
        mat.SetColor("_Color",Color.HSVToRGB(huevalue, saturation, brightness));

        lineRenderer.material = mat;

        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
    }

    public void LineRendererComponentFn(float hue, GameObject playerPrefab){
        lineRenderer = playerPrefab.GetComponent<LineRenderer>();

        setLineRenderer(hue);

        if(replaySystem.loadLineAtOnce)
        {
            lineRenderer.positionCount = replaySystem.load_positions.Count;

            for (int i = 0; i < replaySystem.load_positions.Count; i++ )
            {
                lineRenderer.SetPosition(i, replaySystem.load_positions[i]);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }   
    }

    public void AnimateLine(int i, LineRenderer lr, List<Vector3> load_positions)
    {
        lr.positionCount = i;
        Vector3 pos = new Vector3(load_positions[i].x, load_positions[i].y, load_positions[i].z);
        lr.SetPosition(i-1, pos);
    }


    public List<float> HueCalculator(int num, List<float> hues)
    {
        var tempHue = new List<float>();
        int step = 0;

        for(int i = 0; i < num; i++)
        {
            if(i == 0)
            {
                tempHue.Add(hues[0]);
            }
            else if(i % 2 == 0)
            {
                tempHue.Add(hues[step]);
            }
            else
            {
                ++step;
                tempHue.Add(hues[hues.Count - step]);
            }
        }

        return tempHue;
    }

}
