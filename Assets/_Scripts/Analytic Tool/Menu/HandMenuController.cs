using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuController : MonoBehaviour
{
    GameObject parent;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void ToggleMenu(GameObject menu)
    {
        parent = menu.transform.parent.gameObject;

        for(int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(false);
        }

        menu.SetActive(!menu.activeSelf);
    }
}
