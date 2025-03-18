using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles auto interact effects of looking at respawn auto interactable, which triggers respawn sequence.
/// </summary>
public class RespawnAutoInteractable : MonoBehaviour
{
    [Header("Scene Swap Specifics")]
    [SerializeField, Tooltip("Name of scene this goes to")] private string _sceneName;
    [SerializeField, Tooltip("Index of the spot the player should be placed at in the next scene")] private int _loadSpot;
    [SerializeField, Tooltip("Transition handler")] private SceneTransitionHandler _handler;

    public void InteractEffects()
    {
        GameManager.Instance.LoadPoint = _loadSpot;
        _handler.LoadScene(_sceneName);
    }
}
