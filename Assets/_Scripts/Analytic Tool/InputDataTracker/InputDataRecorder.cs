using System;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.SceneManagement;

public class InputDataRecorder : MonoBehaviour
{
    // Singleton
    public static InputDataRecorder inputtracker;

    public bool recordInputData = false;

    [SerializeField] private List<InputAction> actions;

    public GameObject inputDataParentObject;
    public GameObject inputDataPrefab;
    GameObject player;
    float timer;

    List<InputData> inputDatas = new List<InputData>();
    List<InputData> inputLoadDatas = new List<InputData>();

    string dataPath;


    LineRendererComponent lr;

    private void Awake() 
    {
        if(inputtracker == null)
        {
            inputtracker = this;
        }

        if (Application.isEditor)
        {
            dataPath = Application.dataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            dataPath = Application.persistentDataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }
    }

    private void Start()
    {
        // action.Enable();

        lr = GetComponent<LineRendererComponent>();

        foreach(InputAction action in actions)
        {
            action.Enable();
        }

        player = ReplaySystem.rs.Player;


        // testing
        // LoadInputData();
    }

    private void Update()
    {
        foreach(InputAction action in actions)
        {
            if(action.triggered)
            {
                Debug.Log(action.ToString());
                RecordActionData(action);
            }
        }
    }


    public void RecordActionData(InputAction action)
    {
        if(recordInputData)
        {
            if(ReplaySystem.rs.isRecord && !ReplaySystem.rs.isReplaying)
            {
                timer += Time.deltaTime;

                if (action.triggered && player != null)
                {
                    InputData newinput = new InputData();

                    newinput.timer = timer;
                    newinput.action = action.ToString();
                    newinput.position = player.transform.position;
                    newinput.rotation = player.transform.rotation;

                    inputDatas.Add(newinput);
                }
            }
        }
    }

    public void SaveInputData()
    {
        // Check if ReplayData folder exists
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        // Get current time and place into logfile's name
        string time = GetCurrentDateTime();
        // Creating logfile
        string path = dataPath + "/ReplayInputData{currentdatetime}.txt".Replace("{currentdatetime}", time);

        StreamWriter writer = new StreamWriter(path);

        if(inputDatas.Count != 0)
        {
            writer.WriteLine("OBJ_START~Input");
            foreach(InputData input in inputDatas){
                writer.WriteLine("\t\t" + input.timer + "~" 
                                + input.action + "~"
                                + "(" + Mathf.Round(input.position.x *100f)*0.01f + "," + Mathf.Round(input.position.y *100f)*0.01f + "," + Mathf.Round(input.position.z *100f)*0.01f + ")"
                                + "(" + Mathf.Round(input.rotation.x *100f)*0.01f + "," + Mathf.Round(input.rotation.y *100f)*0.01f + "," + Mathf.Round(input.rotation.z *100f)*0.01f + "," + Mathf.Round(input.rotation.w *100f)*0.01f + ")");    
            }
            writer.WriteLine("OBJ_END~Input");
        }

        // Adding new log file path to log files
        LogFileManager.logManager.AddLogFile(path);

        writer.Close();
        ResetInputData();
    }

