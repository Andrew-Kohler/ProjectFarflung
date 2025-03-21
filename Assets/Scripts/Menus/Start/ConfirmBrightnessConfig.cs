using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains function for indicating new game save has been started
/// </summary>
public class ConfirmBrightnessConfig : MonoBehaviour
{
    public void ConfirmNewSaveBegun()
    {
        GameManager.Instance.SceneData.NewGameStarted = true;
        GameManager.Instance.SaveSceneDataToGameData(); // ensure new save state transfers not only to scene data but also to game data
    }
}
