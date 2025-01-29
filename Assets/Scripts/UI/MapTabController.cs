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

    private float _baselinePlayerHeight;

    [Header("Position Dot")]
    [SerializeField] private Image _dot;
    [SerializeField] private Transform _player;

    [Header("Supplemental Text")]
    [SerializeField] private TextMeshProUGUI _coordsText;
    [SerializeField] private TextMeshProUGUI _locationText;

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
    [SerializeField] private GameObject floorSelector;

    [Header("Room Image Lists")]
    [SerializeField] private List<Image> _1FHUDImages;
    [SerializeField] private List<Image> _2FHUDImages;
    [SerializeField] private List<Image> _3FHUDImages;

    private int _currentFloor;      // The floor the player is on
    private int _currentHUDFloor;   // The floor the HUD is displaying

    private InputAction _arrowAction;

    private void OnEnable()
    {
        HUDMapRevealer.onPassthrough += UpdateHUDMap;
    }

    private void OnDisable()
    {
        HUDMapRevealer.onPassthrough -= UpdateHUDMap;
    }

    void Start()
    {
        _currentFloor = GameManager.Instance.SceneData.floor;
        _currentHUDFloor = _currentFloor;

        _baselinePlayerHeight = _mainHUD.PlayerTransform.position.y;

        _arrowAction = _mainHUD.PlayerInput.actions.FindAction("Arrows");
        _arrowAction.Enable();

        RevealHUDMapStart(); // Make sure everywhere that's been enabled is visible
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateFloor();
    }

    private void UpdatePosition() // Updating position dot
    {
        float hudXPos = 0;
        float hudYPos = 0;

        float hudZPos = 0; // Purely for narrative purposes

        // Floor check! Gotta be in the right place
        if(_currentFloor == _currentHUDFloor)
        {
            _dot.gameObject.SetActive(true);
        }
        else
        {
            _dot.gameObject.SetActive(false);
        }

        // Updating position correctly based on the bounds of the given floor
        if(_currentFloor == 1)
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
        _dot.transform.localPosition = new Vector2(hudXPos, hudYPos);
        _dot.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, _mainHUD.PlayerTransform.rotation.eulerAngles.y));
            
          //  .eulerAngles.Set(); //=  new Vector3();

        // Display coordinates are based off of the HUD; that way, we don't have to correctly center things in the 2 scenes
        _coordsText.text = "localusercoords:\nX " + System.Math.Round(hudXPos, 2) + "\nY " + System.Math.Round(hudYPos, 2) + "\nZ " + System.Math.Round(hudZPos, 2);
    }

    private void RevealHUDMapStart() // On scene start, show everything the player has been able to explore
    {
        // Floor check! Gotta be in the right place
        if (_currentFloor == 1)
        {
            for(int i = 0; i < GameManager.Instance.SceneData.VisitationList1F.Count; i++)
            {
                if (GameManager.Instance.SceneData.VisitationList1F[i] == true) 
                {
                    _1FHUDImages[i].gameObject.SetActive(true);
                }
            }
        }
        else if (_currentFloor == 2)
        {

        }
        else if (_currentFloor == 3)
        {

        }
    }

    private void UpdateHUDMap(int room1, int room2) // Updating parts of the map that are visible (event listener)
    {
        if (_currentFloor == 1)
        {
            // Visual update in scene
            _1FHUDImages[room1].gameObject.SetActive(true);
            _1FHUDImages[room2].gameObject.SetActive(true);

            // Save data update in GM
            GameManager.Instance.SceneData.VisitationList1F[room1] = true;
            GameManager.Instance.SceneData.VisitationList1F[room2] = true;
        }
        else if (_currentFloor == 2)
        {

        }
        else if (_currentFloor == 3)
        {

        }
        
    }

    private void UpdateFloor() // Switches floors when the up or down arrows are hit
    {
        Vector2 arrowInput = _arrowAction.ReadValue<Vector2>();
        if (arrowInput.y > 0)
        {
            _currentHUDFloor++;
            if(_currentHUDFloor > 3) 
            {
                _currentHUDFloor = 3; // Infinite scrolling is NOT enabled, that's too easy and useful for a horror interface
            }
        }
        else if(arrowInput.y < 0)
        {
            _currentHUDFloor--;
            if (_currentHUDFloor < 1)
            {
                _currentHUDFloor = 1;
            }
        }
    }

    public void SetLocationText(string txt)
    {
        _locationText.text = ">locationdata\n>roomname \"" + txt + "\"";
    }

    private IEnumerator DoFloorSwap()
    {
        yield return new WaitForSeconds(.2f);
    }

}
