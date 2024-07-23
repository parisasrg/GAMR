using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandRecorder : MonoBehaviour
{
    // Test
    public bool enableHandTracker = false;
    public bool enableReplay = false;
    [SerializeField]private float CurrentIndex;


    // Singlton
    public static HandRecorder hr;


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

    private float timer;

    private void Awake() {
        // Creating singleton instance of this class
        if(hr == null)
        {
            hr = this;
        }

        // Initialization of saving path
        if (Application.isEditor)
        {
            dataPath = Application.dataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }
        else
        {
            dataPath = Application.persistentDataPath + "/ReplayData/" + SceneManager.GetActiveScene().name;
        }

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

    private void OnDestroy()
    {
        if(leftHandData.Count != 0 && recordHandGestures)
        {
            SaveHandData();
        }
    }
}
