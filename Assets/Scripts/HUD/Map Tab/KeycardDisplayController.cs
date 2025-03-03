using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class KeycardDisplayController : MonoBehaviour
{
    [SerializeField, Tooltip("Parent transform of the keys")] 
    private GameObject _keyParent;
    private HorizontalLayoutGroup _hlg;
    [SerializeField, Tooltip("Prefab of keycard graphic")]
    private GameObject _keyPrefab;
    [SerializeField, Tooltip("Selection icon")]
    private GameObject _selector;
    [SerializeField, Tooltip("Graphic information associated with keycards")]
    public List<string> _keycardInfoStrings;
    public List<Color> _keycardInfoColors;
    public List<Sprite> _keycardInfoSprites;
    [SerializeField, Tooltip("Icon of selected key")]
    private Image _selectedKeyIcon;
    [SerializeField, Tooltip("Text of selected key")]
    private TextMeshProUGUI _selectedKeyText;

    private List<GameObject> _keyObjects; // For moving the selector from one to another
    private float _localKeyCount = 0;   // Used for seeing if there's a difference in GameManager & updating the visual here
    private int _currentIndex = 0;      // What key is currently selected

    // move input actions
    private InputAction _upArrow;
    private InputAction _rightArrow;
    private InputAction _downArrow;
    private InputAction _leftArrow;

    private void OnEnable()
    {
        // bind input updating
        _upArrow = InputSystem.actions.FindAction("HUDUp");
        _upArrow.started += context => KeyInteraction();
        _upArrow.Enable();
        _rightArrow = InputSystem.actions.FindAction("HUDRight");
        _rightArrow.started += context => KeyInteraction();
        _rightArrow.Enable();
        _downArrow = InputSystem.actions.FindAction("HUDDown");
        _downArrow.started += context => KeyInteraction();
        _downArrow.Enable();
        _leftArrow = InputSystem.actions.FindAction("HUDLeft");
        _leftArrow.started += context => KeyInteraction();
        _leftArrow.Enable();
    }

    private void OnDisable()
    {
        // unbind input updating
        _upArrow.started -= context => KeyInteraction();
        _rightArrow.started -= context => KeyInteraction();
        _downArrow.started -= context => KeyInteraction();
        _leftArrow.started -= context => KeyInteraction();
    }

    void Start()
    {
        _hlg = _keyParent.GetComponent<HorizontalLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateKeyList();
    }
    
    private void UpdateKeyList()
    {
        if(_localKeyCount != GameManager.Instance.SceneData.Keys.Count)
        {
            TabOpen();
        }
    }

    private void KeyInteraction()
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

        if (arrowInput.x > 0)
        {
            _currentIndex++;

            if (_currentIndex > GameManager.Instance.SceneData.Keys.Count - 1)
            {
                _currentIndex = GameManager.Instance.SceneData.Keys.Count - 1; // Infinite scrolling is NOT enabled, that's too easy and useful for a horror interface
            }
            else
            {
                UpdateSelection();
            }
        }
        else if (arrowInput.x < 0)
        {
            _currentIndex--;
            if (_currentIndex < 0)
            {
                _currentIndex = 0;
            }
            else
            {
                UpdateSelection();
            }
        }
    }

    private void UpdateSelection()
    {
        _selector.transform.position = _keyObjects[_currentIndex].transform.position;
        _selector.SetActive(true);
        _selectedKeyText.text = GameManager.Instance.SceneData.Keys[_currentIndex];
        _selectedKeyIcon.sprite =
            _keycardInfoSprites[_keycardInfoStrings.FindIndex(x => x == GameManager.Instance.SceneData.Keys[_currentIndex])];
    }

    public void TabOpen()
    {
        if (_localKeyCount != GameManager.Instance.SceneData.Keys.Count)
        {
            _localKeyCount = GameManager.Instance.SceneData.Keys.Count; // Update local count
            foreach (Transform child in _keyParent.transform)           // Empty the list from last time
            {
                Destroy(child.gameObject);
            }

            _keyObjects = new List<GameObject>();
            _hlg = _keyParent.GetComponent<HorizontalLayoutGroup>();
            _hlg.enabled = true;

            for (int i = 0; i < GameManager.Instance.SceneData.Keys.Count; i++)
            {
                // Instantiate a Key prefab at a position
                GameObject newKey = Instantiate(_keyPrefab, _keyParent.transform, false);
                _keyObjects.Add(newKey);

                // Set the key's color corresponding to what it is
                int currentInspectorIndex = _keycardInfoStrings.FindIndex(x => x == GameManager.Instance.SceneData.Keys[i]);
                newKey.GetComponent<Image>().color = _keycardInfoColors[currentInspectorIndex];

            }
            
            StartCoroutine(DoTabOpenEnd());
            
        }
        
    }

    private IEnumerator DoTabOpenEnd()
    {
        yield return new WaitForEndOfFrame();
        _hlg.enabled = false;
        if (_keyObjects.Count > 0)
        {
            UpdateSelection();
            
        }
        else
        {
            _selector.SetActive(false);
            _selectedKeyText.text = "no access keys";
        }
    }
}
