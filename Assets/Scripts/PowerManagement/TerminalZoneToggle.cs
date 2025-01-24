using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerminalZoneToggle : MonoBehaviour
{
    [SerializeField, Tooltip("Zone index of the region this toggle controls.")]
    private int _zoneIndex;
    [SerializeField, Tooltip("Used to read the state of the toggle.")]
    private Toggle _toggle;

    private void Awake()
    {
        // Precondition: zone index must be valid
        if (_zoneIndex < 0 || _zoneIndex >= GameManager.Instance.SceneData.PoweredZones.Length || _zoneIndex >= GameManager.Instance.SceneData.TerminalUnlocks.Length)
            throw new System.Exception("Invalid zone index: toggle MUST be in range of Game Manager's powered zones list.");

        // set toggle to initial state of zone
        _toggle.isOn = GameManager.Instance.SceneData.PoweredZones[_zoneIndex];
    }

    // Update is called once per frame
    void Update()
    {
        // update toggle with changes (changes made in other panels)
        if (_toggle.isOn != GameManager.Instance.SceneData.PoweredZones[_zoneIndex])
            _toggle.isOn = GameManager.Instance.SceneData.PoweredZones[_zoneIndex];
    }

    /// <summary>
    /// Called by the UI toggle to change the values in the game manager.
    /// </summary>
    public void TogglePowerZone()
    {
        // set game manager data to match toggle state on toggle change
        GameManager.Instance.SceneData.PoweredZones[_zoneIndex] = _toggle.isOn;
    }
}
