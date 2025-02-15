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

    private PoweredElement[] _poweredElements;
    private bool _isPowered;

    private void Awake()
    {
        // Precondition: must have valid zone index
        if (_zoneIndex < 0 || _zoneIndex >= GameManager.Instance.SceneData.PoweredZones.Length)
            throw new System.Exception("Invalid zone index. Must be in range of Game Manager's powered zones list");

        // fetch powered elements from children
        // easier configuration then assigning manually in inspector, and it only happens once per scene
        _poweredElements = transform.GetComponentsInChildren<PoweredElement>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // initial configuration
        UpdatePowerStates();
    }

    /// <summary>
    /// Powers all or unpowers all elements of the zone, based on the powered state of the zone.
    /// Can be called externally to sync this object with game manager data.
    /// </summary>
    public void UpdatePowerStates()
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
    public int GetCurrentConsumption()
    {
        // no consumption if zone not powered
        if (!_isPowered)
            return 0;

        int currConsum = 0;
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
    public int GetMaxConsumption()
    {
        int maxConsum = 0;
        foreach (PoweredElement elem in _poweredElements)
            maxConsum += elem.PowerDraw;
        return maxConsum;
    }

    /// <summary>
    /// The powered state of the zone.
    /// </summary>
    public bool IsPowered()
    {
        return _isPowered;
    }
}
