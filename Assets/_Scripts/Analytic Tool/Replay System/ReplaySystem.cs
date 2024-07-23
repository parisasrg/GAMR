using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using System.Linq;


public class ReplaySystem : MonoBehaviour
{
    // For testing
    AnalyticPCControls analyticCtrl;

    public static ReplaySystem rs;


    public bool recordAtAwake = false;


    // GameControls inputAction;

    public List<GameObject> prefabsToLoad;
    public List<Objects> objectsToRecord;
    List<ObjectsPositions> objspositions = new List<ObjectsPositions>();

    List<Position> positions = new List<Position>();

    // Player
    public GameObject PlayerTrackerParent;
    public GameObject PlayerPrefab;
    List<GameObject> PlayerPrefabs = new List<GameObject>();
    private Vector3 DefaultPosition;
    private Quaternion DefaultRotation;

    // Camera
    public GameObject CameraTrackerParent;
    public GameObject CameraPrefab;
    List<GameObject> CameraPrefabs = new List<GameObject>();
    private Vector3 DefaultCameraPosition;
    private FOVCamera DefaultCameraPoints;

    // Game Objects
    GameObject objectPrefab;
    List<GameObject> objectsLoaded = new List<GameObject>();
    List<ObjectsPositions> objectsLoad = new List<ObjectsPositions>();
    List<ObjectsPositions> objectsLoadTemp = new List<ObjectsPositions>();
    private Vector3 objectPosition;
    private Vector3 objectRotation;
    public GameObject GOTrackerParent;

    string currentObjectName;
    
    LineRendererComponent lr;
    public bool loadLineAtOnce = true;
    public GameObject loadLineAtOnceBtn;

    public GameObject Player;
    [SerializeField]private float CurrentIndex;
    private float changeRate = 0;
    public bool isReplaying = false;
    public bool isRewind = false;
    public bool isFastForward = false;
    public bool isRecord = false;
    public bool resetReplay = false;
    private bool pos,rot;
    private string dataPath;

    public List<Vector3> load_positions = new List<Vector3>();
    protected List<Quaternion> load_rotations = new List<Quaternion>();

    public List<FOVCamera> load_fovpoints = new List<FOVCamera>();
    public List<Vector3> load_fovpositions = new List<Vector3>();

    // Timer
    private float timer;
    
    int randNum;
    
    void Awake()
    {
        // Testing
        analyticCtrl = new AnalyticPCControls();
        // analyticCtrl.Analytic.Record.performed += cntxt => Record();
        // analyticCtrl.Analytic.StopRecord.performed += cntxt => StopRecording();
        // analyticCtrl.Analytic.Replay.performed += cntxt => this.gameObject.GetComponent<HandRecorder>().LoadHandData();
        // analyticCtrl.Analytic.Save.performed += cntxt => this.gameObject.GetComponent<HandRecorder>().SaveHandData();
        analyticCtrl.Analytic.Load.performed += cntxt => LoadButton();
        analyticCtrl.Analytic.Replay.performed += cntxt => Replay();
        analyticCtrl.Analytic.Menu.performed += cntxt => SceneManager.LoadScene(0);
        analyticCtrl.Analytic.Rewind.performed += cntxt => Rewind();
        analyticCtrl.Analytic.FastForward.performed += cntxt => FastForward();

        // Datapath for log files
        // LogFileManager.logManager.levelpath = levelpath;

        if(rs == null)
        {
            rs = this;
        }

        if (Application.isEditor)
        {
            dataPath = Application.dataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            dataPath = Application.persistentDataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }

        if(objectsToRecord.Count != 0)
        {
            for(int i = 0; i < objectsToRecord.Count; i++)
            {
                ObjectsPositions objspos = new ObjectsPositions();
                objspositions.Add(objspos);
            }
        }
    }

    void Start()
    {
        lr = GetComponent<LineRendererComponent>();

        // Check if ReplayData folder exists
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        if(recordAtAwake)
        {
            Record();
        }
    }

    private void OnEnable() 
    {
        analyticCtrl.Analytic.Enable();
    }

    private void OnDisable() 
    {
        analyticCtrl.Analytic.Disable();
    }

