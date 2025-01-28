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
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;

    [Header("Supplemental Text")]
    [SerializeField] private TextMeshProUGUI _coordsText;

    [Header("Ranges of Motion")]
    // The X and Y (or X and Z) ranges that (a) the dot can move within in the HUD and (b) the player can move within in the game
    // Format of -x to x, -y to y / -x to x, -z to z
    [SerializeField] private Vector4 _1FHUDRange;
    [SerializeField] private Vector4 _1FGameRange;
    [SerializeField] private Vector4 _2FHUDRange;
    [SerializeField] private Vector4 _2FGameRange;
    [SerializeField] private Vector4 _3FHUDRange;
    [SerializeField] private Vector4 _3FGameRange;

    [Header("HUD Image Lists")]
    [SerializeField] private List<Image> _1FHUDImages;
    [SerializeField] private List<Image> _2FHUDImages;
    [SerializeField] private List<Image> _3FHUDImages;

    private int _currentFloor;      // The floor the player is on
    private int _currentHUDFloor;   // The floor the HUD is displaying

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

        RevealHUDMapStart(); // Make sure everywhere that's been enabled is visible
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        CheckMapOverlap();
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
            hudXPos = math.remap(_1FGameRange.x, _1FGameRange.y, _1FHUDRange.x, _1FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_1FGameRange.z, _1FGameRange.w, _1FHUDRange.z, _1FHUDRange.w, _player.position.z);
            hudZPos = 0f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight);

        }
        else if (_currentFloor == 2)
        {
            hudXPos = math.remap(_2FGameRange.x, _2FGameRange.y, _2FHUDRange.x, _2FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_2FGameRange.z, _2FGameRange.w, _2FHUDRange.z, _2FHUDRange.w, _player.position.z);
            hudZPos = 15f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight); 
        }
        else if (_currentFloor == 3)
        {
            hudXPos = math.remap(_3FGameRange.x, _3FGameRange.y, _3FHUDRange.x, _3FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_3FGameRange.z, _3FGameRange.w, _3FHUDRange.z, _3FHUDRange.w, _player.position.z);
            hudZPos = 30f + (_mainHUD.PlayerTransform.position.y - _baselinePlayerHeight); 
        }
        _dot.transform.localPosition = new Vector2(hudXPos, hudYPos);

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

    private void CheckMapOverlap() // Function that highlights the room the player is currently in
    {
        if (_currentFloor == 1)
        {
            /*for (int i = 0; i < _1FHUDImages.Count; i++)
            {
                if(CheckRectOverlap(_dot.rectTransform, _1FHUDImages[i].rectTransform))
                {
                    _1FHUDImages[i].color = _activeColor;
                }
                else
                {
                    _1FHUDImages[i].color = _inactiveColor;
                }
            }*/
        }
        else if (_currentFloor == 2)
        {

        }
        else if (_currentFloor == 3)
        {

        }
    }

    /*private bool CheckRectOverlap(RectTransform rectTrans1, RectTransform rectTrans2) // Helper function for seeing if UI elements overlap
    {
        return WorldRect(rectTrans1).Overlaps(WorldRect(rectTrans2));
    }

    // Credit to https://stackoverflow.com/questions/42043017/check-if-ui-elements-recttransform-are-overlapping for this sol'n
    // for ensuring rectangles are compared appropriately
    public static Rect WorldRect(RectTransform rectTransform)
    {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        Vector2 pivot = rectTransform.pivot;

        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        //With this it works even if the pivot is not at the center
        Vector3 position = rectTransform.TransformPoint(rectTransform.rect.center);
        float x = position.x - rectTransformWidth * 0.5f;
        float y = position.y - rectTransformHeight * 0.5f;
        Debug.Log("X: " + x + " Y: " + y + " W/H: " + rectTransformHeight + " " + rectTransformHeight);

        return new Rect(x, y, rectTransformWidth, rectTransformHeight);
    }*/
}
