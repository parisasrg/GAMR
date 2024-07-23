using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenIndicator : MonoBehaviour
{
    public Image indicator;
    public GameObject target;

    [HideInInspector]
    public bool isVisible = false;

    Renderer rd;
    Vector3 pos;

    private void Awake() 
    {
        rd = GetComponent<MeshRenderer>();    

        if(indicator)
        {
            indicator.transform.gameObject.SetActive(false);
        }
    }

    private void OnBecameVisible() {
        isVisible = true;
    }

    private void OnBecameInvisible() {
        isVisible = false;
    }
    
    void Update()
    {
        if(isVisible == false)
        {
            if(!indicator.transform.gameObject.activeSelf)
            {
                indicator.transform.gameObject.SetActive(true);
            }

            float minX = -400;
            float maxX = 400;

            float minY = -300;
            float maxY = 115;

            pos = Camera.main.WorldToScreenPoint(target.transform.position);

            if(Vector3.Dot((target.transform.position - transform.position), transform.forward) < 0)
            {
                if(pos.x < Screen.width / 2)
                {
                    pos.x = maxX;
                }
                else
                {
                    pos.x = minX;
                }
            }

            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);

            indicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);
            float angle = Vector3.SignedAngle(Vector3.up, indicator.GetComponent<RectTransform>().anchoredPosition, Vector3.forward);
            indicator.GetComponent<RectTransform>().transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
        else
        {
            if(indicator.transform.gameObject.activeSelf)
            {
                indicator.transform.gameObject.SetActive(false);
            }
        }
    }
}
