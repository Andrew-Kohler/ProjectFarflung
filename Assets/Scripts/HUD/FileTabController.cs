using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FileTabController : MonoBehaviour
{
    [SerializeField, Tooltip("The prefab for a file node")]
    private GameObject _fileNodePrefab;
    [SerializeField,Tooltip("The parent object of the file nodes")] 
    private GameObject _fileNodeParent;
    [SerializeField, Tooltip("Scroll time for moving between logs (= to animation time for grow/shrink)")]
    private float _scrollTime = .25f;

    private List<FileNode> _nodeDisplayList;
    private float _fileNodeDistance = 120f;

    private InputAction _arrowAction;

    private bool _isActiveCoroutine;
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
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex++;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x - _fileNodeDistance, _scrollTime));
            }
            else if (arrowInput.x < 0 && GameManager.Instance.SceneData.LogIndex > 0) // Moving to the left
            {
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Shrink();
                GameManager.Instance.SceneData.LogIndex--;
                _nodeDisplayList[GameManager.Instance.SceneData.LogIndex].Grow();
                StartCoroutine(DoLerpXTimed(_fileNodeParent, _fileNodeParent.transform.localPosition.x, _fileNodeParent.transform.localPosition.x + _fileNodeDistance, _scrollTime));
            }

            else if (arrowInput.y > 0) // Closing a log
            {

            }
            else if (arrowInput.y < 0) // Opening a log
            {

            }
        }
        
        
    }

    /// <summary>
    /// Used for moving the list left and right.
    /// </summary>
    private IEnumerator DoLerpXTimed(GameObject obj, float startX, float endX, float duration)
    {
        _isActiveCoroutine = true;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            obj.transform.localPosition = new Vector2(Mathf.Lerp(startX, endX, timeElapsed / duration), obj.transform.localPosition.y);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        obj.transform.localPosition = new Vector2(endX, obj.transform.localPosition.y);

        _isActiveCoroutine = false;
    }

    /// <summary>
    /// Refreshes the file list when the tab is enabled in case new logs were picked up.
    /// Called on enable.
    /// </summary>
    private void TabOpen()
    {
        float currentXPosition = 0;
        _nodeDisplayList = new List<FileNode>(); // Initialize the list since OnEnable happens before it would get its default


        for(int i = 0; i < GameManager.Instance.SceneData.LogUnlocks.Length; i++)
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
                if(GameManager.Instance.SceneData.LogIndex != _nodeDisplayList.Count - 1)
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

    /// <summary>
    /// Makes sure no files are left 'open'.
    /// Called on disable.
    /// </summary>
    private void TabClose()
    {

    }
}
