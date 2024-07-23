using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandLoad : MonoBehaviour
{
    // Test
    public bool enableHandTracker = false;
    public bool enableReplay = false;
    [SerializeField]private float CurrentIndex;


    // Singlton
    public static HandLoad hl;


    // Hand Tracker Parent Object
    public GameObject HandTrackerParent;


    // Hand Prefabs
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;

    Transform[] leftHandChildren;
    Transform[] rightHandChildren;

    public bool recordHandGestures = false;
    private string dataPath;

    GameObject generatedLeftHand;
    GameObject generatedRightHand;



    // Lists of saved hand data
    List<Hand> leftHandData = new List<Hand>();
    List<Hand> rightHandData = new List<Hand>();


    // Lists of loaded hand data
    List<Hand> loadLeftHandData;
    List<Hand> loadRightHandData;
    int loadInnerCount = 0;
    Hand newLoadLeftHand;
    Hand newLoadRightHand;
    bool showHand = true;


    // Instantiate left and right hand mesh GOs
    // Displying the recorded handmeshes during playback
    GameObject leftHandModel;
    GameObject rightHandModel;
    Transform[] leftHandModelChildren;
    Transform[] rightHandModelChildren;

    private float timer;

    string currentHandName;

    LineRendererComponent lr;

    private void Awake() {
        // Creating singleton instance of this class
        if(hl == null)
        {
            hl = this;
        }

        // Initialization of load hand data lists
        loadLeftHandData = new List<Hand>();
        loadRightHandData = new List<Hand>();

        // Initialization of saving path
        if (Application.isEditor)
        {
            dataPath = Application.dataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            dataPath = Application.persistentDataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }

        // Component used to get colors for hands
        lr = GetComponent<LineRendererComponent>();

        // Getting both hands' children transform
        leftHandChildren = GetHandChildren(leftHandPrefab.transform, true);
        rightHandChildren = GetHandChildren(rightHandPrefab.transform, true);
    }

    private void Update() 
    {
        // Activate hand gesture recording?
        // recordHandGestures = handRecorderBtn.GetComponent<CheckBox>().checkboxed;

        if(recordHandGestures)
        {
            generatedLeftHand = null;
            generatedRightHand = null;
            List<GameObject> unidentifiedHands = new List<GameObject>();
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            // Getting hand game objects
            foreach (GameObject go in allObjects)
            {
                if (go.name.Contains("Left_RiggedHandLeft(Clone)") || go.name.Contains("Right_RiggedHandRight(Clone)"))
                {
                    unidentifiedHands.Add(go);
                }
            }
            foreach (GameObject hand in unidentifiedHands)
            { 
                if (hand.name.Contains("Left"))
                {
                    generatedLeftHand = hand;
                }
                else if (hand.name.Contains("Right"))
                {
                    generatedRightHand = hand;
                }
            }


            // Getting and saving hand info when recording is enabled
            if(ReplaySystem.rs.isRecord && !ReplaySystem.rs.isReplaying)
            {
                timer += Time.deltaTime;

                // Adding new left hand data
                if (generatedLeftHand != null)
                {
                    Transform[] generatedLeftHandChildren = null;
                    generatedLeftHandChildren = GetHandChildren(generatedLeftHand.transform, true);

                    if(generatedLeftHandChildren.Length != 0)
                    {
                        Hand newLeftHand = new Hand();

                        newLeftHand.timer = timer;
                        if(generatedLeftHandChildren.Length != 0)
                        {
                            // Adding children names
                            for(int i=0; i < leftHandChildren.Length; i++)
                            {
                                newLeftHand.handChildrenName.Add(generatedLeftHandChildren[i].gameObject.name);
                            }

                            // Adding children postions
                            for(int i=0; i < leftHandChildren.Length; i++)
                            {
                                newLeftHand.handChildrenPos.Add(generatedLeftHandChildren[i].position);
                            }

                            // Adding children rotations
                            for(int i=0; i < leftHandChildren.Length; i++)
                            {
                                newLeftHand.handChildrenRotation.Add(generatedLeftHandChildren[i].rotation);
                            }

                            // Adding number of children
                            newLeftHand.numChildren = newLeftHand.handChildrenName.Count;
                        }

                        leftHandData.Add(newLeftHand);
                    }
                }

                // Adding new right hand data
                if (generatedRightHand != null)
                {
                    Transform[] generatedRightHandChildren = null;
                    generatedRightHandChildren = GetHandChildren(generatedRightHand.transform, true);

                    if(generatedRightHandChildren.Length != 0)
                    {
                        Hand newRightHand = new Hand();

                        newRightHand.timer = timer;
                        if(generatedRightHandChildren.Length != 0)
                        {
                            // Adding children names
                            for(int i=0; i < rightHandChildren.Length; i++)
                            {
                                newRightHand.handChildrenName.Add(generatedRightHandChildren[i].gameObject.name);
                            }

                            // Adding children postions
                            for(int i=0; i < rightHandChildren.Length; i++)
                            {
                                newRightHand.handChildrenPos.Add(generatedRightHandChildren[i].position);
                            }

                            // Adding children rotations
                            for(int i=0; i < rightHandChildren.Length; i++)
                            {
                                newRightHand.handChildrenRotation.Add(generatedRightHandChildren[i].rotation);
                            }

                            // Adding number of children
                            newRightHand.numChildren = newRightHand.handChildrenName.Count;
                        }

                        rightHandData.Add(newRightHand);
                    }
                }
            }

            // Replaying loaded data if reply is enabled
            if(!ReplaySystem.rs.isRecord && ReplaySystem.rs.isReplaying)
            {
                // Displaying left hand accroding to loaded data
                if(loadLeftHandData.Count != 0)
                {
                    float nextIndex = CurrentIndex + 1;

                    if(nextIndex < loadLeftHandData.Count && nextIndex >= 0)
                    {
                        // Calculating the gap time between current and next hand
                        float gapTime = Mathf.Round((loadLeftHandData[(int)nextIndex].timer - loadLeftHandData[(int)nextIndex-1].timer) * 100f ) * 0.01f;

                        // Activating/deactivating and positioning  left hand game object according to gaptime
                        StartCoroutine(SetLeftHandData(nextIndex, loadLeftHandData[(int)nextIndex-1], gapTime));
                    }
                    
                }


                // Displaying right hand accroding to loaded data
                if(loadRightHandData.Count != 0)
                {
                    float nextIndex = CurrentIndex + 1;

                    if(nextIndex < loadRightHandData.Count && nextIndex >= 0)
                    {
                        // Calculating the gap time between current and next hand
                        float gapTime = Mathf.Round((loadLeftHandData[(int)nextIndex].timer - loadLeftHandData[(int)nextIndex-1].timer) * 100f ) * 0.01f;

                        // Activating/deactivating and positioning right hand game object according to gaptime
                        StartCoroutine(SetRightHandData(nextIndex, loadLeftHandData[(int)nextIndex-1], gapTime));
                    }
                    
                }
            }
        }
    }

    public Transform[] GetHandChildren(Transform parent, bool recursive)
    {
        List<Transform> children = new List<Transform>();

        // Looping through child objects
        foreach(Transform child in parent)
        {
            children.Add(child);
            if(recursive)
            {
                children.AddRange(GetHandChildren(child, true));
            }
        }

        return children.ToArray();
    }

    IEnumerator WaitForHand(float delay)
    {
        // Deactivating hand if there's a gaptime
        showHand = false;
        leftHandModel.SetActive(false);
        yield return new WaitForSeconds(delay);
        Debug.Log(delay);
    }

    IEnumerator SetLeftHandData(float Index, Hand hand, float gapTime)
    {
        // Checking for gaptime
        if(gapTime > Time.deltaTime)
        {
            yield return StartCoroutine(WaitForHand(gapTime));
        }   

        CurrentIndex = Index;

        leftHandModel.SetActive(true);

        // Setting left hand position and rotation
        for(int i=0; i < hand.numChildren; i++)
        {
            
            leftHandModelChildren[i].position = hand.handChildrenPos[i];
            leftHandModelChildren[i].rotation = hand.handChildrenRotation[i];
        }                  
    }

    IEnumerator SetRightHandData(float Index, Hand hand, float gapTime)
    {
        // Checking for gaptime
        if(gapTime > Time.deltaTime)
        {
            yield return StartCoroutine(WaitForHand(gapTime));
        }   

        CurrentIndex = Index;

        rightHandModel.SetActive(true);

        // Setting right hand position and rotation
        for(int i=0; i < hand.numChildren; i++)
        {
            
            rightHandModelChildren[i].position = hand.handChildrenPos[i];
            rightHandModelChildren[i].rotation = hand.handChildrenRotation[i];
        }                  
    }

    public void SaveHandData()
    {
        // Check if ReplayData folder exists
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        // Get current time and place into logfile's name
        string time = GetCurrentDateTime();
        // Creating logfile
        string path = dataPath + "/ReplayHandData{currentdatetime}.txt".Replace("{currentdatetime}", time);

        StreamWriter writer = new StreamWriter(path);

        if(leftHandData.Count != 0)
        {
            writer.WriteLine("OBJ_START~LeftHand");
            foreach(Hand leftHand in leftHandData){
                if(leftHand.handChildrenName.Count != 0)
                {
                    writer.WriteLine("\t\t" + leftHand.timer + "~" + leftHand.numChildren);
                
                    for(int i=0; i < leftHand.handChildrenName.Count; i++)
                    {
                        writer.WriteLine("\t\t\t" + leftHand.handChildrenName[i] + "~" + "(" + Mathf.Round(leftHand.handChildrenPos[i].x *100f)*0.01f + "," + Mathf.Round(leftHand.handChildrenPos[i].y *100f)*0.01f + "," + Mathf.Round(leftHand.handChildrenPos[i].z *100f)*0.01f + ")");
                        writer.WriteLine("\t\t\t" + leftHand.handChildrenName[i] + "~" + "(" + Mathf.Round(leftHand.handChildrenRotation[i].x *100f)*0.01f + "," + Mathf.Round(leftHand.handChildrenRotation[i].y *100f)*0.01f + "," + Mathf.Round(leftHand.handChildrenRotation[i].z *100f)*0.01f + "," + Mathf.Round(leftHand.handChildrenRotation[i].w *100f)*0.01f + ")");
                    }
                }
                
            }
            writer.WriteLine("OBJ_END~LeftHand");
        }     

        if(rightHandData.Count != 0)
        {
            writer.WriteLine("OBJ_START~RightHand");
            foreach(Hand rightHand in rightHandData){
                if(rightHand.handChildrenName.Count != 0)
                {
                    writer.WriteLine("\t\t" + rightHand.timer + "~" + rightHand.numChildren);
                
                    for(int i=0; i < rightHand.handChildrenName.Count; i++)
                    {
                        writer.WriteLine("\t\t\t" + rightHand.handChildrenName[i] + "~" + "(" + Mathf.Round(rightHand.handChildrenPos[i].x *100f)*0.01f + "," + Mathf.Round(rightHand.handChildrenPos[i].y *100f)*0.01f + "," + Mathf.Round(rightHand.handChildrenPos[i].z *100f)*0.01f + ")");
                        writer.WriteLine("\t\t\t" + rightHand.handChildrenName[i] + "~" + "(" + Mathf.Round(rightHand.handChildrenRotation[i].x *100f)*0.01f + "," + Mathf.Round(rightHand.handChildrenRotation[i].y *100f)*0.01f + "," + Mathf.Round(rightHand.handChildrenRotation[i].z *100f)*0.01f + "," + Mathf.Round(rightHand.handChildrenRotation[i].w *100f)*0.01f + ")");
                    }
                }
                
            }
            writer.WriteLine("OBJ_END~RightHand");
        }      

        // Adding new log file path to log files
        LogFileManager.logManager.AddLogFile(path);

        writer.Close();

        ResetHandData();
    }

    public void LoadHandData()
    {
        ResetLoadHandData();

        // Get all log files in ReplayData folder
        string[] files = Directory.GetFiles(dataPath,"ReplayHandData*.txt");

        // Ignore hidden lines in log file
        bool endObject = false;

        int filesCount = Mathf.Clamp(files.Length,1,10);
        List<float> hues = lr.HueCalculator(filesCount, lr.huevalues);
        int fileCounter = 0;

        foreach(string file in files)
        {
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

                    // Checking name of current hand
                    if(tokens[0] == "OBJ_START")
                    {
                        currentHandName = tokens[1];
                        endObject = false;
                    }
                    else if(tokens[0] != "OBJ_END" && !endObject)
                    {
                        if(currentHandName.Contains("LeftHand"))
                        {
                            // Gathering data of single left hand
                            if (loadInnerCount > 48 || loadInnerCount == 0)
                            {
                                if(newLoadLeftHand != null)
                                {
                                    loadLeftHandData.Add(newLoadLeftHand);
                                }
                                
                                loadInnerCount = 0;
                                newLoadLeftHand = new Hand();
                            }

                            if(float.TryParse(tokens[0],out float x))
                            {
                                newLoadLeftHand.timer = float.Parse(tokens[0]);
                                newLoadLeftHand.numChildren = int.Parse(tokens[1]);

                                loadInnerCount++;
                            }
                            else
                            {
                                // newLoadLeftHand.handChildrenName.Add(tokens[0]);

                                string tmpvalue = tokens[1].Replace("(", "").Replace(")", "");

                                string[] tmpvalues = tmpvalue.Split(',');

                                ParseHandData(tmpvalues,newLoadLeftHand, tokens[0]);

                                loadInnerCount++;
                            }
                        }
                        else if(currentHandName.Contains("RightHand"))
                        {
                            // Gathering data of single right hand
                            if (loadInnerCount > 48 || loadInnerCount == 0)
                            {
                                if(newLoadRightHand != null)
                                {
                                    loadRightHandData.Add(newLoadRightHand);
                                }
                                
                                loadInnerCount = 0;
                                newLoadRightHand = new Hand();
                            }

                            if(float.TryParse(tokens[0],out float x))
                            {
                                newLoadRightHand.timer = float.Parse(tokens[0]);
                                newLoadRightHand.numChildren = int.Parse(tokens[1]);

                                loadInnerCount++;
                            }
                            else
                            {                             
                                // newLoadRightHand.handChildrenName.Add(tokens[0]);

                                string tmpvalue = tokens[1].Replace("(", "").Replace(")", "");

                                string[] tmpvalues = tmpvalue.Split(',');

                                ParseHandData(tmpvalues,newLoadRightHand,tokens[0]);

                                loadInnerCount++;
                            }
                        }
                    }
                    else if(tokens[0] == "OBJ_END")
                    {
                        if(currentHandName.Contains("LeftHand"))
                        {
                            if(newLoadLeftHand != null)
                            {
                                loadLeftHandData.Add(newLoadLeftHand);
                            }
                        }
                        else if(currentHandName.Contains("RightHand"))
                        {
                            if(newLoadRightHand != null)
                            {
                                loadRightHandData.Add(newLoadRightHand);
                            }
                        }

                        // End of the object in log file
                        endObject = true;
                    }
                }
            }

            Color matcolor = Color.HSVToRGB(hues[fileCounter], 1, 1);

            if(loadLeftHandData.Count != 0)
            {
                leftHandModel = Instantiate(leftHandPrefab,new Vector3(-100, -100, -100), Quaternion.identity);
                leftHandModel.transform.SetParent(HandTrackerParent.transform);
                leftHandModelChildren = GetHandChildren(leftHandModel.transform, true);
                leftHandModel.SetActive(false);
                leftHandModelChildren[1].GetComponent<SkinnedMeshRenderer>().materials[0].color = matcolor;
            }
            
            if(loadRightHandData.Count != 0)
            {
                rightHandModel = Instantiate(rightHandPrefab,new Vector3(-100, -100, -100), Quaternion.identity);
                rightHandModel.transform.parent = gameObject.transform;
                rightHandModel = rightHandPrefab;
                rightHandModel.transform.SetParent(HandTrackerParent.transform);
                rightHandModelChildren = GetHandChildren(rightHandModel.transform, true);
                rightHandModel.SetActive(false);
                rightHandModelChildren[1].GetComponent<SkinnedMeshRenderer>().materials[0].color = matcolor;
            }

            fileCounter++;
        }
    }

    // Getting current time for saving file name
    string GetCurrentDateTime()
    {
        DateTime dt = DateTime.Now;

        string date = dt.Year + "-" + dt.Month + "-" + dt.Day;
        string currenttime = date + "T" + dt.Hour + "-" + dt.Minute + "-" + dt.Second;

        return currenttime;
    }

    // Resetting lists of hand list
    public void ResetHandData()
    {
        rightHandData.Clear();
        leftHandData.Clear();
        timer = 0.0f;
    }

    // Resetting lists of loaded hand list
    public void ResetLoadHandData()
    {
        loadLeftHandData.Clear();
        loadRightHandData.Clear();
        timer = 0.0f;
    }

    // Parsing selected file data
    void ParseHandData(string[] lines, Hand hand, string childName)
    {
        if(float.TryParse(lines[0],out float x) && float.TryParse(lines[1],out float y) && float.TryParse(lines[2],out float z)){
            if(lines.Length == 3 && lines[0] != null){
                Vector3 newPosition = new Vector3(float.Parse(lines[0]),float.Parse(lines[1]),float.Parse(lines[2]));  
                hand.handChildrenPos.Add(newPosition);
                hand.handChildrenName.Add(childName);
            }
            if(lines.Length == 4 && lines[0] != null){
                Quaternion newRotation = new Quaternion(float.Parse(lines[0]),float.Parse(lines[1]),float.Parse(lines[2]),float.Parse(lines[3]));
                hand.handChildrenRotation.Add(newRotation);
            }       
        }
    }

    private void OnDestroy()
    {
        if(leftHandData.Count != 0 && recordHandGestures)
        {
            SaveHandData();
        }
    }
}
