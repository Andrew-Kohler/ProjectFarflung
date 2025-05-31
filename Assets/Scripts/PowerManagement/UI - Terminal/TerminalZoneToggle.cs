using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles interfacing between toggle and game manager data for power management systems.
/// </summary>
public class TerminalZoneToggle : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Zone index of the region this toggle controls.")]
    public int ZoneIndex;
    [Tooltip("Zone name of the region this toggle controls.")]
    public string ZoneName;

    [Header("References")]
    [SerializeField, Tooltip("Used to actually call the functional toggle of powered elements on the power system prefab instance.")]
    public TerminalConfiguration Terminal;
    [SerializeField, Tooltip("Used to read the state of the toggle.")]
    private Toggle _toggle;
    [SerializeField, Tooltip("Enabled to indicate locked state.")]
    private GameObject _lockedIndicator;
    [SerializeField, Tooltip("Enabled to indicated powered state.")]
    private GameObject _poweredIndicator;
    [SerializeField, Tooltip("Text that displays name on hover")]
    private TextMeshProUGUI _zoneNameText;

    [SerializeField, Tooltip("Numbers in ZoneConsumptionDisplay")]
    private TextMeshProUGUI _consumptionDisplayText;
    [SerializeField, Tooltip("Bar between said numbers")]
    private GameObject _consumptionDisplayBar;

    private PowerSystem _powerSystem;

    private void Awake()
    {
        _powerSystem = Terminal.PowerSystem;

        // Precondition: zone index must be valid
        if (ZoneIndex < 0 || ZoneIndex >= GameManager.Instance.SceneData.PoweredZones.Length || ZoneIndex >= GameManager.Instance.SceneData.TerminalUnlocks.Length)
            throw new System.Exception("Invalid zone index: toggle MUST be in range of Game Manager's powered zones list.");

        // initial configuration
        UpdatePoweredState();
        UpdateLockedState();
    }

    // Update is called once per frame
    void Update()
    {
        // update toggle with changes (changes made in other panels)
        if (_toggle.isOn != GameManager.Instance.SceneData.PoweredZones[ZoneIndex])
            UpdatePoweredState();

        // update toggle interactability
        if (_toggle.interactable != GameManager.Instance.SceneData.TerminalUnlocks[ZoneIndex])
            UpdateLockedState();
    }

    /// <summary>
    /// Updates isOn state of toggle and powered indicator to match game manager values.
    /// Useful for when a state is changed by a DIFFERENT button either within the same terminal or in a different terminal.
    /// </summary>
    private void UpdatePoweredState()
    {
        // set toggle to initial state of zone
        // without notify!! otherwise TogglePowerZone could be called on Awake, causing PoweredZone to call functions on ininitialized list
        // random behavior since it would depend on which Awake() method happened first (pseudodrandom)
        _toggle.SetIsOnWithoutNotify(GameManager.Instance.SceneData.PoweredZones[ZoneIndex]);
        _poweredIndicator.SetActive(_toggle.isOn); // powered indicator should match toggle

        if (!_poweredIndicator.activeSelf)
        {
            _consumptionDisplayText.color = Color.gray;
            _consumptionDisplayBar.GetComponent<Image>().color = Color.gray;
        }

        else
        {
            _consumptionDisplayText.color = Color.white;
            _consumptionDisplayBar.GetComponent<Image>().color = Color.white;
        }
            
    }

    /// <summary>
    /// Updates interactable stte of toggle and locked indicator to match game manager values.
    /// </summary>
    private void UpdateLockedState()
    {
        _toggle.interactable = GameManager.Instance.SceneData.TerminalUnlocks[ZoneIndex];
        _lockedIndicator.SetActive(!_toggle.interactable);

/*        if (!_lockedIndicator.activeSelf)
        {
            _consumptionDisplayText.color = Color.white;
            _consumptionDisplayBar.GetComponent<Image>().color = Color.white; 
        }*/
    }

    /// <summary>
    /// Called by the UI toggle to change the values in the game manager.
    /// </summary>
    public void TogglePowerZone()
    {
        // set game manager data to match toggle state on toggle change
        GameManager.Instance.SceneData.PoweredZones[ZoneIndex] = _toggle.isOn;
        // ensure powered indicator matches change
        _poweredIndicator.SetActive(_toggle.isOn);
        // ensure power elements are properly configured after UI press
        _powerSystem.PoweredZones[ZoneIndex].UpdatePowerStates();

        // check for grid shutdown
        if (_powerSystem.GetCurrentConsumption() > _powerSystem.GetCapacity())
        {
            _powerSystem.ShutdownGrid();

            // grid shutdown SFX
            AudioManager.Instance.PlayGridOverload();
        }
        // NOT shutting down grid
        else
        {
            // play proper power/unpower zone SFX & change visuals
            if (GameManager.Instance.SceneData.PoweredZones[ZoneIndex] == true)
            {
                AudioManager.Instance.PlayPowerZone();
                _consumptionDisplayText.color = Color.white;
                _consumptionDisplayBar.GetComponent<Image>().color = Color.white;
            }

            else
            {
                AudioManager.Instance.PlayUnpowerZone();
                _consumptionDisplayText.color = Color.gray;
                _consumptionDisplayBar.GetComponent<Image>().color = Color.gray;
            }
                
        }
    }

    public void ShowZoneName()
    {
        _zoneNameText.text = ZoneName;
    }

    public void HideZoneName()
    {
        _zoneNameText.text = "";
    }
}
