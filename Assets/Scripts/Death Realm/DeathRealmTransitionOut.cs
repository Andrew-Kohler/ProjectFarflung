using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains function used by Respawn/Death Animation animators to functionally change scenes once the animation ends.
/// </summary>
public class DeathRealmTransitionOut : MonoBehaviour
{
    [Header("Scene Swap")]
    [SerializeField, Tooltip("Transition handler")] private SceneTransitionHandler _handler;

    public void DoRespawnTransitionOut()
    {
        // exit death realm data - AND save game state again now that you are out
        GameManager.Instance.SceneData.IsInDeathRealm = false;
        GameManager.Instance.SaveSceneDataToGameData();

        // restore flashlight battery on respawn since you respawn at a terminal
        GameManager.FlashlightCharge = 1f;

        // respawn SFX
        AudioManager.Instance.PlayRespawn();

        _handler.LoadScene("Resume");
    }

    public void DoDeathTransitionOut()
    {
        // clear game data - GAME OVER state
        GameManager.Instance.ResetGameData();

        // return to start menu
        _handler.LoadScene("StartMenu");
    }
}