    public void LoadInputData(){
        // Get selected log files
        // string[] inputfiles = LogFileManager.logManager.SelectedLogFiles();        

        // Get all log files in ReplayData folder
        string[] files = Directory.GetFiles(dataPath,"ReplayInputData*.txt");

        // Ignore hidden lines in log file
        bool endObject = false;

        int filesCount = Mathf.Clamp(files.Length,1,10);
        List<float> hues = lr.HueCalculator(filesCount, lr.huevalues);
        int fileCounter = 0;

        foreach(string file in files)
        {
            int i=1;
            StreamReader stream = new StreamReader(file);
            string lineReader = stream.ReadToEnd();
            string[] lines = lineReader.Split('\n');
            
            foreach(string line in lines)
            {
                if(line != "")
                {
                    // Start by removing the \r at end of the line
                    string Trimmedline = line.TrimEnd(new char[] {'\r'});

                    // Also remove the \t at start of the line
                    Trimmedline = line.TrimStart(new char[] {'\t'});

                    string[] tokens = Trimmedline.Split('~');

                    if(tokens[0] == "OBJ_START")
                    {
                        endObject = false;
                    }
                    else if(tokens[0] != "OBJ_END" && !endObject)
                    {
                        InputData loaddata  = new InputData();
                        loaddata.timer = float.Parse(tokens[0]);
                        loaddata.action = tokens[1];
                        string tmpvalue = tokens[2].Replace("(", ",").Replace(")", "");

                        string[] tmpvalues = tmpvalue.Split(',');

                        if(i <= lineReader.Length && tmpvalues.Length > 0)
                        {
                            ParseInputData(tmpvalues,i, loaddata);  
                        }
                        else
                        {
                            break;
                        }

                        inputLoadDatas.Add(loaddata);
                    }
                    else if(tokens[0] == "OBJ_END")
                    {
                        // End of the object in log file
                        endObject = true;
                    }

                    i++; 
                }
            }

            stream.Close();

            GameObject inputdataparent = new GameObject();
            inputdataparent.transform.name = "Input"+fileCounter;
            inputdataparent.transform.SetParent(inputDataParentObject.transform);

            if(inputLoadDatas != null)
            {
                foreach(InputData data in inputLoadDatas)
                {
                    GameObject inputdata = Instantiate(inputDataPrefab, data.position, data.rotation);
                    inputdata.transform.SetParent(inputdataparent.transform);

                    inputdata.GetComponent<MeshRenderer>().materials[0].color = Color.HSVToRGB(hues[fileCounter], 0.8f, 1);

                    // Not ideal and correct (Only works for this game)
                    if(data.action.Contains("buttonSouth"))
                    {
                        inputdata.GetComponent<DataDisplay>().data = "Button Pressed : " + data.action + "\n \nAction Type: Jump";
                    }
                    if(data.action.Contains("buttonNorth"))
                    {
                        inputdata.GetComponent<DataDisplay>().data = "Button Pressed : " + data.action + "\n \nAction Type: Interact";
                    }
                    if(data.action.Contains("buttonWest"))
                    {
                        inputdata.GetComponent<DataDisplay>().data = "Button Pressed : " + data.action + "\n \nAction Type: Attack";
                        // Play attack animation
                    }
                    if(data.action.Contains("buttonEast"))
                    {
                        inputdata.GetComponent<DataDisplay>().data = "Button Pressed : " + data.action + "\n \nAction Type: Dodge";
                    }
                }

                fileCounter++;
            }

            inputLoadDatas.Clear();
        }
    }

    void ParseInputData(string[] lines,int i, InputData input)
    {
        if(float.TryParse(lines[1],out float x) && float.TryParse(lines[2],out float y) && float.TryParse(lines[3],out float z)){
            if(lines.Length == 8 && lines[1] != null){
                Vector3 newPosition = new Vector3(float.Parse(lines[1]),float.Parse(lines[2]),float.Parse(lines[3]));  
                input.position = newPosition;
                Quaternion newRotation = new Quaternion(float.Parse(lines[4]),float.Parse(lines[5]),float.Parse(lines[6]),float.Parse(lines[7]));
                input.rotation = newRotation;
            }       
        }
    }

    string GetCurrentDateTime()
    {
        DateTime dt = DateTime.Now;

        string date = dt.Year + "-" + dt.Month + "-" + dt.Day;
        string currenttime = date + "T" + dt.Hour + "-" + dt.Minute + "-" + dt.Second;

        return currenttime;
    }

    void ResetInputData()
    {
        inputDatas.Clear();
        timer = 0.0f;
    }

    void OnDestroy()
    {
        if(inputDatas.Count != 0)
        {
            SaveInputData();
        }
    }
}
