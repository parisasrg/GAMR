using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class LogFileManager : BaseObjectCollection
{
    public static LogFileManager logManager;
    string datapath;

    List<string> logfiles;
    List<GameObject> logFileButtons;

    public GameObject logfileMenu;
    public GameObject logfilebtn;
    public Transform logfiletarget;

    private void Awake() {
        if(logManager == null)
        {
            logManager = this;
        }

        if (Application.isEditor)
        {
            datapath = Application.dataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            datapath = Application.persistentDataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }

        logfileMenu.SetActive(true);
        logfiles = new List<string>(); 
        logFileButtons = new List<GameObject>();
    }

    private void Start() {
        GetLogFiles();
        AddLogFileButtons();
    }

    protected override void LayoutChildren()
    {

    }

    public void GetLogFiles()
    {
        string [] files = System.IO.Directory.GetFiles(datapath,"*.txt");

        foreach(string file in files)
        {
            logfiles.Add(file);
        }
    }

    public void AddLogFile(string path)
    {
        logfiles.Add(path); 

        DeleteLogFileButtons();

        AddLogFileButtons();
    }

    public void ToggleLogFileMenu()
    {
        logfileMenu.SetActive(!logfileMenu.activeSelf);
        DisplayLogFileNames();
    }

    public void AddLogFileButtons()
    { 
        foreach(string file in logfiles)
        {
            GameObject btn = Instantiate(logfilebtn, logfiletarget.position, transform.rotation);
            btn.transform.SetParent(logfiletarget);
            btn.GetComponent<ButtonConfigHelper>().MainLabelText = Path.GetFileName(file); 
            logFileButtons.Add(btn);    
        } 

        StartCoroutine(InvokeUpdateCollection());
    }

    private IEnumerator InvokeUpdateCollection()
    {
        yield return null;
        logfiletarget.GetComponent<GridObjectCollection>().UpdateCollection();
    }

    public string[] SelectedLogFiles()
    {
        List<string> selectedFiles;

        selectedFiles = new List<string>();

        foreach(GameObject btn in logFileButtons)
        {
            if(btn.GetComponent<CheckBox>().checkboxed)
                selectedFiles.Add(datapath + "/" + btn.GetComponent<ButtonConfigHelper>().MainLabelText);
        }

        return selectedFiles.ToArray();
    }

    public void DeleteLogFileButtons()
    {
        foreach (Transform child in logfiletarget) {
            Destroy(child.gameObject);
        }
        logFileButtons.Clear();
    }

    public void DisplayLogFileNames()
    {
        foreach(string file in logfiles)
        {
            // Debug.Log(Path.GetFileName(file));
        } 
    }
}
