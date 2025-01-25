using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class MapTabController : MonoBehaviour
{
    [Header("Position Dot")]
    [SerializeField] private Image _dot;
    [SerializeField] private Transform _player;

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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateHUDMap();
    }

    private void UpdatePosition() // Updating position dot
    {
        int floor = GameManager.Instance.SceneData.floor;
        float hudXPos = 0;
        float hudYPos = 0;

        // Floor check! Gotta be in the right place
        if(floor == 1)
        {
            hudXPos = math.remap(_1FGameRange.x, _1FGameRange.y, _1FHUDRange.x, _1FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_1FGameRange.z, _1FGameRange.w, _1FHUDRange.z, _1FHUDRange.w, _player.position.z);

        }
        else if (floor == 2)
        {
            hudXPos = math.remap(_2FGameRange.x, _2FGameRange.y, _2FHUDRange.x, _2FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_2FGameRange.z, _2FGameRange.w, _2FHUDRange.z, _2FHUDRange.w, _player.position.z);
        }
        else if (floor == 3)
        {
            hudXPos = math.remap(_3FGameRange.x, _3FGameRange.y, _3FHUDRange.x, _3FHUDRange.y, _player.position.x);
            hudYPos = math.remap(_3FGameRange.z, _3FGameRange.w, _3FHUDRange.z, _3FHUDRange.w, _player.position.z);
        }
        _dot.transform.localPosition = new Vector2(hudXPos, hudYPos);
    }

    private void UpdateHUDMap() // Updating parts of the map that are visible
    {

    }
}
