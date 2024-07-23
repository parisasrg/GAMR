using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterLogFilesManager : MonoBehaviour
{
    public static FilterLogFilesManager filtermanager;

    public List<FilterLogFiles> filters;

    private void Awake() 
    {
        if(filtermanager == null )
        {
            filtermanager = this;
        }
    }

    public void ReportFilters()
    {
        foreach(FilterLogFiles filter in filters)
        {
            filter.UpdateFilterButtons();
        }
    }
}
