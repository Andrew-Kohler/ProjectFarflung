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

    [Header("Keybinds/Accessiblity")]
    [SerializeField, Tooltip("Prefabs for left and right arrow at the end of the list")]
    private GameObject _leftArrowBind;
    [SerializeField] private GameObject _rightArrowBind;

    private int _lowestIndex = 0;
    private int _highestIndex;

    private List<GameObject> _keyObjects; // For moving the selector from one to another
    private int _localKeyCount = 0;   // Used for seeing if there's a difference in GameManager & updating the visual here
    private int _currentIndex = 0;      // What key is currently selected

    #region Controls Bindings
    // move input actions
    private InputAction _upArrow;
    private InputAction _rightArrow;
    private InputAction _downArrow;
    private InputAction _leftArrow;

    private void OnEnable()
    {
        // bind input updating
        _upArrow = InputSystem.actions.FindAction("HUDUp");
        _upArrow.started += DoKeyInteraction;
        _upArrow.Enable();
        _rightArrow = InputSystem.actions.FindAction("HUDRight");
        _rightArrow.started += DoKeyInteraction;
        _rightArrow.Enable();
        _downArrow = InputSystem.actions.FindAction("HUDDown");
        _downArrow.started += DoKeyInteraction;
        _downArrow.Enable();
        _leftArrow = InputSystem.actions.FindAction("HUDLeft");
        _leftArrow.started += DoKeyInteraction;
        _leftArrow.Enable();
    }

    private void OnDisable()
    {
        // unbind input updating
        _upArrow.started -= DoKeyInteraction;
        _rightArrow.started -= DoKeyInteraction;
        _downArrow.started -= DoKeyInteraction;
        _leftArrow.started -= DoKeyInteraction;
    }

    /// <summary>
    /// Simply calls KeyInteraction() function.
    /// Necessary to avoid memory leak.
    /// </summary>
    private void DoKeyInteraction(InputAction.CallbackContext context)
    {
        KeyInteraction();
    }
    #endregion

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

            if (_currentIndex > _highestIndex)
            {
                _currentIndex = _highestIndex; // Infinite scrolling is NOT enabled, that's too easy and useful for a horror interface
            }
            else
            {
                // general HUD SFX
                AudioManager.Instance.PlayGeneralSoundHUD();

                UpdateSelection();
            }
        }
        else if (arrowInput.x < 0)
        {
            _currentIndex--;
            if (_currentIndex < _lowestIndex)
            {
                _currentIndex = _lowestIndex;
            }
            else
            {
                // general HUD SFX
                AudioManager.Instance.PlayGeneralSoundHUD();

                UpdateSelection();
            }
        }
    }

    private void UpdateSelection()
    {
        _selector.transform.position = _keyObjects[_currentIndex].transform.position;
        _selector.SetActive(true);
        if(_keyObjects.Count >= 2)
        {
            _selectedKeyText.text = GameManager.Instance.SceneData.Keys[_currentIndex - 1];
            _selectedKeyIcon.sprite =
            _keycardInfoSprites[_keycardInfoStrings.FindIndex(x => x == GameManager.Instance.SceneData.Keys[_currentIndex - 1])];
        }
        else
        {
            _selectedKeyText.text = GameManager.Instance.SceneData.Keys[_currentIndex];
            _selectedKeyIcon.sprite =
            _keycardInfoSprites[_keycardInfoStrings.FindIndex(x => x == GameManager.Instance.SceneData.Keys[_currentIndex])];
        }
        
        
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

            if(_localKeyCount >= 2) // If we have at least 2 keys, we need the left and right arrows to be added to the list
            {
                _lowestIndex = 1; // Set the lowest and highest navigable incidies correctly so the player can't select the arrows
                
                GameObject newArrow1 = Instantiate(_leftArrowBind, _keyParent.transform, false);
                _keyObjects.Add(newArrow1);

                if (_currentIndex < _lowestIndex) // Taking care that any changes in highest/lowest indices don't break anything
                {
                    _currentIndex = _lowestIndex;
                }
            }

            for (int i = 0; i < GameManager.Instance.SceneData.Keys.Count; i++)
            {
                // Instantiate a Key prefab at a position
                GameObject newKey = Instantiate(_keyPrefab, _keyParent.transform, false);
                _keyObjects.Add(newKey);

                // Set the key's color corresponding to what it is
                int currentInspectorIndex = _keycardInfoStrings.FindIndex(x => x == GameManager.Instance.SceneData.Keys[i]);
                newKey.GetComponent<Image>().color = _keycardInfoColors[currentInspectorIndex];
            }

            if (_localKeyCount >= 2)
            {
                GameObject newArrow2 = Instantiate(_rightArrowBind, _keyParent.transform, false);
                _keyObjects.Add(newArrow2);
                _highestIndex = _keyObjects.Count - 2;

                if (_currentIndex > _highestIndex)
                {
                    _currentIndex = _highestIndex;
                }
            }


            UpdateSelection();
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
