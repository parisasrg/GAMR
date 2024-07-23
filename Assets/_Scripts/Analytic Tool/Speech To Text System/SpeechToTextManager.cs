using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class SpeechToTextManager : MonoBehaviour
{
    public TMP_Text comment;
    public GameObject commentUI;

    public GameObject startbtn;
    public GameObject stopbtn;
    bool isRecording = false;

    Vector3 notePos;
    public GameObject noteBtn;

    DictationRecognizer dictationRecognizer;

    public MessageBehavior startmessage;
    public MessageBehavior stopmessage;

    void Awake()
    {
        // comment.text = "Press start and say something to add a note...";
        dictationRecognizer = new DictationRecognizer();

        dictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
        dictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
        dictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;

        commentUI.SetActive(false);
        startbtn.SetActive(true);
        stopbtn.SetActive(false);
    }

    private void DictationRecognizer_DictationHypothesis(string text)
    {
        // this.comment.text = this.comment.text + text;
    }

    private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
    {
        if(this.comment.text == "Press start and say something to add a note...")
        {
            comment.text = "";
        }
        this.comment.text = this.comment.text + text + " ";
    }

    private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
    {
        dictationRecognizer.Stop();
    }

    public void SpeechToText()
    {
        // PhraseRecognitionSystem.Shutdown();
        // commentUI.SetActive(true);
        
        // Stops replaying of the recording
        ReplaySystem.rs.StopReplay();
    }

    public void StartSpeechToText()
    {
        if(!isRecording)
        {
            // Display start recording message
            startmessage.FadeOut();

            PhraseRecognitionSystem.Shutdown();
            startbtn.SetActive(false);
            stopbtn.SetActive(true);

            this.comment.text = "";

            // RecordIndicator.recordindicator.StartBlink();
            dictationRecognizer.Start();
        }

        isRecording = true;
    }

    public void StopSpeechToText()
    {
        if(isRecording)
        {
            // Display stop recording message
            stopmessage.FadeOut();

            // Restarts replaying of the recording
            ReplaySystem.rs.Replay();

            // RecordIndicator.recordindicator.StopBlink();
            dictationRecognizer.Stop();

            notePos = Camera.main.transform.position;
            GameObject note = Instantiate(noteBtn, new Vector3(notePos.x, 0.05f, notePos.z), Quaternion.identity);
            note.GetComponentInChildren<Note>().note = this.comment.text;

            commentUI.SetActive(false);
            startbtn.SetActive(true);
            stopbtn.SetActive(false);
        }

        isRecording = false;
    }
}
