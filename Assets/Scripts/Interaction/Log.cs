using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objs/Log Data")]
public class Log : ScriptableObject
{
    // The type of information the log contains
    public enum LogType { Text, Audio, Image};
    public LogType type;

    [Header("All Logs")]

    // Depending on the manner in which we decide to store and display these, we might need an index that this gets placed into 
    // an array at

    [Tooltip("Date of the log (MM/DD/YYYY)")]
    public Vector3 date;
    [Tooltip("Log timestamp (for nondigital content, 'timestamp indeterminate')")]
    public string timestamp;
    [TextArea(1, 5), Tooltip("Paragraphs of text for text log, subtitles for audio log, or image description for image log")] 
    public List<string> textParas;

    // If an audio file, two additional pieces of info are needed
    [Header("Audio Logs")]
    [Tooltip("How long each subtitle needs to be on screen / read out for before the next one begins")] 
    public List<float> subtitleTiming;
    [Tooltip("The audio contents of the log")]
    public AudioClip audio;

    // If an image file, we need the image
    [Header("Image Logs")]
    [Tooltip("The visual contents of the log")]
    public Sprite visual;

}
