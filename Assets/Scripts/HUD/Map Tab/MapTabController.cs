using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class MapTabController : MonoBehaviour
{
    [Header("HUD Controller")]
    [SerializeField] private HUDController _mainHUD;
    [SerializeField] private GameObject _mapTabContents; 

    private float _baselinePlayerHeight;

    [Header("Position Dot")]
    [SerializeField] private MapRaycaster _dotScript;

    [Header("Supplemental Text")]
    [SerializeField] private TextMeshProUGUI _coordsText;
    [SerializeField] private TextMeshProUGUI _locationText;
    [SerializeField] private TextMeshProUGUI _flavorText;
    private bool _isFlavorAnimRunning = false;

    [Header("Ranges of Motion")]
    // The X and Y (or X and Z) ranges that (a) the dot can move within in the HUD and (b) the player can move within in the game
    // Format of -x to x, -y to y / -x to x, -z to z
    [SerializeField] private Vector4 _1FHUDRange;
    [SerializeField] private Vector4 _1FGameRange;
    [SerializeField] private Vector4 _2FHUDRange;
    [SerializeField] private Vector4 _2FGameRange;
    [SerializeField] private Vector4 _3FHUDRange;
    [SerializeField] private Vector4 _3FGameRange;

    [Header("Floor Lists & Floor Switching")]
    [SerializeField] private List<GameObject> _floorGameObjects;
    [SerializeField] private List<GameObject> _floorNumbers;
    [SerializeField] private GameObject _floorSelector;

    [Header("Room Image Lists")]
    [SerializeField] private List<Image> _1FHUDImages;
    [SerializeField] private List<Image> _2FHUDImages;
    [SerializeField] private List<Image> _3FHUDImages;

    private int _currentFloor;      // The floor the player is on
    private int _currentHUDFloor;   // The floor the HUD is displaying

    // game manager vars
    private int _visitationL1Length; // Kept because OnDisable tab cleanup calls the GameManager after it's destroyed
    private int _visitationL2Length;
    private int _visitationL3Length;

    #region Controls Bindings
    // move input actions
    private InputAction _upArrow;
    private InputAction _rightArrow;
    private InputAction _downArrow;
    private InputAction _leftArrow;

    private void OnEnable()
    {
        MapRaycaster.onPassthrough += UpdateHUDMap;

        // bind input updating
        _upArrow = InputSystem.actions.FindAction("HUDUp");
        _upArrow.started += DoUpdateFloor;
        _upArrow.Enable();
        _rightArrow = InputSystem.actions.FindAction("HUDRight");
        _rightArrow.started += DoUpdateFloor;
        _rightArrow.Enable();
        _downArrow = InputSystem.actions.FindAction("HUDDown");
        _downArrow.started += DoUpdateFloor;
        _downArrow.Enable();
        _leftArrow = InputSystem.actions.FindAction("HUDLeft");
        _leftArrow.started += DoUpdateFloor;
        _leftArrow.Enable();
    }

    private void OnDisable()
    {
        MapRaycaster.onPassthrough -= UpdateHUDMap;

        // unbind input updating
        _upArrow.started -= DoUpdateFloor;
        _rightArrow.started -= DoUpdateFloor;
        _downArrow.started -= DoUpdateFloor;
        _leftArrow.started -= DoUpdateFloor;
    }

    /// <summary>
    /// Simply calls UpdateFloor function.
    /// Necessary to avoid memory leak.
    /// </summary>
    private void DoUpdateFloor(InputAction.CallbackContext context)
    {
        if(_mapTabContents.activeSelf)
            UpdateFloor();
    }
    #endregion

    void Start()
    {
        // Lore height of the floors
        _baselinePlayerHeight = _mainHUD.PlayerTransform.position.y;

        // Update the floor we're on, the floor the HUD is on, and the floor being shown by the HUD
        _currentFloor = GameManager.Instance.SceneData.Floor;
        _currentHUDFloor = _currentFloor;
        _floorSelector.transform.position = _floorNumbers[_currentHUDFloor - 1].transform.position;
        _floorGameObjects[_currentHUDFloor - 1].gameObject.SetActive(true);

        _visitationL1Length = GameManager.Instance.SceneData.VisitationList1F.Length;
        _visitationL2Length = GameManager.Instance.SceneData.VisitationList2F.Length;
        _visitationL3Length = GameManager.Instance.SceneData.VisitationList3F.Length;
    }

    #region Map Zones
    /// <summary>
    /// Configures map zone active states to match states stored in game manager
    /// </summary>
    public void RevealHUDMapStart() // On scene start, show everything the player has been able to explore
    {
        // Reveals all explored images so the player can still see them when they switch tabs
        for (int i = 0; i < _visitationL1Length; i++)
        {
            if (GameManager.Instance.SceneData.VisitationList1F[i])
            {
                _1FHUDImages[i].enabled = true;
            }
        }

        for (int i = 0; i < _visitationL2Length; i++)
        {
            if (GameManager.Instance.SceneData.VisitationList2F[i])
            {
                _2FHUDImages[i].enabled = true;
            }
        }

        for (int i = 0; i < _visitationL3Length; i++)
        {
            if (GameManager.Instance.SceneData.VisitationList3F[i] == true)
            {
                _3FHUDImages[i].enabled = true;
            }
        }
    }

    /// <summary>
    /// Configures map zone image states to all be inactive, and properly sets the active state of the current floor's map colliders
    /// </summary>
    public void HideHUDMapEnd()
    {
        // Reveals all explored images so the player can still see them when they switch tabs
        for (int i = 0; i < _visitationL1Length; i++)
        {
            _1FHUDImages[i].enabled = false;
        }

        for (int i = 0; i < _visitationL2Length; i++)
        {
            _2FHUDImages[i].enabled = false;
        }

        for (int i = 0; i < _visitationL3Length; i++)
        {

             _3FHUDImages[i].enabled = false;
        }

        _floorGameObjects[_currentHUDFloor - 1].gameObject.SetActive(false);
        _currentHUDFloor = _currentFloor;
        _floorSelector.transform.position = _floorNumbers[_currentHUDFloor - 1].transform.position;
        _floorGameObjects[_currentHUDFloor - 1].gameObject.SetActive(true);
    }

    /// <summary>
    /// Only called when player is triggered to entering a new zone.
    /// </summary>
    private void UpdateHUDMap(int room1) // Updating parts of the map that are visible (event listener)
    {
        if (_currentFloor == 1)
        {
            // Visual update in scene
            if (_mapTabContents.activeSelf)
            {
                _1FHUDImages[room1].enabled = true;
            }

            // Save data update in GM
            GameManager.Instance.SceneData.VisitationList1F[room1] = true;
        }
        else if (_currentFloor == 2)
        {
            // Visual update in scene
            if (_mapTabContents.activeSelf)
            {
                _2FHUDImages[room1].enabled = true;
            }

            // Save data update in GM
            GameManager.Instance.SceneData.VisitationList2F[room1] = true;
        }
        else if (_currentFloor == 3)
        {
            // Visual update in scene
            if (_mapTabContents.activeSelf)
            {
                _3FHUDImages[room1].enabled = true;
            }

            // Save data update in GM
            GameManager.Instance.SceneData.VisitationList3F[room1] = true;
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateFlavorText();
    }

    private void UpdatePosition() // Updating position dot
    {
        float hudXPos = 0;
        float hudYPos = 0;

        float hudZPos = 0; // Purely for narrative purposes

        // Floor check! 
        _dotScript.ToggleState(_currentFloor == _currentHUDFloor);
        // Tab check! The dot still needs to be active when tabbed out of map, but we turn the image component off
        _dotScript.ToggleEnabled(_mapTabContents.activeSelf);


        // Updating position correctly based on the bounds of the given floor
        if (_currentFloor == 1)
        {
            hudXPos = math.remap(_1FGameRange.x, _1FGameRange.y, _1FHUDRange.x, _1FHUDRange.y, _mainHUD.PlayerTransform.position.x);
            hudYPos = math.remap(_1FGameRange.z, _1FGameRange.w, _1FHUDRange.z, _1FHUDRange.w, _mainHUD.PlayerTransform.position.z);
            hudZPos = 0f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight);

        }
        else if (_currentFloor == 2)
        {
            hudXPos = math.remap(_2FGameRange.x, _2FGameRange.y, _2FHUDRange.x, _2FHUDRange.y, _mainHUD.PlayerTransform.position.x);
            hudYPos = math.remap(_2FGameRange.z, _2FGameRange.w, _2FHUDRange.z, _2FHUDRange.w, _mainHUD.PlayerTransform.position.z);
            hudZPos = 15f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight); 
        }
        else if (_currentFloor == 3)
        {
            hudXPos = math.remap(_3FGameRange.x, _3FGameRange.y, _3FHUDRange.x, _3FHUDRange.y, _mainHUD.PlayerTransform.position.x);
            hudYPos = math.remap(_3FGameRange.z, _3FGameRange.w, _3FHUDRange.z, _3FHUDRange.w, _mainHUD.PlayerTransform.position.z);
            hudZPos = 30f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight); 
        }
        _dotScript.transform.localPosition = new Vector2(hudXPos, hudYPos);
        _dotScript.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _mainHUD.PlayerTransform.rotation.eulerAngles.y));

        // Display coordinates are based off of the HUD; that way, we don't have to correctly center things in the 2 scenes
        _coordsText.text = "localusercoords:\nX " + System.Math.Round(hudXPos, 2) + "\nY " + System.Math.Round(hudYPos, 2) + "\nZ " + System.Math.Round(hudZPos, 2);
    }

    private void UpdateFloor() // Switches floors when the up or down arrows are hit
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

        if (arrowInput.y > 0)
        {
            _currentHUDFloor++;

            if (_currentHUDFloor > 3)
            {
                _currentHUDFloor = 3; // Infinite scrolling is NOT enabled, that's too easy and useful for a horror interface
            }
            else
            {
                _floorGameObjects[_currentHUDFloor - 2].gameObject.SetActive(false);
                _floorSelector.transform.position = _floorNumbers[_currentHUDFloor - 1].transform.position;
                _floorGameObjects[_currentHUDFloor - 1].gameObject.SetActive(true);
            }
        }
        else if (arrowInput.y < 0)
        {   
            _currentHUDFloor--;
            if (_currentHUDFloor < 1)
            {
                _currentHUDFloor = 1;
            }
            else
            {
                _floorGameObjects[_currentHUDFloor].gameObject.SetActive(false);
                _floorSelector.transform.position = _floorNumbers[_currentHUDFloor - 1].transform.position;
                _floorGameObjects[_currentHUDFloor - 1].gameObject.SetActive(true);
            }
        }   
    }

    private void UpdateFlavorText()
    {
        if (!_isFlavorAnimRunning)
        {
            StartCoroutine(DoUpdateFlavorText());
        }
    }

    private IEnumerator DoUpdateFlavorText()
    {
        _isFlavorAnimRunning = true;
        _flavorText.text = ">Farflung Station\n> stationindex 7804 - F\n> recordedpopulation 38\n" +
            "> ERROR: network offline, cannot update from station servers\n> deadreckoning enabled";
        yield return new WaitForSeconds(10f);

        float connectionCount = 0f;

        while(connectionCount < 9f)
        {
            _flavorText.text = ">Farflung Station\n> stationindex 7804 - F\n> recordedpopulation 38\n" +
            "> \\ Attemping to connect to network...";
            yield return new WaitForSeconds(.25f);
            _flavorText.text = ">Farflung Station\n> stationindex 7804 - F\n> recordedpopulation 38\n" +
                "> | Attemping to connect to network...";
            yield return new WaitForSeconds(.25f);
            _flavorText.text = ">Farflung Station\n> stationindex 7804 - F\n> recordedpopulation 38\n" +
                "> / Attemping to connect to network...";
            yield return new WaitForSeconds(.25f);
            _flavorText.text = ">Farflung Station\n> stationindex 7804 - F\n> recordedpopulation 38\n" +
                "> - Attemping to connect to network...";
            yield return new WaitForSeconds(.25f);
            connectionCount++;
        }
        _isFlavorAnimRunning = false;

    }

    public void SetLocationText(string txt)
    {
        _locationText.text = ">locationdata\n>roomname \"" + txt + "\"";
    }
}
