using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class FileTabController : MonoBehaviour
{
    [SerializeField, Tooltip("The prefab for a file node")]
    private GameObject _fileNodePrefab;
    [SerializeField,Tooltip("The parent object of the file nodes")] 
    private GameObject _fileNodeParent;
    [SerializeField, Tooltip("Scroll time for moving between logs (= to animation time for grow/shrink)")]
    private float _scrollTime = .25f;
    [SerializeField, Tooltip("Metadata text for files")]
    private TextMeshProUGUI _sideText;
    [SerializeField, Tooltip("Animator for showing files")]
    private Animator _fileDisplayAnim;
    [SerializeField, Tooltip("The text explaining how to open a log")]
    private GameObject _downKeybindOpenLog;
    [SerializeField, Tooltip("The text explaining how to close a log")]
    private GameObject _upKeybindCloseLog;
    [SerializeField, Tooltip("The text explaining how to navigate left and right in the log list")]
    private GameObject _lrKeybindNavLog;

    [Header("Hard Drive Capacity")]
    [SerializeField, Tooltip("Made up file sizes for text, audio, and image logs")]
    private Vector3 _fileSizes;
    [SerializeField, Tooltip("Sum of all our made up file size numbers")]
    private float _maxDataCapacity;
    [SerializeField, Tooltip("Sum of all our made up file size numbers")]
    private TextMeshProUGUI _dataCapacityText;
    [SerializeField, Tooltip("Fill bar for our made up file size sum")]
    private Image _dataCapacityMeter;
    private float _currentDataSum;

    [Header("Timeline")]
    [SerializeField, Tooltip("Timeline object")]
    private GameObject _timeline;
    [SerializeField, Tooltip("How fast the timeline moves")]
    private float _timelineSpeed = .5f;
    [SerializeField, Tooltip("The furthest and most recent datestamp values of our logs")]
    private Vector2 _timelineDateBounds;
    [SerializeField, Tooltip("The start and end X values that the timeline can move to on the HUD")]
    private Vector2 _timelineHUDBounds;

    [Header("Text Log Display")]
    [SerializeField, Tooltip("Parent of log display")]
    private GameObject _textLogDisplayParent;
    [SerializeField, Tooltip("Text content of log")]
    private TextMeshProUGUI _textLogText;
    [SerializeField, Tooltip("Page counter of log")]
    private TextMeshProUGUI _textLogPageCounter;
    [SerializeField, Tooltip("Keybind text for scrolling")]
    private GameObject _downKeybindScroll;

    [Header("Image Log Display")]
    [SerializeField, Tooltip("Parent of image display")]
    private GameObject _imgDisplayParent;
    [SerializeField, Tooltip("Text desc of image")]
    private TextMeshProUGUI _imgText;
    [SerializeField, Tooltip("Image for putting log sprite into")]
    private Image _img;

    [Header("Audio Log Display")]
    [SerializeField, Tooltip("Parent of audio display")]
    private GameObject _audioDisplayParent;
    [SerializeField, Tooltip("Subtitle text")]
    private TextMeshProUGUI _subText;
    [SerializeField, Tooltip("Progress slider")]
    private Slider _timeSlider;
    [SerializeField, Tooltip("Time remaining text")]
    private TextMeshProUGUI _timeRemainingText;
    [SerializeField, Tooltip("Waveform parent")]
    private GameObject _waveformParent;
    [SerializeField, Tooltip("Waveform scale speed")]
    private float _waveformSpeed = 10f;
    [SerializeField, Tooltip("Waveform sensitivity")]
    private float _waveformSensitivity = 2f;
    [SerializeField, Tooltip("Waveform color gradient")]
    private Gradient _gradient;

    // Audio components
    private AudioSource _source;    
    private List<GameObject> _bars; // Bars of the waveform
    private List<Image> _barsImg;   // Image component of waveform bars
    private float[] spectrum;       // Array that holds the live audio data

    // File list components
    private List<FileNode> _nodeDisplayList;
    private float _fileNodeDistance = 120f; // How far apart the file nodes are from each other
    private Log _selectedLog;

    // Some control variables
    private bool _isActiveCoroutineUpDown; // Prevents navigation coroutines from overlapping
    private bool _isActiveCoroutineLeftRight; // Prevents navigation coroutines from overlapping
    private bool _isLogOpen;         // Is there a log open?
    private int _textLogIndex;       // What page of an open text log we're on 
    private int _localFileCount = 0;     // The last local count of how many files there are (if there's a difference, list gets updated)
    private Log _lastSelected = null;    // Used to hold the last log the player was looking at so that new additions don't move the menu around
                                         // Necessary because selectedLog can be affected by things other than direct player input.
    private IEnumerator _currentTimelineCoroutine;
    private IEnumerator _currentSubtitleCoroutine;
    private IEnumerator _currentOpenCloseCoroutine;

    #region Controls Binding
    // move input actions
    private InputAction _upArrow;
    private InputAction _rightArrow;
    private InputAction _downArrow;
    private InputAction _leftArrow;

    private void OnEnable()
    {
        // bind input updating
        _upArrow = InputSystem.actions.FindAction("HUDUp");
        _upArrow.started += DoFileInteraction;
        _upArrow.Enable();
        _rightArrow = InputSystem.actions.FindAction("HUDRight");
        _rightArrow.started += DoFileInteraction;
        _rightArrow.Enable();
        _downArrow = InputSystem.actions.FindAction("HUDDown");
        _downArrow.started += DoFileInteraction;
        _downArrow.canceled += ResetAudioPitch;
        _downArrow.Enable();
        _leftArrow = InputSystem.actions.FindAction("HUDLeft");
        _leftArrow.started += DoFileInteraction;
        _leftArrow.Enable();

        TabOpen();
    }

    private void OnDisable()
    {
        // unbind input updating
        _upArrow.started -= DoFileInteraction;
        _rightArrow.started -= DoFileInteraction;
        _downArrow.started -= DoFileInteraction;
        _downArrow.canceled -= ResetAudioPitch;
        _leftArrow.started -= DoFileInteraction;

        TabClose();
    }

    /// <summary>
    /// simply calls the FileInteraction function.
    /// Necessary to avoid memory leak.
    /// </summary>
    private void DoFileInteraction(InputAction.CallbackContext context)
    {
        FileInteraction();
    }
    #endregion

    void Start()
    {
        // Audio tab setup
        _source = GetComponent<AudioSource>();
        _bars = new List<GameObject>();
        _barsImg = new List<Image>();
        foreach (Transform child in _waveformParent.transform)
        {
            _bars.Add(child.gameObject);
            _barsImg.Add(child.GetComponent<Image>());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFileList();
        UpdateFileText();
        if (_source.isPlaying)
        {
            UpdateAudioSlider();
            UpdateAudioWaveform();
        }

    }

    /// <summary>
    /// Handles all arrow key inputs on the file tab.
    /// </summary>
    private void FileInteraction()
    {
        // Read inputs
        int xInput = 0;
        int yInput = 0;
        if (_rightArrow.ReadValue<float>() > 0.5f)
            xInput++;
        if (_leftArrow.ReadValue<float>() > 0.5f)
            xInput--;
        if (_upArrow.ReadValue<float>() > 0.5f)
            yInput++;
        if (_downArrow.ReadValue<float>() > 0.5f)
            yInput--;
        Vector2 arrowInput = new Vector2(xInput, yInput);

        // If a movement coroutine isn't running, this tab is open, and there's at least one log to look at
        if (this.isActiveAndEnabled && GameManager.Instance.SceneData.FoundLogNames.Count > 0 && !_isActiveCoroutineLeftRight)
        {
            if (arrowInput.x > 0 && GameManager.Instance.SceneData.LogIndex < GameManager.Instance.SceneData.FoundLogNames.Count - 1) // Moving to the right
            {
                if (_isActiveCoroutineUpDown) // Allows L/R presses to interrupt opening and closing animations
                {
                    StopCoroutine(_currentOpenCloseCoroutine);
                    //CloseCurrentLogFast();
                }

                // Start the change animations and change the index
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex++;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();
                _lastSelected = GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex];

                // Move the timeline
                MoveTimeline(false);

                // Move the file nodes
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x - _fileNodeDistance, _scrollTime));
            }
            else if (arrowInput.x < 0 && GameManager.Instance.SceneData.LogIndex > 0) // Moving to the left
            {

                if (_isActiveCoroutineUpDown)
                {
                    StopCoroutine(_currentOpenCloseCoroutine);
                    //CloseCurrentLogFast();
                }

                // Start the change animations and change the index
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex--;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();
                _lastSelected = GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex];

                MoveTimeline(false);

                // Move the file nodes
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x + _fileNodeDistance, _scrollTime));
            }

            else if (arrowInput.y > 0 && !_isActiveCoroutineUpDown) // Closing a log
            {
                if (_isLogOpen)
                {
                    CurrentLogDisplay(false);
                    _isLogOpen = false;
                }
                
            }
            else if (arrowInput.y < 0 && !_isActiveCoroutineUpDown) // Opening a log
            {
                if (!_isLogOpen) // If there's not a log open, open the log
                {
                    CurrentLogDisplay(true);
                    _isLogOpen = true;

                    // We have now accessed this log
                    _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].SetRead(true);
                    GameManager.Instance.SceneData.FoundLogReadStatus[GameManager.Instance.SceneData.LogIndex] = true;
                }
                else
                {
                    if (_selectedLog.type == Log.LogType.Text) // If it's a text log, down is also used to scroll through the log contents
                    {
                        _textLogIndex++;
                        if(_textLogIndex >= _selectedLog.textParas.Count)
                            _textLogIndex = 0;
                        _textLogText.text = _selectedLog.textParas[_textLogIndex];
                        _textLogPageCounter.text = (_textLogIndex + 1) + "/" + _selectedLog.textParas.Count;
                    }
                    if(_selectedLog.type == Log.LogType.Audio)
                    {
                        _source.pitch = 1.5f;
                    }
                    
                }
                
            }

            

        }
    }

    private void ResetAudioPitch(InputAction.CallbackContext context)
    {
        _source.pitch = 1f;
    }

    // Listens for a change in the file count when the file tab is open
    private void UpdateFileList()
    {
        if(_localFileCount != GameManager.Instance.FoundLogs.Count)
        {
            TabOpen();
        }

        if (GameManager.Instance.FoundLogs.Count >= 2)
        {
            _lrKeybindNavLog.SetActive(true);
        }
    }

    // Updates the metadata text on the side of the screen, and also keeps track of which log is selected
    private void UpdateFileText()
    {
        // If there's a log for us to update the text from
        if(GameManager.Instance.FoundLogs.Count > 0)
        {
            _selectedLog = GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex]; // Set a variable for accessing the currently selected log

            // Format the text string correctly depending on if a log originates from the station or offworld, or has no datestamp
            string date = "3.604." + _selectedLog.date.x + "." + _selectedLog.date.y;
            if (!_selectedLog.hasDate)
            {
                _sideText.text = ">filename\n\"" + _selectedLog.filename + "\"\n>datestamp unknown" + "\n>filetype " + _selectedLog.type;
            }
            else
            {
                if (_selectedLog.offworldOrigin)
                {
                    string offworldDate = "3.604." + _selectedLog.transmissionDate.x + "." + _selectedLog.transmissionDate.y;
                    _sideText.text = ">filename\n\"" + _selectedLog.filename + "\"\n>fileorigin " + _selectedLog.planetOfOrigin +
                        "\n>sent\n" + offworldDate + "\n>received\n" + date + "\n>timestamp\n" + _selectedLog.timestamp + "\n>filetype " + _selectedLog.type;
                }
                else
                {
                    _sideText.text = ">filename\n\"" + _selectedLog.filename + "\"\n>datestamp\n" + date + "\n>timestamp\n" + _selectedLog.timestamp + "\n>filetype " + _selectedLog.type;
                }
            }
        }
        else
        {
            _sideText.text = ">no readable files in memory";
        }
        
    }

    #region AUDIO LOG METHODS
    // Updates the audio waveform graphic when an audio log plays.
    private void UpdateAudioWaveform()
    {
            // Get the waveform data (1024 = sample rate)
            spectrum = new float[1024];
            _source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

            for (int i = 0; i < _bars.Count; i++) //needs to be <= sample rate
            {
                float level = spectrum[i] * _waveformSensitivity * Time.deltaTime * 1000; 
                
                // Scale bars
                Vector3 previousScale = _bars[i].transform.localScale;
                previousScale.y = Mathf.Lerp(previousScale.y, level, _waveformSpeed * Time.deltaTime);
                if (previousScale.y < 0.02f)
                    previousScale.y = .02f;

                // Color the bars based on their size (because it looks cool)
                _bars[i].transform.localScale = previousScale;
                float gradientPercent = previousScale.y * 2;
                if (gradientPercent > 1) gradientPercent = 1;
                _barsImg[i].color = _gradient.Evaluate(gradientPercent);
            }
    }

    private void UpdateAudioSlider()
    {
            // Update the slider
            _timeSlider.value = (_source.time / _source.clip.length);

            // Update time remaining
            int minRemaining = (int)((_source.clip.length - _source.time)/ 60);
            int secRemaining = (int)(_source.clip.length - (minRemaining * 60) - _source.time);
            string timeString = "";
            if (minRemaining < 10)
                timeString += "0";
            timeString += minRemaining + ":";
            if (secRemaining < 10)
                timeString += "0";
            timeString += secRemaining;
            _timeRemainingText.text = timeString;
    }

    private IEnumerator DoAudioSubtitles()
    {
        for (int i = 0; i < _selectedLog.subtitleTiming.Count; i++)
        {
            _subText.text = _selectedLog.textParas[i];
            
            if (i + 1 < _selectedLog.subtitleTiming.Count)
            {
                yield return new WaitUntil(() => _source.time >= _selectedLog.subtitleTiming[i + 1]);
            }

        }
    }
    #endregion

    /// <summary>
    /// Show or close the selected log; method that routes to a coroutine
    /// </summary>
    private void CurrentLogDisplay(bool show)
    {
        if (!_isActiveCoroutineUpDown)
        {
            _currentOpenCloseCoroutine = DoDisplayCurrentLog(show);
            StartCoroutine(_currentOpenCloseCoroutine);
        }
    }

    /// <summary>
    /// Used for playing an animation to either show or hide the contents of a log.
    /// </summary>
    private IEnumerator DoDisplayCurrentLog(bool show)
    {
        _isActiveCoroutineUpDown = true; // Prevent anything from overlapping this

        // If we're closing the log, hide the content before the animation
        if (!show)
        {
            _upKeybindCloseLog.SetActive(false);
            if (_selectedLog.type == Log.LogType.Text)
            {
                _textLogDisplayParent.SetActive(false);
            }
            else if (_selectedLog.type == Log.LogType.Image)
            {
                _imgDisplayParent.SetActive(false);
            }
            else
            {
                _audioDisplayParent.SetActive(false);
                _timeSlider.value = 0;
                StopCoroutine(_currentSubtitleCoroutine);
                _source.Stop();
            }
        }
        else // When opening a log, hide the text explaining how to do so
        {
            _downKeybindOpenLog.SetActive(false);
        }

        // Play the animation
        if (show)
        {
            _fileDisplayAnim.Play("ShowLog");
            _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].ShowLog();
        }

        else
        {
            _fileDisplayAnim.Play("CloseLog");
            _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].CloseLog();
        }
            

        yield return new WaitForSeconds(5f / 6f); 

        // If we're opening a log, show the content after the animation
        if (show)
        {
            _upKeybindCloseLog.SetActive(true); // Show the text saying how to close the log
            if (_selectedLog.type == Log.LogType.Text) // If it's a text log, make the text shown the first segment of the log
            {
                _textLogDisplayParent.SetActive(true);
                _textLogIndex = 0;
                _textLogText.text = _selectedLog.textParas[_textLogIndex];
                _textLogPageCounter.text = "1/" + _selectedLog.textParas.Count;

                // If there isn't a need to scroll, don't show how
                if (_selectedLog.textParas.Count > 1)
                    _downKeybindScroll.SetActive(true);
                else
                    _downKeybindScroll.SetActive(false);
                
            }
            else if (_selectedLog.type == Log.LogType.Image)
            {
                _imgDisplayParent.SetActive(true);
                _imgText.text = _selectedLog.textParas[0];
                _img.sprite = _selectedLog.visual;
            }
            else // It's an audio log
            {
                _audioDisplayParent.SetActive(true);
                _source.clip = _selectedLog.audio;
                _source.Play();
                _currentSubtitleCoroutine = DoAudioSubtitles();
                StartCoroutine(_currentSubtitleCoroutine);
            }
        }
        else // If a log has finished closing, once again display the text saying how to open it
        {
            _downKeybindOpenLog.SetActive(true);
        }

        _isActiveCoroutineUpDown = false;
    }

    /// <summary>
    /// Closes the selected log instantly (skipping the animation)
    /// </summary>
    private void CloseCurrentLogFast()
    {
        // Hide all possible displays
        _textLogDisplayParent.SetActive(false);
        _imgDisplayParent.SetActive(false);
        _audioDisplayParent.SetActive(false);
        
        //Audio log specific resets
        _timeSlider.value = 0;
        if(_source.isPlaying)
            _source.Stop();
        if (_currentSubtitleCoroutine != null)
            StopCoroutine(_currentSubtitleCoroutine);

        // Internal variable / animation /text resets
        _downKeybindOpenLog.SetActive(true);
        _upKeybindCloseLog.SetActive(false);
        if(_fileDisplayAnim.isActiveAndEnabled)
            _fileDisplayAnim.Play("Static");
        _isActiveCoroutineUpDown = false;
        _isLogOpen = false;
    }

    // Converts the date stamp of the current log into the X position and moves the timeline to that position
    private void MoveTimeline(bool instant)
    {
        if (GameManager.Instance.FoundLogs.Count > 0)
        {
            if (!GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex].hasDate) // If the selected log is undated, a timeline isn't very useful, now, is it
            {
                _timeline.SetActive(false);
            }
            else
            {
                _timeline.SetActive(true);

                float convertedDate = GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex].date.x * 100 + GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex].date.y;
                if (convertedDate >= 201)
                {
                    convertedDate -= 48;
                }
                float convertedXPos = math.remap(_timelineDateBounds.x, _timelineDateBounds.y, _timelineHUDBounds.x, _timelineHUDBounds.y, convertedDate);

                if (instant)
                {
                    _timeline.transform.localPosition = new Vector2(convertedXPos, _timeline.transform.localPosition.y);
                }
                else
                {
                    if (_currentTimelineCoroutine != null)
                        StopCoroutine(_currentTimelineCoroutine);

                    _currentTimelineCoroutine = DoLerpXConstant(_timeline, convertedXPos);
                    StartCoroutine(_currentTimelineCoroutine);
                }

            }
        }
        
    }

    #region LERP COROUTINES
    /// <summary>
    /// Used for moving the file list left and right.
    /// </summary>
    private IEnumerator DoLerpXTimed(GameObject obj, float startX, float endX, float duration)
    {
        _isActiveCoroutineLeftRight = true; // Prevent anything (like opening a log mid switch) from overlapping this
        float timeElapsed = 0;
        CloseCurrentLogFast();

        while (timeElapsed < duration)
        {
            obj.transform.localPosition = new Vector2(Mathf.Lerp(startX, endX, timeElapsed / duration), obj.transform.localPosition.y);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = new Vector2(endX, obj.transform.localPosition.y);

        _isActiveCoroutineLeftRight = false;
    }

    /// <summary>
    /// Used for moving the timeline left and right.
    /// </summary>
    private IEnumerator DoLerpXConstant(GameObject obj, float endX)
    {
        float startX = obj.transform.localPosition.x;
        //float timeElapsed = 0f;

        while (Mathf.Abs(obj.transform.localPosition.x - endX) > .05f)
        {
            //obj.transform.localPosition = new Vector2(Mathf.Lerp(startX, endX, timeElapsed/_timelineSpeed), obj.transform.localPosition.y);
            obj.transform.localPosition = Vector2.MoveTowards(obj.transform.localPosition, new Vector2(endX, obj.transform.localPosition.y), Time.deltaTime * _timelineSpeed * 100);
            //timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = new Vector2(endX, obj.transform.localPosition.y);
    }
    #endregion

    /// <summary>
    /// Refreshes the file list when the tab is enabled in case new logs were picked up.
    /// Called on enable.
    /// </summary>
    private void TabOpen()
    {
        if(_localFileCount == 0)  // Initialize the list since OnEnable happens before it would get its default
        {
            _nodeDisplayList = new List<FileNode>();
        }

        // Regenerates the list if there is a new log, OR if the list was left at an incorrect position / misaligned
        if(_localFileCount != GameManager.Instance.FoundLogs.Count || _isActiveCoroutineLeftRight)
        {
            _isActiveCoroutineLeftRight = false;
            if (_localFileCount == 0) // If this is the first log picked up, we need to set _lastSelected (as it's normally set when the arrow keys are pressed)
            {
                _lastSelected = GameManager.Instance.FoundLogs[GameManager.Instance.SceneData.LogIndex];
            }
            // Get the new index of the data that was previously selected so the player will have the same data selected as when they left
            if (_lastSelected != null)
            {
                GameManager.Instance.SceneData.LogIndex = GameManager.Instance.FoundLogs.FindIndex(x => x.filename == _lastSelected.filename);
            }

            _localFileCount = GameManager.Instance.FoundLogs.Count; // Update local count
            foreach(Transform child in _fileNodeParent.transform)   // Empty the list from last time
            {
                Destroy(child.gameObject);
            }

            float currentXPosition = 0;
            _currentDataSum = 0;
            _nodeDisplayList = new List<FileNode>();

            for (int i = 0; i < GameManager.Instance.FoundLogs.Count; i++)
            {
                // Instantiate a File Node prefab at a position
                GameObject newNode = Instantiate(_fileNodePrefab, _fileNodeParent.transform, false);
                newNode.transform.localPosition = new Vector3(currentXPosition, 0f, 0f);

                // Get the File Node component and add it to our display list; tell it which log it correlates to
                FileNode newNodeScript = newNode.GetComponent<FileNode>();
                _nodeDisplayList.Add(newNodeScript);

                // Set the type of the node corresponding to its contents
                newNodeScript.SetType(GameManager.Instance.FoundLogs[i].type);

                // Set this node's unread indicator to the correct position
                newNodeScript.SetRead(GameManager.Instance.SceneData.FoundLogReadStatus[i]);

                // Set the correct size of this log 
                if (GameManager.Instance.SceneData.LogIndex != _nodeDisplayList.Count - 1)
                {
                    newNodeScript.SetSmall();
                }

                // Add this log's "file size" to our file size counter
                if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Text)
                    _currentDataSum += _fileSizes.x;
                if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Audio)
                    _currentDataSum += _fileSizes.y;
                if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Image)
                    _currentDataSum += _fileSizes.z;

                // Make sure we spawn the next file node at the right place
                currentXPosition += _fileNodeDistance;
                
            }

            _dataCapacityText.text = ">freespace \n" + (_maxDataCapacity - _currentDataSum) + "/" + _maxDataCapacity + " GB";
            _dataCapacityMeter.fillAmount = _currentDataSum / _maxDataCapacity;

            _fileNodeParent.transform.localPosition = new Vector3(-_fileNodeDistance * GameManager.Instance.SceneData.LogIndex, 195, 0);

            MoveTimeline(true);
        }
        else
        {
            // Since default animation onEnable is to be large, we still need to make everyone small
            if(_nodeDisplayList.Count > 0)
            {
                MoveTimeline(true);
                _currentDataSum = 0;

                for (int i = 0; i < _nodeDisplayList.Count; i++)
                {
                    if (GameManager.Instance.SceneData.LogIndex != i)
                    {
                        _nodeDisplayList[i].SetSmall();
                    }

                    // Add this log's "file size" to our file size counter
                    if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Text)
                        _currentDataSum += _fileSizes.x;
                    if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Audio)
                        _currentDataSum += _fileSizes.y;
                    if (GameManager.Instance.FoundLogs[i].type == Log.LogType.Image)
                        _currentDataSum += _fileSizes.z;
                }

                _dataCapacityText.text = ">freespace \n" + (_maxDataCapacity - _currentDataSum) + "/" + _maxDataCapacity + " GB";
                _dataCapacityMeter.fillAmount = _currentDataSum / _maxDataCapacity;
            }
            
        }
        
    }

    /// <summary>
    /// Makes sure no files are left 'open'.
    /// Called on disable.
    /// </summary>
    private void TabClose()
    {
        CloseCurrentLogFast();
    }
}
