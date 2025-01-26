using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles zone-level toggling of powered elements contained within the zone.
/// </summary>
public class PoweredZone : MonoBehaviour
{
    [SerializeField, Tooltip("Numbered index zone that this area corresponds with.")]
    private int _zoneIndex;
    [SerializeField, Tooltip("Used to power/unpower individual powered elements.")]
    private PoweredElement[] _poweredElements;

    private bool _isPowered;
    private float _maxPowerConsum = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Precondition: must have valid zone index
        if (_zoneIndex < 0 || _zoneIndex >= GameManager.Instance.SceneData.PoweredZones.Length)
            throw new System.Exception("Invalid zone index. Must be in range of Game Manager's powered zones list");

        // initial configuration
        UpdatePowerStates();

        // Calculate max power consumption
        foreach (PoweredElement elem in _poweredElements)
            _maxPowerConsum += elem.PowerDraw;
    }

    // Update is called once per frame
    void Update()
    {
        // if a change occurs
        if (_isPowered != GameManager.Instance.SceneData.PoweredZones[_zoneIndex])
        {
            UpdatePowerStates();
        }
    }

    /// <summary>
    /// Powers all or unpowers all elements of the zone, based on the powered state of the zone.
    /// </summary>
    private void UpdatePowerStates()
    {
        // power up each powered element
        if (GameManager.Instance.SceneData.PoweredZones[_zoneIndex])
        {
            _isPowered = true;
            foreach (PoweredElement elem in _poweredElements)
                elem.PowerUpZone();
        }
        // power down each powered element
        else
        {
            _isPowered = false;
            foreach (PoweredElement elem in _poweredElements)
                elem.PowerDownZone();
        }
    }

    /// <summary>
    /// Current power draw of all elements powered in this zone.
    /// </summary>
    public float GetCurrentConsumption()
    {
        // no consumption if zone not powered
        if (!_isPowered)
            return 0f;

        float currConsum = 0;
        foreach(PoweredElement elem in _poweredElements)
        {
            if (elem.IsPowered())
                currConsum += elem.PowerDraw;
        }
        return currConsum;
    }

    /// <summary>
    /// Maximum potential power draw if all elements in the zone are powered.
    /// </summary>
    public float GetMaxConsumption()
    {
        return _maxPowerConsum;
    }
}
