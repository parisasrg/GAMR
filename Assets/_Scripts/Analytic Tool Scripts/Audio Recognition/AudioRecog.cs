using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class AudioRecog : MonoBehaviour, IMixedRealitySpeechHandler
{
    
    void Awake()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {

        switch(eventData.Command.Keyword.ToLower())
        {
            case "record":
                ReplaySystem.rs.Record();
                break;
            case "stop":
                ReplaySystem.rs.StopRecording();
                break;
            default:
                Debug.Log($"Unknown option {eventData.Command.Keyword}");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