    private void FixedUpdate(){

        if(!isReplaying && isRecord)
        {
            timer += Time.deltaTime;
            
            positions.Add(new Position{name = Player.gameObject.name, position = Player.transform.position,rotation = Player.transform.rotation, timer = timer});
            CameraTracker.camtrack.AddFOVpoint(timer);  

            if(objectsToRecord.Count != 0)
            {
                for(int i = 0; i < objectsToRecord.Count; i++)
                {
                    Position newpos = new Position();
                    newpos.name = objectsToRecord[i].gameobj.name;
                    newpos.position = objectsToRecord[i].gameobj.transform.position;
                    newpos.rotation = objectsToRecord[i].gameobj.transform.rotation;
                    newpos.timer = timer;

                    objspositions[i].prefabName = objectsToRecord[i].prefabName;
                    objspositions[i].positions.Add(newpos);
                }
            }
        }

        if(isReplaying || isFastForward)
        {

            // Fix this, it shouldn't happen
            RecordIndicator.recordindicator.StopBlink();
            isRecord = false;
            // Fix this

            float nextIndex = CurrentIndex + changeRate;
            for(int i = 0; i<PlayerPrefabs.Count; i++)
            {
                load_positions.Clear();
                load_rotations.Clear();

                load_fovpositions.Clear();
                load_fovpoints.Clear();

                load_positions.AddRange(PlayerPrefabs[i].GetComponent<PositionTracker>().positions);
                load_rotations.AddRange(PlayerPrefabs[i].GetComponent<PositionTracker>().rotations);
                load_fovpositions.AddRange(CameraPrefabs[i].GetComponent<CameraPositionTracker>().positions);
                load_fovpoints.AddRange(CameraPrefabs[i].GetComponent<CameraPositionTracker>().points);
                if(nextIndex < load_positions.Count && nextIndex >= 0)
                {
                    if(!loadLineAtOnce)
                    {
                        if(isReplaying)
                        {
                            lr.AnimateLine((int)nextIndex, PlayerPrefabs[i].GetComponent<LineRenderer>(), load_positions);
                        }
                        else if(isFastForward)
                        {
                            if(nextIndex != 0)
                                lr.AnimateLine((int)nextIndex - 1, PlayerPrefabs[i].GetComponent<LineRenderer>(), load_positions);
                            lr.AnimateLine((int)nextIndex, PlayerPrefabs[i].GetComponent<LineRenderer>(), load_positions);
                        }
                    }
                    setTransform(nextIndex, PlayerPrefabs[i], CameraPrefabs[i]);
                }
            }


            if(objectsLoad.Count != 0)
            {
                for(int i = 0; i < objectsLoaded.Count; i++)
                {
                    if(nextIndex < objectsLoad[i].positions.Count && nextIndex >= 0)
                    {
                        setObjectTransform(nextIndex, objectsLoaded[i], objectsLoad[i]);
                    }
                }
            }
        } 

        if(isRewind)
        {
            float prevIndex = CurrentIndex + changeRate;
            for(int i = 0; i<PlayerPrefabs.Count; i++)
            {
                load_positions.Clear();
                load_rotations.Clear();

                load_fovpositions.Clear();
                load_fovpoints.Clear();

                load_positions.AddRange(PlayerPrefabs[i].GetComponent<PositionTracker>().positions);
                load_rotations.AddRange(PlayerPrefabs[i].GetComponent<PositionTracker>().rotations);
                load_fovpositions.AddRange(CameraPrefabs[i].GetComponent<CameraPositionTracker>().positions);
                load_fovpoints.AddRange(CameraPrefabs[i].GetComponent<CameraPositionTracker>().points);
                if(prevIndex < load_positions.Count && prevIndex >= 0)
                {
                    if(!loadLineAtOnce)
                    {
                        lr.AnimateLine((int)prevIndex, PlayerPrefabs[i].GetComponent<LineRenderer>(), load_positions);
                    }
                    setTransform(prevIndex, PlayerPrefabs[i], CameraPrefabs[i]); 
                }
            }

            if(objectsLoad.Count != 0)
            {
                for(int i = 0; i < objectsLoaded.Count; i++)
                {
                    if(prevIndex < objectsLoad[i].positions.Count && prevIndex >= 0)
                    {
                        setObjectTransform(prevIndex, objectsLoaded[i], objectsLoad[i]);
                    }
                }
            }
        }
    }

