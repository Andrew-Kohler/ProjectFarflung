using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwapInteractable : Interactable
{
    [Header("Scene Swap Specifics")]
    [SerializeField, Tooltip("Name of scene this goes to")] 
    private string _sceneName;
    [SerializeField, Tooltip("Index of the spot the player should be placed at in the next scene")] 
    private int _loadSpot;
    [SerializeField, Tooltip("Transition handler")] 
    private SceneTransitionHandler _handler;
    [SerializeField, Tooltip("Key required to use this scene swap interactable. Default if none.")]
    private PoweredDoor.KeyType _requiredKey;

    [Header("Display of Use Requirements")]
    [SerializeField, Tooltip("Whether use requirements should be displayed at all - EXISTS TO PREVENT ERRORS FOR UNCONFIGURED INTERACTABLES")]
    private bool _displayReqs = false;
    [SerializeField, Tooltip("SpriteRenderer for displaying which keycard is needed")]
    private SpriteRenderer _requiredKeycard;
    [SerializeField, Tooltip("Image of the corresponding keycard icon - manually configured because there are so few of these")]
    private Sprite _requiredKeycardImage;
    [SerializeField, Tooltip("Parent of indicator sprites")]
    private GameObject _indicatorParent;
    [SerializeField, Tooltip("Parent of E to interact")]
    private GameObject _interactPromptParent;
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if(_displayReqs) UpdateKeycardIndicator();
    }

    public override void InteractEffects()
    {
        if (_requiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(_requiredKey.ToString())) // If they have the key
        {
            GameManager.Instance.LoadPoint = _loadSpot;
            _handler.LoadScene(_sceneName);
        }
        else
        {
            AudioManager.Instance.PlayDoorLocked();
        }
    }

    private void UpdateKeycardIndicator()
    {
        if (_requiredKey.ToString() == "Default" || GameManager.Instance.SceneData.Keys.Contains(_requiredKey.ToString())) // If they have the key
        {
            _indicatorParent.SetActive(false);
        }
        else
        {
            _requiredKeycard.sprite = _requiredKeycardImage;
            _indicatorParent.SetActive(true);
            _interactPromptParent.SetActive(false);
        }
    }
}
