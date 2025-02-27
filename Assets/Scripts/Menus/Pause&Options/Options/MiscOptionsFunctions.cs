using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains misc. functions for UI buttons for the options tabs which are otherwise not handled by self-contained scripts.
/// This is purely a convenience to avoid having to make a ton of one line scripts.
/// </summary>
public class MiscOptionsFunctions : MonoBehaviour
{
    #region VOLUME TAB
    /// <summary>
    /// Called by reset volume to default button in Volume Tab.
    /// </summary>
    public void ResetVolumeToDefaults()
    {
        GameManager.Instance.OptionsData.ResetVolumeToDefaults();
    }
    #endregion
}
