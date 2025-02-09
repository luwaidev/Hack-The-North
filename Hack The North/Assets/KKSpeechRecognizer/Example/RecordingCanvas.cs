﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using KKSpeech;

public class RecordingCanvas : MonoBehaviour
{
    public Button startRecordingButton;
    public Text resultText;
    public FuckYou fy;
    public Image indicator;
    public bool isFrench = false;
    public string language = "en-US";

    void Start()
    {
        if (SpeechRecognizer.ExistsOnDevice())
        {
            SpeechRecognizerListener listener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
            listener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
            listener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
            listener.onErrorDuringRecording.AddListener(OnError);
            listener.onErrorOnStartRecording.AddListener(OnError);
            listener.onFinalResults.AddListener(OnFinalResult);
            listener.onPartialResults.AddListener(OnPartialResult);
            listener.onEndOfSpeech.AddListener(OnEndOfSpeech);
            SpeechRecognizer.RequestAccess();
        }
        else
        {
            resultText.text = "Sorry, but this device doesn't support speech recognition";
            startRecordingButton.enabled = false;
        }

        SpeechRecognizer.SetDetectionLanguage("en-US");
    }

    public void OnChangeLanguage(int num)
    {
        switch (num)
        {
            case 0:
                SpeechRecognizer.SetDetectionLanguage("en-US");
                isFrench = false;
                break;
            case 1:
                SpeechRecognizer.SetDetectionLanguage("fr-CA");
                isFrench = true;
                break;
            default:
                break;
        }
    }

    public void OnFinalResult(string result)
    {
        string text = result;
        fy.AudioEnd(text);

        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        indicator.enabled = true;
        // resultText.text = result;
        startRecordingButton.enabled = true;

    }

    public void OnPartialResult(string result)
    {
        resultText.text = result;
    }

    public void OnAvailabilityChange(bool available)
    {
        startRecordingButton.enabled = available;
        if (!available)
        {
            resultText.text = "Speech Recognition not available";
        }
        else
        {
            resultText.text = "Say something :-)";
            indicator.enabled = true;
        }
    }

    public void OnAuthorizationStatusFetched(AuthorizationStatus status)
    {
        switch (status)
        {
            case AuthorizationStatus.Authorized:
                startRecordingButton.enabled = true;
                break;
            default:
                startRecordingButton.enabled = false;
                resultText.text = "Cannot use Speech Recognition, authorization status is " + status;
                break;
        }
    }

    public void OnEndOfSpeech()
    {
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        indicator.enabled = false;
        string text = resultText.text;
        if (language != "en-US")
        {
            // fy.translate(resultText.text);
        }
        // GameObject.Find("Notes Manager").GetComponent<NotesManager>().transcript += "\n" + resultText.text;
    }

    public void OnError(string error)
    {
        Debug.LogError(error);
        startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        indicator.enabled = false;
        startRecordingButton.enabled = true;
    }

    public void OnStartRecordingPressed()
    {
        if (SpeechRecognizer.IsRecording())
        {
#if UNITY_IOS && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Stopping";
			startRecordingButton.enabled = false;
#elif UNITY_ANDROID && !UNITY_EDITOR
			SpeechRecognizer.StopIfRecording();
			startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
        indicator.enabled = false;
#endif
        }
        else
        {
            SpeechRecognizer.StartRecording(true);
            startRecordingButton.GetComponentInChildren<Text>().text = "Stop Recording";
            resultText.text = "Say something :-)";
            indicator.enabled = true;
        }
    }
}
