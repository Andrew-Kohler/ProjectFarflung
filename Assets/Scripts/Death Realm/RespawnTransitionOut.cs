using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains function used by Respawn Animation animator to functionally change scenes once the animation ends.
/// </summary>
public class RespawnTransitionOut : MonoBehaviour
{
    [Header("Scene Swap")]
    [SerializeField, Tooltip("Name of scene this goes to")] private string _sceneName;
    [SerializeField, Tooltip("Transition handler")] private SceneTransitionHandler _handler;

    public void DoTransitionOut()
    {
        // exit death realm data - AND save game state again now that you are out
        GameManager.Instance.SceneData.IsInDeathRealm = false;
        GameManager.Instance.SaveSceneDataToGameData();

        _handler.LoadScene(_sceneName);
    }
}
