using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It just has the function to reset the volume
/// </summary>
public class ResetVolume : MonoBehaviour
{
    /// <summary>
    /// Called by reset volume to default button in Volume Tab.
    /// </summary>
    public void ResetVolumeToDefaults()
    {
        GameManager.Instance.OptionsData.ResetVolumeToDefaults();

        // UI Click SFX
        AudioManager.Instance.PlayClickUI();
    }
}