    void TimerReport()
    {
        float hours = TimeSpan.FromSeconds(timer).Hours;
        float minutes = TimeSpan.FromSeconds(timer).Minutes;
        float seconds = TimeSpan.FromSeconds(timer).Seconds;
        Debug.Log(string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds));
    }

    public void LoadButton(){
        loadLineAtOnce = loadLineAtOnceBtn.GetComponent<CheckBox>().checkboxed;
        
        loadData();
        AudioRecordTracker.audiotracker.LoadAudioData();
        if(HandRecorder.hr.enableHandTracker)
            HandRecorder.hr.rightHandPrefab = objectsLoaded[0].transform.GetChild(1).gameObject;
        HandRecorder.hr.LoadHandData();

        FilterLogFilesManager.filtermanager.ReportFilters();
    }

    public void Record(){
        isRecord = true;
        timer = 0.0f;
        RecordIndicator.recordindicator.StartBlink();
    }

    public void StopRecording(){
        isRecord = false;
        RecordIndicator.recordindicator.StopBlink();
        saveData();
        ResetPositions();
    }

    public void Replay(){
        isRecord = false;
        isRewind = false;

        // Check if user want to restart the replay?
        resetReplay = false;

        if(changeRate != 1)
        {
            isReplaying = true;
            changeRate = 1;
        }   
        else
        {
            isReplaying = false;
            changeRate = 0;
        }
    }

    public void StopReplay()
    {
        isRecord = false;
        isReplaying = true;
        isRewind = false;

        changeRate = 0;
    }

    public void Rewind(){
        isRewind = true;
        isReplaying = false;

        changeRate = -1; /* Skip how many frames? */
    }

    public void FastForward()
    {
        isFastForward = true;
        isReplaying = false;
        isRewind = false;
        isRecord = false;

        changeRate = 2;
    }

    private void setTransform(float Index, GameObject playerprefab, GameObject cameraprefab){
        
        CurrentIndex = Index;
        Vector3 playerPos = load_positions[(int)Index];
        Quaternion rotation = load_rotations[(int)Index];
        playerprefab.transform.position = playerPos;
        playerprefab.transform.rotation = rotation;

        Vector3 camPos = load_fovpositions[(int)Index];
        cameraprefab.transform.GetChild(0).position = camPos;
        cameraprefab.GetComponent<FOVPyramid>().DisplayFOVPyramid(cameraprefab.transform.GetChild(0).localPosition, load_fovpoints[(int)Index].camCor1, load_fovpoints[(int)Index].camCor2, load_fovpoints[(int)Index].camCor3, load_fovpoints[(int)Index].camCor4);
    }

    private void setObjectTransform(float Index, GameObject objectprefab, ObjectsPositions objpos){
        
        CurrentIndex = Index;
        Vector3 objectPos = objpos.positions[(int)Index].position;
        Quaternion objectRot = objpos.positions[(int)Index].rotation;
        objectprefab.transform.position = objectPos;
        objectprefab.transform.rotation = objectRot;
    }

    private void ResetPositions()
    {
        positions.Clear();
        CameraTracker.camtrack.FOVpoints.Clear();
    }

    private void saveData(){
        // Check if ReplayData folder exists
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        // Get current time and place into logfile's name
        string time = GetCurrentDateTime();
        // Creating logfile
        string path = dataPath + "/ReplayData{currentdatetime}.txt".Replace("{currentdatetime}", time);

        // Start writing in logfile
        StreamWriter writer = new StreamWriter(path);

        if(positions.Count != 0)
        {
            writer.WriteLine("OBJ_START~" + positions[0].name);
                foreach(Position pos in positions)
                {
                    writer.WriteLine("\t\t" + Mathf.Round(pos.timer * 1000)/1000 + "~" + "(" + Mathf.Round(pos.position.x *100f)*0.01f + "," + Mathf.Round(pos.position.y *100f)*0.01f + "," + Mathf.Round(pos.position.z *100f)*0.01f + ")");
                    writer.WriteLine("\t\t" + Mathf.Round(pos.timer * 1000)/1000 + "~" + "(" + Mathf.Round(pos.rotation.x *100f)*0.01f + "," + Mathf.Round(pos.rotation.y *100f)*0.01f + "," + Mathf.Round(pos.rotation.z *100f)*0.01f + ")");
                }
            writer.WriteLine("OBJ_END~" + positions[0].name);
        }

        // Camera Tracking
        if(CameraTracker.camtrack.FOVpoints.Count != 0)
        {
            writer.WriteLine("OBJ_START~" + CameraTracker.camtrack.FOVpoints[0].camName);
            
                foreach(FOVCamera point in CameraTracker.camtrack.FOVpoints){
                    writer.WriteLine("\t\t" + Mathf.Round(point.timer * 1000)/1000 + "~" + point.camPos + "," + point.camCor1 + "," + point.camCor2 + "," + point.camCor3 + "," + point.camCor4);
                }     
            writer.WriteLine("OBJ_END~" + CameraTracker.camtrack.FOVpoints[0].camName);
        }

        if(objspositions.Count != 0)
        {
            for(int i = 0; i < objspositions.Count; i++)
            {
                writer.WriteLine("OBJ_START~" + objspositions[i].positions[0].name + "~" + objspositions[i].prefabName);
                foreach(Position pos in objspositions[i].positions)
                {
                    writer.WriteLine("\t\t" + Mathf.Round(pos.timer * 1000)/1000 + "~" + "(" + Mathf.Round(pos.position.x *100f)*0.01f + "," + Mathf.Round(pos.position.y *100f)*0.01f + "," + Mathf.Round(pos.position.z *100f)*0.01f + ")");
                    writer.WriteLine("\t\t" + Mathf.Round(pos.timer * 1000)/1000 + "~" + "(" + Mathf.Round(pos.rotation.x *100f)*0.01f + "," + Mathf.Round(pos.rotation.y *100f)*0.01f + "," + Mathf.Round(pos.rotation.z *100f)*0.01f + ")");
                }
                writer.WriteLine("OBJ_END~" + objspositions[i].positions[0].name);
            }
        }

        // Adding new log file path to log files
        LogFileManager.logManager.AddLogFile(path);

        writer.Close();

        string title = "X,Y,Z\n";
        string postxt = "";
        foreach(Position pos in positions){
            postxt += Mathf.Round(pos.position.x *100f)*0.01f + "," + Mathf.Round(pos.position.y *100f)*0.01f + "," + Mathf.Round(pos.position.z *100f)*0.01f + "\n";
        }
        // System.IO.File.WriteAllText(dataPath + "Database.csv", title + postxt);
        System.IO.File.WriteAllText(dataPath + "/HeatmapData{currentdatetime}.csv".Replace("{currentdatetime}", time), title + postxt);
    }

    private void loadData(){
        // Clean objects to load list
        objectsLoad.Clear();
        objectsLoadTemp.Clear();

        // Get selected log files
        // string[] files = LogFileManager.logManager.SelectedLogFiles();

        // Get all log files in ReplayData folder
        string[] files = Directory.GetFiles(dataPath,"ReplayData*.txt");

        // Ignore hidden lines in log file
        bool endObject = false;

        // Deactivating objects to record and place the recorded ones instead
        if(objectsToRecord.Count != 0)
        {
            for(int i = 0; i < objectsToRecord.Count; i++)
            {
                objectsToRecord[i].gameobj.SetActive(false);
            }
        }

        int filesCount = Mathf.Clamp(files.Length,1,10);
        List<float> hues = lr.HueCalculator(filesCount, lr.huevalues);
        int fileCounter = 0;

        foreach(string file in files){
            int i=1;
            StreamReader stream = new StreamReader(file);
            string lineReader = stream.ReadToEnd();
            string[] lines = lineReader.Split('\n');

            int io=1;
            ObjectsPositions obj = new ObjectsPositions();
            
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
                        currentObjectName = tokens[1];

                        // Game Object's data
                        io = 1;
                        
                        if(tokens.Length == 3)
                        {
                            obj = new ObjectsPositions();
                            obj.prefabName = tokens[2];
                        }

                        endObject = false;
                    }
                    else if(tokens[0] != "OBJ_END" && !endObject)
                    {
                        string tmpvalue = tokens[1].Replace("(", "").Replace(")", "");

                        string[] tmpvalues = tmpvalue.Split(',');

                        if(i <= lineReader.Length && tmpvalues.Length > 0){
                            if(currentObjectName.Contains("Player"))
                            {
                                AddPlayerData(tmpvalues,i);
                            }   
                            else if(currentObjectName.Contains("Camera"))
                            {
                                AddCameraData(tmpvalues,i);
                            }
                            else if(obj.prefabName != null)
                            {
                                if(io%2 == 0)
                                {
                                    objectPosition = AddObjectData(tmpvalues);
                                }
                                else if(io%2 != 0)
                                {
                                    objectRotation = AddObjectData(tmpvalues);
                                    Position newobject = new Position();
                                    newobject.timer = float.Parse(tokens[0]);
                                    newobject.name = currentObjectName;
                                    newobject.position = objectPosition;
                                    newobject.rotation = new Quaternion(objectRotation.x, objectRotation.y, objectRotation.z, 0);
                                    obj.positions.Add(newobject);
                                }
                            }
                                
                        }
                    }
                    else if(tokens[0] == "OBJ_END")
                    {
                        // Checking if game object is not null
                        if(obj.prefabName != null)
                        {
                            objectsLoadTemp.Add(obj);
                        }

                        // End of the object in log file
                        endObject = true;
                    }

                    i++; 
                    io++;
                }
            }

            stream.Close();

            // Getting player prefab's first recorded position
            DefaultPosition = load_positions[0];
            DefaultRotation = load_rotations[0];

            if(GameObject.FindGameObjectsWithTag("PlayerPrefab").Length < files.Length){
                // Creating a player prefab for each log file
                PlayerPrefab = Instantiate(PlayerPrefab, DefaultPosition, DefaultRotation);

                // Setting player prefab's name
                PlayerPrefab.transform.name = "Player"+fileCounter;

                // Setting the player gameobject parent
                PlayerPrefab.transform.SetParent(PlayerTrackerParent.transform);

                // Adding Line Renderer component
                if(PlayerPrefab.GetComponent<LineRenderer>() == null)
                    PlayerPrefab.AddComponent<LineRenderer>();

                // Adding Position Tracker component if does not exist
                if(PlayerPrefab.GetComponent<PositionTracker>() == null)
                    PlayerPrefab.AddComponent<PositionTracker>();
                
                // Removing previous positions from the list
                PlayerPrefab.GetComponent<PositionTracker>().ResetPositions();
                // Adding new values to the list for each player prefab
                PlayerPrefab.GetComponent<PositionTracker>().positions.AddRange(load_positions);
                PlayerPrefab.GetComponent<PositionTracker>().rotations.AddRange(load_rotations);

                PlayerPrefabs.Add(PlayerPrefab);
            }
            
            lr.LineRendererComponentFn(hues[fileCounter], PlayerPrefab);

            DefaultCameraPosition = load_fovpositions[0];
            DefaultCameraPoints = load_fovpoints[0];

            if(GameObject.FindGameObjectsWithTag("FOVPyramid").Length < files.Length){
                // Creating a camera prefab for each log file
                GameObject FovPrefab = Instantiate(CameraPrefab, DefaultCameraPosition, Quaternion.identity);
                FovPrefab.transform.GetChild(0).localPosition = DefaultCameraPosition;

                // Setting camera prefab's name
                FovPrefab.transform.name = "Camera"+fileCounter;

                // Setting the camra gameobject parent
                FovPrefab.transform.SetParent(CameraTrackerParent.transform);

                if(FovPrefab.GetComponent<FOVPyramid>() == null)
                    FovPrefab.AddComponent<FOVPyramid>();
                
                if(FovPrefab.GetComponent<CameraPositionTracker>() == null)
                    FovPrefab.AddComponent<CameraPositionTracker>();

                // Removing previous positions from the list
                FovPrefab.GetComponent<CameraPositionTracker>().ResetPositions();
                // Adding new values to the list for each FOV prefab
                FovPrefab.GetComponent<CameraPositionTracker>().positions.AddRange(load_fovpositions);

                FovPrefab.GetComponent<CameraPositionTracker>().points.AddRange(load_fovpoints);
                FovPrefab.GetComponent<FOVPyramid>().SetMeshColor(hues[fileCounter]);
                FovPrefab.GetComponent<FOVPyramid>().DisplayFOVPyramid(DefaultCameraPoints.camPos, DefaultCameraPoints.camCor1, DefaultCameraPoints.camCor2, DefaultCameraPoints.camCor3, DefaultCameraPoints.camCor4);

                CameraPrefabs.Add(FovPrefab);
            }

            // Setting parent for each group of recorded GOs
            GameObject GOgroupparent = new GameObject();
            GOgroupparent.transform.name = "GOGroup"+fileCounter;
            GOgroupparent.transform.SetParent(GOTrackerParent.transform);

            // Creating recorded game objects in their intial position and rotation
            if(objectsLoadTemp.Count != 0)
            {
                foreach(ObjectsPositions objs in objectsLoadTemp)
                {
                    foreach(GameObject prfb in prefabsToLoad)
                    {
                        if(objs.prefabName.Contains(prfb.transform.name))
                        {
                            GameObject gobject = Instantiate(prfb, objs.positions[0].position, objs.positions[0].rotation);
                            gobject.transform.name = objs.positions[0].name;
                            gobject.transform.SetParent(GOgroupparent.transform);
                            gobject.transform.GetChild(1).GetComponent<HandMesh>().handMesh.materials[0].color = Color.HSVToRGB(hues[fileCounter], 1, 1);
                            objectsLoad.Add(objs);
                            objectsLoaded.Add(gobject);
                        }
                    }
                }
            }

            fileCounter++;

            load_positions.Clear();
            load_rotations.Clear();
            load_fovpoints.Clear();
            load_fovpositions.Clear();
            objectsLoadTemp.Clear();
        }

        // int count = 0;

        // // Creating recorded game objects in their intial position and rotation
        // if(objectsLoad.Count != 0)
        // {
        //     foreach(ObjectsPositions objs in objectsLoad)
        //     {
        //         foreach(GameObject prfb in prefabsToLoad)
        //         {
        //             if(objs.prefabName.Contains(prfb.transform.name))
        //             {
        //                 GameObject gobject = Instantiate(prfb, objs.positions[0].position, objs.positions[0].rotation);
        //                 gobject.transform.name = objs.positions[0].name;
        //                 gobject.transform.SetParent(GOTrackerParent.transform);
        //                 gobject.GetComponentInChildren<HandMesh>().handMesh.material.color = Color.HSVToRGB(hues[count], 1, 1);
        //                 objectsLoaded.Add(gobject);

        //                 count++;
        //             }
        //         }
        //     }
        // }

    }

    void AddPlayerData(string[] lines, int i){
        if(float.TryParse(lines[0],out float x) && float.TryParse(lines[1],out float y) && float.TryParse(lines[2],out float z)){
            if(i%2 == 0 && lines[0] != null){
                Vector3 newPos = new Vector3(float.Parse(lines[0]),float.Parse(lines[1]),float.Parse(lines[2]));
                load_positions.Add(newPos);           
            }
            if(i%2 != 0 && lines[0] != null){
                Quaternion newRot = new Quaternion(float.Parse(lines[0]),float.Parse(lines[1]),float.Parse(lines[2]),0);
                load_rotations.Add(newRot);   
            }
        }          
    }

    void AddCameraData(string[] stringpts,int index)
    {
        List<Vector3> points = new List<Vector3>();

        for(int i=0; i <= stringpts.Length; i++)
        {
            if(i%3 == 0 && i > 0)
                points.Add(StringToVector3(stringpts[i-3],stringpts[i-2],stringpts[i-1]));
        }

        load_fovpoints.Add(new FOVCamera{camPos = points[0], camCor1 = points[1], camCor2 = points[2], camCor3 = points[3], camCor4 = points[4], timer = 0f});       
        load_fovpositions.Add(points[0]); 
    }

    Vector3 AddObjectData(string[] lines){
        if(float.TryParse(lines[0],out float x) && float.TryParse(lines[1],out float y) && float.TryParse(lines[2],out float z))
        {
            if(lines[0] != null)
            {
                Vector3 newvector = new Vector3(float.Parse(lines[0]),float.Parse(lines[1]),float.Parse(lines[2]));
                return newvector;       
            }
            else
            {
                return Vector3.zero;
            }
        }  
        else
        {
            return Vector3.zero;
        }        
    }

    Vector3 StringToVector3(string x, string y, string z)
    {
        return new Vector3(float.Parse(x),float.Parse(y),float.Parse(z));
    }

    string GetCurrentDateTime()
    {
        DateTime dt = DateTime.Now;

        string date = dt.Year + "-" + dt.Month + "-" + dt.Day;
        string currenttime = date + "T" + dt.Hour + "-" + dt.Minute + "-" + dt.Second;

        return currenttime;
    }

    private void OnDestroy() 
    {
        if(positions.Count != 0)
        {
            saveData();
        }    
    }

    private void OnApplicationQuit()
    {
        if(positions.Count != 0)
        {
            saveData();
        }    
    }
}

