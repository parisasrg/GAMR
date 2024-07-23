using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AudioRecordTracker : MonoBehaviour
{
    // Singleton
    public static AudioRecordTracker audiotracker;

    // Controller
    List<AudioSource> unidentifiedAudioSources;

    // Audio tracker button
    public GameObject audioTrackerBtn;

    // List of audio data to save
    List<Audio> audioData;

    // Record audio
    public bool recordAudios = false;
    private string dataPath;

    string objName;
    Vector3 objPosition;

    // Timer
    float timer = 0f;

    // List of game's audio clips
    public List<AudioClip> audioclips;

    // Load Audio
    public GameObject audioDataParentObject;
    public GameObject audioDataPrefab;

    List<Audio> audioLoadDatas = new List<Audio>();

    LineRendererComponent lr;

    private void Awake() 
    {
        if(audiotracker == null)
        {
            audiotracker = this;
        }

        audioData = new List<Audio>();

        if (Application.isEditor)
        {
            dataPath = Application.dataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            dataPath = Application.persistentDataPath +"/ReplayData/" + SceneManager.GetActiveScene().name;
        }
    }

    private void Start() 
    {
        lr = GetComponent<LineRendererComponent>();

        // testing
        // LoadAudioData();
    }

    private void Update() 
    {
        // Activate audio tracking?
        // recordAudios = audioTrackerBtn.GetComponent<CheckBox>().checkboxed;

        if(recordAudios)
        {
            unidentifiedAudioSources = new List<AudioSource>();
            AudioSource[] allObjects = FindObjectsOfType<AudioSource>();

            foreach (AudioSource audioSource in allObjects)
            {
                if(audioSource.isPlaying && audioSource.time == 0 && audioSource.timeSamples == 0)
                {
                    unidentifiedAudioSources.Add(audioSource);
                }
            }

            if(ReplaySystem.rs.isRecord && !ReplaySystem.rs.isReplaying)
            {
                timer += Time.deltaTime;

                foreach(AudioSource audio in unidentifiedAudioSources)
                {
                    Audio newAudio = new Audio();

                    newAudio.timer = timer;
                    newAudio.audioSourceObject = audio.gameObject.name;
                    newAudio.audioClip = audio.clip.name;
                    newAudio.audioLength = audio.clip.length;
                    newAudio.audioPosition = audio.transform.position;
                    newAudio.audioRotation = audio.transform.rotation;

                    audioData.Add(newAudio);
                }
            }
        }
    }

    public void SaveAudioData()
    {
        // Check if ReplayData folder exists
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
        }

        // Get current time and place into logfile's name
        string time = GetCurrentDateTime();
        // Creating logfile
        string path = dataPath + "/ReplayAudioData{currentdatetime}.txt".Replace("{currentdatetime}", time);

        StreamWriter writer = new StreamWriter(path);

        if(audioData.Count != 0)
        {
            writer.WriteLine("OBJ_START~Audio");
            foreach(Audio audio in audioData){
                writer.WriteLine("\t\t" + audio.timer + "~" 
                                + audio.audioSourceObject + "~"
                                + audio.audioClip + "~"
                                + audio.audioLength + "~"
                                + "(" + Mathf.Round(audio.audioPosition.x *100f)*0.01f + "," + Mathf.Round(audio.audioPosition.y *100f)*0.01f + "," + Mathf.Round(audio.audioPosition.z *100f)*0.01f + ")"
                                + "(" + Mathf.Round(audio.audioRotation.x *100f)*0.01f + "," + Mathf.Round(audio.audioRotation.y *100f)*0.01f + "," + Mathf.Round(audio.audioRotation.z *100f)*0.01f + "," + Mathf.Round(audio.audioRotation.w *100f)*0.01f + ")");    
            }
            writer.WriteLine("OBJ_END~Audio");
        }

        // Adding new log file path to log files
        LogFileManager.logManager.AddLogFile(path);

        writer.Close();

        ResetAudioData();
    }

    public void LoadAudioData(){

        // Get all log files in ReplayData folder
        string[] files = Directory.GetFiles(dataPath,"ReplayAudioData*.txt");

        // Get selected log files
        // string[] inputfiles = LogFileManager.logManager.SelectedLogFiles();        

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
                        Audio loaddata  = new Audio();
                        loaddata.timer = float.Parse(tokens[0]);
                        loaddata.audioSourceObject = tokens[1];
                        loaddata.audioClip = tokens[2];
                        loaddata.audioLength = float.Parse(tokens[3]);
                        string tmpvalue = tokens[4].Replace("(", ",").Replace(")", "");

                        string[] tmpvalues = tmpvalue.Split(',');

                        if(i <= lineReader.Length && tmpvalues.Length > 0)
                        {
                            ParseAudioData(tmpvalues,i, loaddata);  
                        }
                        else
                        {
                            break;
                        }

                        audioLoadDatas.Add(loaddata);
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

            GameObject audiodataparent = new GameObject();
            audiodataparent.transform.name = "Audio"+fileCounter;
            audiodataparent.transform.SetParent(audioDataParentObject.transform);

            if(audioLoadDatas != null)
            {
                for(int cnt = 0; cnt < audioLoadDatas.Count; cnt++)
                {
                    if(cnt == 0)
                    {
                        GameObject audiodata = Instantiate(audioDataPrefab, audioLoadDatas[cnt].audioPosition, audioLoadDatas[cnt].audioRotation);

                        audiodata.transform.SetParent(audiodataparent.transform);

                        audiodata.GetComponent<MeshRenderer>().materials[0].color = Color.HSVToRGB(hues[fileCounter], 1, 1);

                        audiodata.GetComponent<AudioDataDisplay>().audioname = audioLoadDatas[cnt].audioClip;
                        // audiodata.GetComponent<AudioDataDisplay>().audioinfo = "Audio Clip : " + audioLoadDatas[cnt].audioClip + "\n \nAudio Source Object: " + audioLoadDatas[cnt].audioSourceObject + "\n \nAudio Clip Length: " + audioLoadDatas[cnt].audioLength;
                        audiodata.GetComponent<AudioDataDisplay>().data = "Audio Clip : " + audioLoadDatas[cnt].audioClip + "\n \nAudio Source Object: " + audioLoadDatas[cnt].audioSourceObject + "\n \nAudio Clip Length: " + audioLoadDatas[cnt].audioLength;
                    }
                    else if(audioLoadDatas[cnt].audioPosition != audioLoadDatas[cnt-1].audioPosition || audioLoadDatas[cnt].audioRotation != audioLoadDatas[cnt-1].audioRotation)
                    {
                        GameObject audiodata = Instantiate(audioDataPrefab, audioLoadDatas[cnt].audioPosition, audioLoadDatas[cnt].audioRotation);

                        audiodata.transform.SetParent(audiodataparent.transform);

                        audiodata.GetComponent<MeshRenderer>().materials[0].color = Color.HSVToRGB(hues[fileCounter], 1, 1);

                        audiodata.GetComponent<AudioDataDisplay>().audioname = audioLoadDatas[cnt].audioClip;
                        // audiodata.GetComponent<AudioDataDisplay>().audioinfo = "Audio Clip : " + audioLoadDatas[cnt].audioClip + "\n \nAudio Source Object: " + audioLoadDatas[cnt].audioSourceObject + "\n \nAudio Clip Length: " + audioLoadDatas[cnt].audioLength;
                        audiodata.GetComponent<AudioDataDisplay>().data = "Audio Clip : " + audioLoadDatas[cnt].audioClip + "\n \nAudio Source Object: " + audioLoadDatas[cnt].audioSourceObject + "\n \nAudio Clip Length: " + audioLoadDatas[cnt].audioLength;
                    }
                }

                fileCounter++;
            }

            audioLoadDatas.Clear();
        }
    }

    void ParseAudioData(string[] lines,int i, Audio audio)
    {
        if(float.TryParse(lines[1],out float x) && float.TryParse(lines[2],out float y) && float.TryParse(lines[3],out float z))
        {
            if(lines.Length == 8 && lines[1] != null){
                Vector3 newPosition = new Vector3(float.Parse(lines[1]),float.Parse(lines[2]),float.Parse(lines[3]));  
                audio.audioPosition = newPosition;
                Quaternion newRotation = new Quaternion(float.Parse(lines[4]),float.Parse(lines[5]),float.Parse(lines[6]),float.Parse(lines[7]));
                audio.audioRotation = newRotation;
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

    public void ResetAudioData()
    {
        audioData.Clear();
        timer = 0.0f;
    }

    private void OnDestroy()
    {
        if(audioData.Count != 0)
        {
            SaveAudioData();
        }
    }
}
