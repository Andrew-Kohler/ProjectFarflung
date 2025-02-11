using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class FileTabController : MonoBehaviour
{
    [SerializeField, Tooltip("The master list of the contents of all the logs")]
    private LogList _masterList;
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

    [Header("Image Log Display")]
    [SerializeField, Tooltip("Parent of image display")]
    private GameObject _imgDisplayParent;
    [SerializeField, Tooltip("Text desc of image")]
    private TextMeshProUGUI _imgText;
    [SerializeField, Tooltip("Image for putting log sprite into")]
    private Image _img;

    private List<FileNode> _nodeDisplayList;
    private float _fileNodeDistance = 120f;
    private Log _selectedLog;

    private InputAction _arrowAction;

    // Some control variables
    private bool _isActiveCoroutine;
    private bool _isLogOpen;
    private int _textLogIndex;
    private IEnumerator _currentTimelineCoroutine;
    private void OnEnable()
    {
        InputSystem.actions.FindAction("Arrows").started += context => FileInteraction();
        TabOpen();
    }

    private void OnDisable()
    {
        InputSystem.actions.FindAction("Arrows").started -= context => FileInteraction();
    }
    void Start()
    {
        // Arrow keys
        _arrowAction = InputSystem.actions.FindAction("Arrows");
        _arrowAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFileText();
    }

    /// <summary>
    /// Handles all arrow key inputs on the file tab.
    /// </summary>
    private void FileInteraction()
    {
        Vector2 arrowInput = _arrowAction.ReadValue<Vector2>();
        if (!_isActiveCoroutine && this.isActiveAndEnabled)
        {
            if (arrowInput.x > 0 && GameManager.Instance.SceneData.LogIndex < GameManager.Instance.SceneData.LogUnlocks.Length - 1) // Moving to the right
            {
                // Start the change animations and change the index
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex++;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();

                // Move the timeline
                MoveTimeline();

                // Move the file nodes
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x - _fileNodeDistance, _scrollTime));
            }
            else if (arrowInput.x < 0 && GameManager.Instance.SceneData.LogIndex > 0) // Moving to the left
            {
                // Start the change animations and change the index
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex--;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();

                MoveTimeline();

                // Move the file nodes
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x + _fileNodeDistance, _scrollTime));
            }

            else if (arrowInput.y > 0) // Closing a log
            {
                if (_isLogOpen)
                {
                    CurrentLogDisplay(false);
                    _isLogOpen = false;
                }
                
            }
            else if (arrowInput.y < 0) // Opening a log
            {
                if (!_isLogOpen)
                {
                    CurrentLogDisplay(true);
                    _isLogOpen = true;
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
                }
                
            }
        }
    }

    // Show or close the selected log
    private void CurrentLogDisplay(bool show)
    {
        if (!_isActiveCoroutine)
        {
            StartCoroutine(DoDisplayCurrentLog(show));
        }
    }

    // Closes the selected log in response to a left or right press (skipping the animation)
    private void CloseCurrentLogFast()
    {
        _fileDisplayAnim.Play("Static");
        _textLogDisplayParent.SetActive(false);
        _imgDisplayParent.SetActive(false);
        _isLogOpen = false;
    }

    // Updates the metadata text on the side of the screen, and also keeps track of which log is selected
    private void UpdateFileText()
    {
        int currentMasterIndex = _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].masterIndex; // The index of the contents of the log
        _selectedLog = _masterList.logList[currentMasterIndex]; // The contents of the log
        string date = "3.604." + _selectedLog.date.x + "." + _selectedLog.date.y;
        _sideText.text = ">filename\n\"" + _selectedLog.filename + "\"\n>datestamp\n" + date + "\n>timestamp\n" + _selectedLog.timestamp + "\n>filetype " + _selectedLog.type;
    }

    // Converts the date stamp of the current log into the X position
    private void MoveTimeline()
    {
        float convertedDate = _masterList.logList[GameManager.Instance.SceneData.LogIndex].date.x * 100 + _masterList.logList[GameManager.Instance.SceneData.LogIndex].date.y;
        if(convertedDate >= 201)
        {
            convertedDate -= 49;
        }
        float convertedXPos = math.remap(_timelineDateBounds.x, _timelineDateBounds.y, _timelineHUDBounds.x, _timelineHUDBounds.y, convertedDate);
        if (_currentTimelineCoroutine != null)
            StopCoroutine(_currentTimelineCoroutine);

        _currentTimelineCoroutine = DoLerpXConstant(_timeline, convertedXPos);
        StartCoroutine(_currentTimelineCoroutine);
    }

    /// <summary>
    /// Used for moving the list left and right.
    /// </summary>
    private IEnumerator DoLerpXTimed(GameObject obj, float startX, float endX, float duration)
    {
        _isActiveCoroutine = true; // Prevent anything (like opening a log mid switch) from overlapping this
        float timeElapsed = 0;
        CloseCurrentLogFast();

        while (timeElapsed < duration)
        {
            obj.transform.localPosition = new Vector2(Mathf.Lerp(startX, endX, timeElapsed / duration), obj.transform.localPosition.y);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = new Vector2(endX, obj.transform.localPosition.y);

        _isActiveCoroutine = false;
    }

    private IEnumerator DoLerpXConstant(GameObject obj, float endX)
    {
        float startX = obj.transform.localPosition.x;
        float timeElapsed = 0f;

        while (Mathf.Abs(obj.transform.localPosition.x - endX) > .05f)
        {
            obj.transform.localPosition = new Vector2(Mathf.Lerp(startX, endX, timeElapsed/_timelineSpeed), obj.transform.localPosition.y);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = new Vector2(endX, obj.transform.localPosition.y);
    }

    /// <summary>
    /// Used for playing an animation to either show or hide the contents of a log.
    /// </summary>
    private IEnumerator DoDisplayCurrentLog(bool show)
    {
        _isActiveCoroutine = true; // Prevent anything from overlapping this

        // If we're closing the log, hide the content before the animation
        if (!show)
        {
            if (_selectedLog.type == Log.LogType.Text)
            {
                _textLogDisplayParent.SetActive(false);
            }
            else if (_selectedLog.type == Log.LogType.Image)
            {
                _imgDisplayParent.SetActive(false);
            }
        }

        // Play the animation
        if (show)
            _fileDisplayAnim.Play("ShowLog");
        else
            _fileDisplayAnim.Play("CloseLog");

        yield return new WaitForSeconds(5f / 6f);

        // If we're opening a log, show the content after the animation
        if (show)
        {
            if (_selectedLog.type == Log.LogType.Text) // If it's a text log, make the text shown the first segment of the log
            {
                _textLogDisplayParent.SetActive(true);
                _textLogIndex = 0;
                _textLogText.text = _selectedLog.textParas[_textLogIndex];
                _textLogPageCounter.text = "1/" + _selectedLog.textParas.Count;
            }
            else if (_selectedLog.type == Log.LogType.Image)
            {
                _imgDisplayParent.SetActive(true);
                _imgText.text = _selectedLog.textParas[0];
                _img.sprite = _selectedLog.visual;
            }
        }
        
        _isActiveCoroutine = false;
    }

    /// <summary>
    /// Refreshes the file list when the tab is enabled in case new logs were picked up.
    /// Called on enable.
    /// </summary>
    private void TabOpen()
    {

        if(_fileNodeParent.transform.childCount == 0) {
            float currentXPosition = 0;
            _nodeDisplayList = new List<FileNode>(); // Initialize the list since OnEnable happens before it would get its default

            for (int i = 0; i < GameManager.Instance.SceneData.LogUnlocks.Length; i++)
            {
                if (GameManager.Instance.SceneData.LogUnlocks[i]) // If we have this log...
                {
                    // Instantiate a File Node prefab at a position
                    GameObject newNode = Instantiate(_fileNodePrefab, _fileNodeParent.transform, false);
                    newNode.transform.localPosition = new Vector3(currentXPosition, 0f, 0f);
                    // Get the File Node component and add it to our display list; tell it which log it correlates to
                    FileNode newNodeScript = newNode.GetComponent<FileNode>();
                    _nodeDisplayList.Add(newNodeScript);
                    newNodeScript.masterIndex = i;

                    // Set the correct size of this log because for some reason I wanted this detail
                    if (GameManager.Instance.SceneData.LogIndex != _nodeDisplayList.Count - 1)
                    {
                        newNodeScript.SetSmall();
                    }

                    // Make sure we spawn the next file node at the right place
                    currentXPosition += _fileNodeDistance;
                }
            }

            // Once all the nodes are spawned, we need to make sure the player's selection is in the same place as it was before
            _fileNodeParent.transform.localPosition = new Vector3(-_fileNodeDistance * GameManager.Instance.SceneData.LogIndex, 195, 0);
        }
        else
        {
            // Since default animation onEnable is to be large, we still need to make everyone small
            for(int i = 0; i < _nodeDisplayList.Count; i++)
            {
                if (GameManager.Instance.SceneData.LogIndex != i)
                {
                    _nodeDisplayList[i].SetSmall();
                }
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
