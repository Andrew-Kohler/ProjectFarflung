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
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {

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
            // TODO: negative feedback for unable to interact
        }
    }
}
