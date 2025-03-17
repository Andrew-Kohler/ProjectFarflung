using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwapInteractable : Interactable
{
    [Header("Scene Swap Specifics")]
    [SerializeField, Tooltip("Name of scene this goes to")] private string _sceneName;
    [SerializeField, Tooltip("Index of the spot the player should be placed at in the next scene")] private int _loadSpot;
    [SerializeField, Tooltip("Transition handler")] private SceneTransitionHandler _handler;
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
        GameManager.Instance.LoadPoint = _loadSpot;
        GameManager.Instance.PlayerEnabled = false;
        _handler.LoadScene(_sceneName);
    }
}
