using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalIcon : MonoBehaviour
{
    [SerializeField, Tooltip("Zone index of this terminal")]
    private int _zoneIndex;
    [SerializeField, Tooltip("Floor parent object reference")]
    private GameObject _floorParent;
    [SerializeField, Tooltip("Map tab contents reference")]
    private GameObject _mapTabContents;

    private Image _img;
    void Start()
    {
        _img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.SceneData.TerminalUnlocks[_zoneIndex] && _floorParent.activeSelf && _mapTabContents.activeSelf)
        {
            _img.enabled = true;
        }
        else
            _img.enabled = false;
    }
}
