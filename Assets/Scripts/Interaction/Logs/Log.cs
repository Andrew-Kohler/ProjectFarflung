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
    [Tooltip("File name (file(1), What's For Lunch, etc.)")]
    public string filename;
    [Tooltip("If this is a dated material or not")]
    public bool hasDate = true;
    [Tooltip("Date of the log on Oixys-3 (RelativeMonth.Day)")]
    public Vector2 date;
    [Tooltip("Was this log sent to Oixys-3 from another planet?")]
    public bool offworldOrigin;
    [Tooltip("If so, where was it sent from?")]
    public string planetOfOrigin;
    [Tooltip("Transmission date of the log from planet of origin (RelativeMonth.Day)")]
    public Vector2 transmissionDate;
    [Tooltip("Log timestamp (for nondigital content, 'indeterminate')")]
    public string timestamp;
    [TextArea(1, 15), Tooltip("Blocks of text for text log, subtitles for audio log, or image description for image log")] 
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
