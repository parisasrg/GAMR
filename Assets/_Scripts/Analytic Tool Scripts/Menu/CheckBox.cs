using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBox : MonoBehaviour
{
    public bool checkboxed = false;

    public void UpdateCheckBox()
    {
        checkboxed = !checkboxed;
    }
}
